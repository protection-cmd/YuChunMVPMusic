using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace YuChunMVPMusic.MVPSystem
{
    public class MvpEvent
    {
        public void WaitingForPlayer()
        {
            Server.FriendlyFire = false;
            MvpEvent.PlayerKillCount.Clear();
        }

        public void Verified(VerifiedEventArgs ev)
        {
            bool flag = ev.Player != null;
            if (flag)
            {
                Timing.CallDelayed(0.5f, delegate ()
                {
                    MvpEvent.PlayerKillCount[ev.Player] = 1;
                });
            }
        }

        public void Dying(DyingEventArgs ev)
        {
            bool flag = ev.Player != null && ev.Attacker != null;
            if (flag)
            {
                bool flag3 = MvpEvent.PlayerKillCount.ContainsKey(ev.Attacker);
                if (flag3)
                {
                    bool isHuman = ev.Player.IsHuman;
                    if (isHuman)
                    {
                        Dictionary<Player, int> playerKillCount = MvpEvent.PlayerKillCount;
                        Player attacker = ev.Attacker;
                        int num = playerKillCount[attacker];
                        playerKillCount[attacker] = num + 1;
                    }

                    bool isScp = ev.Player.IsScp;
                    if (isScp)
                    {
                        Dictionary<Player, int> playerKillCount2 = MvpEvent.PlayerKillCount;
                        Player attacker2 = ev.Attacker;
                        playerKillCount2[attacker2] += 5;
                    }
                }
            }
        }

        public static Dictionary<Player, int> PlayerKillCount = new Dictionary<Player, int>();
    }
}

