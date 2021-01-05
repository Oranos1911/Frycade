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
using Java.Util;

namespace Doodle
{

    class LeaderViewHolder : RecyclerView.ViewHolder
    {
        private Context context;
        public TextView Name { get; set; }
        public TextView Score { get; set; }

        public LeaderViewHolder (Context context , View itemView) : base(itemView)
        {
            this.context = context;
            Name = itemView.FindViewById<TextView>(Resource.Id.tv_player_name);
            Score = itemView.FindViewById<TextView>(Resource.Id.tv_player_score);
        }

    }

    class LeaderAdpater : RecyclerView.Adapter
    {

        Context context;
        List<PlayerData> objects;

        public LeaderAdpater(Context context , List<PlayerData> objects)
        {
            this.context = context;
            this.objects = objects;
        }

        public override int ItemCount => objects.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            View itemView = inflater.Inflate(Resource.Layout.list_leaders, parent, false);

            LeaderViewHolder holder = new LeaderViewHolder(context, itemView);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            LeaderViewHolder cholder = (LeaderViewHolder) holder;

            cholder.Name.Text = objects[position].Name;
            cholder.Score.Text = objects[position].Score;
        }

    }
}