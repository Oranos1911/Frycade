using System;
using System.Collections.Generic;

namespace Doodle
{
    public class TimerManager
    {

        private static double monsterStreakTimeInterval = 2000;
        private static double rampStreakTimeInterval = 2500;

        public int MonsterStreak { get; set; }
        public int RampStreak { get; set; }

        private List<DateTime> monsterClock , rampClock;

        public TimerManager()
        {
            monsterClock = new List<DateTime>();
            MonsterStreak = 0;
            rampClock = new List<DateTime>();
            RampStreak = 0;    
        }

        public bool NotifyRampJump()
        {
            rampClock.Add(DateTime.Now);
            if (rampClock.Count > 1)
                FindRampStreak();
            else
                RampStreak++;

            if (RampStreak > 1)
                return true;
            return false;
        }
        private void FindRampStreak()
        {
            double delta = rampClock[rampClock.Count - 1].Subtract(rampClock[rampClock.Count - 2]).TotalMilliseconds;
            if (delta < rampStreakTimeInterval)
                RampStreak++;
            else
                ResetRampStreak();
        }

        public void ResetRampStreak()
        {
            RampStreak = 1;
            rampClock.Clear();
            rampClock.Add(DateTime.Now);
        }

        public bool NotifyMonsterKill()
        {
            monsterClock.Add(DateTime.Now);
            if (monsterClock.Count > 1)
                FindMonsterStreak();
            else
                MonsterStreak++;

            if (MonsterStreak > 1)
                return true;
            return false;
        }

     
        private void FindMonsterStreak()
        {
            double delta = monsterClock[monsterClock.Count - 1].Subtract(monsterClock[monsterClock.Count - 2]).TotalMilliseconds;
            if (delta < monsterStreakTimeInterval)
                MonsterStreak++;
            else
                ResetMonsterStreak();
        }

        public void ResetMonsterStreak()
        {
            MonsterStreak = 1;
            monsterClock.Clear();
            monsterClock.Add(DateTime.Now);
        }
    }
}