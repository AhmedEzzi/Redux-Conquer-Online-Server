using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redux.Managers;
using Redux.Packets.Game;
using Redux.Enum;
namespace Redux.Events
{
    public class DisCity
    {
        //Run dis city every Monday, Wednesday and Friday at 6PM
        public static List<DayOfWeek> DIS_CITY_DAYS = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday };
        public static int DIS_CITY_HOUR = 18;

        private int _wraithsKilled = 0;
        private int _wraithsRequired = 300;
        public bool CanEnter;
        public DisCity()
        {
            PlayerManager.SendToServer(new TalkPacket(ChatType.GM, "DisCity is about to begin! Talk to SolarSaint in Ape City to sign up!"));
            CanEnter = true;
        }

        public void DisableEntry()
        {
            PlayerManager.SendToServer(new TalkPacket(ChatType.GM, "No more players may enter DisCity. Best of luck to those already inside."));
            CanEnter = false;
        }

        public void End()
        {
            PlayerManager.SendToServer(new TalkPacket(ChatType.GM, "DisCity has ended. Thank you to all those who participated!"));
            foreach(var player in PlayerManager.Players.Values)
            {
                /*Maps are 2021,2022,2023,2024*/
                if (player.MapID == 2021 ||
                    player.MapID == 2022 ||
                    player.MapID == 2023/* ||
                    player.MapID == 2024*/ //Players remain in this map after finish
                    )
                    player.ChangeMap((ushort)player.Character.Map, player.Character.X, player.Character.Y);
            }
            Common.DIS_CITY = null;
        }

        public void WraithKilled()
        {
            _wraithsKilled++;
            if (_wraithsKilled > _wraithsRequired && Managers.PlayerManager.Players.Values.Where(I => I.MapID == 2023).Count() > 0)
            {
                //For concurrency sake so there's no confusion with double rewards, lets set wraiths killed to 0
                _wraithsKilled = 0;
                foreach (var player in Managers.PlayerManager.Players.Values.Where(I => I.MapID == 2023))
                {
                    //Go to final map
                    player.ChangeMap(2024, 149, 285);

                    //gain 1.5 exp balls
                    player.GainExpBall(900);

                    //Send them a inspirational message
                    player.SendMessage("You've made it! Take out the Sirens protecting the UltimatePluto!");
                }
            }            
        }
    }
}
