using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Doodle
{

    public class CharacterViewHolder : RecyclerView.ViewHolder , View.IOnClickListener
    {
        private Context context;
        public ImageView Sprite { get; set; }
        public TextView Name { get; set; }

        public CharacterViewHolder(Context context , View itemView) : base(itemView)
        {
            this.context = context;
            Sprite = itemView.FindViewById<ImageView>(Resource.Id.iv_sprite);
            Name = itemView.FindViewById<TextView>(Resource.Id.tv_character_name);
            itemView.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            CharacterHelper.SetCurrent(Position);
            ((MainActivity)context).SaveTheme();
            ((MainActivity)context).UpdateRecyclerView();
        }

    }

    class CharacterAdapter : RecyclerView.Adapter
    {

        Context context;
        List<Character> objects;

        public CharacterAdapter(Context context , List<Character> objects)
        {
            this.context = context;
            this.objects = objects;
        }

        public override int ItemCount => objects.Count;


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            CharacterViewHolder cholder = (CharacterViewHolder) holder;

            cholder.Name.Text = objects[position].Name;
            cholder.Sprite.SetImageBitmap(objects[position].Sprite);
            if (position == CharacterHelper.Current)
                cholder.Sprite.SetBackgroundDrawable(context.Resources.GetDrawable(Resource.Drawable.sp_border));
            else
            {
                cholder.Sprite.SetBackgroundColor(Color.Transparent);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View itemView = inflater.Inflate(Resource.Layout.list_characters, parent, false);

            CharacterViewHolder holder = new CharacterViewHolder(context , itemView);
            return holder;
        }
    }
}