using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redux.Managers;

namespace Redux.Threading
{
    public class WorldThread:ThreadBase 
    {

        private const int TIMER_OFFSET_LIMIT = 10;
        private const int THREAD_SPEED = 1000;
        private const int INTERVAL_EVENT = 60; 
        
        private long _nextTrigger;
        protected override void OnInit()
        {
            _nextTrigger = Common.Clock + THREAD_SPEED;
        }
        protected override bool OnProcess()
        {
            var curr = Common.Clock;
            if (curr >= _nextTrigger)
            {
                _nextTrigger += THREAD_SPEED;

                var offset = (curr - _nextTrigger) / Common.MS_PER_SECOND;
                if (Math.Abs(offset) > TIMER_OFFSET_LIMIT)
                {
                    _nextTrigger = curr + THREAD_SPEED;
                }
         //start dis city
                    if (Events.DisCity.DIS_CITY_DAYS.Contains(DateTime.Now.DayOfWeek) && DateTime.Now.Hour == Events.DisCity.DIS_CITY_HOUR)
                    {
                        if (DateTime.Now.Minute == 0)
                            Common.DIS_CITY = new Events.DisCity();
                        else if (Common.DIS_CITY != null && DateTime.Now.Minute >= 15 && Common.DIS_CITY.CanEnter)
                            Common.DIS_CITY.DisableEntry();
                    }
                    else if (Common.DIS_CITY != null)
                        Common.DIS_CITY.End();
                //Run managers
                PlayerManager.PlayerManager_Tick();

                MapManager.MapManager_Tick();

                GuildWar.GuildWar_Tick();
            }

            return true;
        }
        protected override void OnDestroy()
        {
        }
    }
}
