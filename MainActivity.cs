using Android.App;
using Android.OS;
using Android.Gms.Tasks;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Content;
using System;
using Android.Graphics.Drawables;
using Android.Graphics;
using Java.Util;
using Firebase.Firestore;

namespace Doodle
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait ,Theme = "@style/AppTheme" , MainLauncher = true)]
    public class MainActivity :  AppCompatActivity , View.IOnClickListener , IOnSuccessListener , IOnFailureListener
    {

        FirebaseData fr;
        ISharedPreferences sharedPreferences;
        Button btnPlay , btnLeaderboard , btnSaveSettings , btnCancel;
        TextView tvError;
        EditText etName;
        CheckBox cbMuteSound, cbMuteMusic;
        RecyclerView rvCharacters;
        CharacterAdapter characterAdapter;
        LinearLayoutManager layoutManager;
        Dialog dgSettings , dgTheme;

        string requestedName;
        bool firstRun;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            firstRun = false;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            InitViews();
            fr = new FirebaseData();
            sharedPreferences = GetSharedPreferences(Constants.SHARED_PREFERENCES_NAME, FileCreationMode.Private);
            HandleFirstRun();
            UpdateVariables();
        }

        public void InitViews()
        {
            btnPlay = FindViewById<Button>(Resource.Id.btn_play);
            btnPlay.SetOnClickListener(this);
            btnLeaderboard = FindViewById<Button>(Resource.Id.btn_leaderboard);
            btnLeaderboard.SetOnClickListener(this);
        }

        private void HandleFirstRun()
        {
            if (sharedPreferences.GetString(Constants.SELECT_NICKNAME_KEY, "") == "")
            {
                firstRun = true;
                OpenSettingsDialog();
            }           
        }
        private void UpdateVariables()
        {           
            SoundManager.SetSounds(sharedPreferences.GetBoolean(Constants.MUTE_SOUNDS_KEY, false));
            SoundManager.SetMusic(sharedPreferences.GetBoolean(Constants.MUTE_MUSIC_KEY, false));
            CharacterHelper.SetCurrent(sharedPreferences.GetInt(Constants.SELECT_CHARACTER_KEY, 0));
        }

        public void UpdateRecyclerView()
        {
            characterAdapter.NotifyDataSetChanged();
        }

        private void UpdateRecord()
        {
            fr.GetPlayer(requestedName)
             .AddOnSuccessListener(this)
             .AddOnFailureListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (QuerySnapshot) result;
            var documents = snapshot.Documents;
            if(documents.Count != 0 && requestedName != sharedPreferences.GetString(Constants.SELECT_NICKNAME_KEY , ""))
            { 
                tvError.Text = "That name is already taken cowboy ;)";
                return;
            }

            if(!firstRun)
                fr.GetPlayerRef(sharedPreferences.GetString(Constants.SELECT_NICKNAME_KEY , "")).Delete();

            // Update new nickname to firebase
            var reference = fr.GetPlayerRef(requestedName);
            HashMap map = new HashMap();
            map.Put("name", requestedName);
            map.Put("score", sharedPreferences.GetInt(Constants.LAST_HIGH_SCORE_KEY , 0));
            reference.Set(map);

            // Update new nickname to SharedPreferences
            var editor = sharedPreferences.Edit();
            editor.PutString(Constants.SELECT_NICKNAME_KEY, requestedName);
            editor.Commit();

            // Close Dialog at the end
            CloseDialogs();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
        }
        public void SaveSettings()
        {
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutBoolean(Constants.MUTE_SOUNDS_KEY, cbMuteSound.Checked);
            editor.PutBoolean(Constants.MUTE_MUSIC_KEY, cbMuteMusic.Checked);
            editor.Commit();

            string name = etName.Text.ToUpper();

            // Handling differnet name limitations
            if (name.Length > Constants.MAX_NAME_LENGTH)
            {
                tvError.Text = String.Format("Your nickname is too long! choose a shorter name (Maximum {0} characters).", Constants.MAX_NAME_LENGTH);
                return;
            }
            if (name.Length < Constants.MIN_NAME_LENGTH)
            {
                tvError.Text = String.Format("Choose a nickname with at least {0} chaaracters.", Constants.MIN_NAME_LENGTH);
                return;
            }
            if(name.Contains("\n"))
            {
                tvError.Text = "Use A-z , a-z , 0-9 only.";
                return;
            }

            // Update new nickname to Firebase (if not duplicated)
            requestedName = name;
            UpdateRecord();

            // Handle different Settings
            firstRun = false;
            UpdateVariables();           
        }

        public void SaveTheme()
        {
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutInt(Constants.SELECT_CHARACTER_KEY, CharacterHelper.Current);
            editor.Commit();
            UpdateVariables();
        }

        public void CloseDialogs()
        {
            if (dgSettings != null)
                dgSettings.Dismiss();
            if (dgTheme != null)
                dgTheme.Dismiss();
        }
        public void OpenSettingsDialog()
        {
            dgSettings = new Dialog(this);
            dgSettings.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dgSettings.SetContentView(Resource.Layout.dialog_settings);
            dgSettings.SetCancelable(false);

            tvError = dgSettings.FindViewById<TextView>(Resource.Id.tv_error);
            etName = dgSettings.FindViewById<EditText>(Resource.Id.et_name);           
            btnSaveSettings = dgSettings.FindViewById<Button>(Resource.Id.btn_save);
            btnSaveSettings.SetOnClickListener(this);
            cbMuteSound = dgSettings.FindViewById<CheckBox>(Resource.Id.cb_muteSounds);
            cbMuteMusic = dgSettings.FindViewById<CheckBox>(Resource.Id.cb_muteMusic);

            if (sharedPreferences.GetBoolean(Constants.MUTE_SOUNDS_KEY, false))
                cbMuteSound.Checked = true;
            if (sharedPreferences.GetBoolean(Constants.MUTE_MUSIC_KEY, false))
                cbMuteMusic.Checked = true;

            etName.Text = sharedPreferences.GetString(Constants.SELECT_NICKNAME_KEY, "");

            dgSettings.Show();
        }

        private void OpenThemeDialog()
        {
            dgTheme = new Dialog(this);
            dgTheme.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dgTheme.SetContentView(Resource.Layout.dialog_theme);

            characterAdapter = new CharacterAdapter(this, CharacterHelper.GetList(this));
            layoutManager = new LinearLayoutManager(dgTheme.Context, LinearLayoutManager.Horizontal, true);
            rvCharacters = dgTheme.FindViewById<RecyclerView>(Resource.Id.rv_characters);
            rvCharacters.SetAdapter(characterAdapter);
            rvCharacters.SetLayoutManager(layoutManager);
            btnCancel = dgTheme.FindViewById<Button>(Resource.Id.btn_cancel);
            btnCancel.SetOnClickListener(this);

            dgTheme.Show();
        }



        public void OnClick(View v)
        {

            if (v == btnPlay)
            {
                Intent intent = new Intent(this, typeof(GameActivity));
                StartActivity(intent);
            }
            if(v == btnLeaderboard)
            {
                Intent intent = new Intent(this, typeof(LeaderActivity));
                StartActivity(intent);
            }
            if (v == btnSaveSettings)
                SaveSettings();
            if (v == btnCancel)
                CloseDialogs();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_settings)
                OpenSettingsDialog();
            if (item.ItemId == Resource.Id.action_theme)
                OpenThemeDialog();

            return base.OnOptionsItemSelected(item);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }
    }
}