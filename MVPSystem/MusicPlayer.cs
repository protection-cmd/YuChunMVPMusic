using AudioApi; 
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using LabApi.Loader.Features.Plugins;
using MEC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VoiceChat;
using YuChunMVPMusic.MVPSystem; 
using YuChunMVPMusic;

namespace YuRiJianQiuChunYu.MVPSystem
{
    public class MusicPlayer
    {
        private static VoicePlayerBase _globalPlayer;

        public void WaitingForPlayer()
        {
            ReleasePlayer(); // 确保清理播放器
            Server.FriendlyFire = false;
        }

        public static string TryPlayMusic(Player p)
        {
            // 清理旧播放器
            ReleasePlayer();

            if (!YuChunMVPMusic.Plugin.Instance.Config.MVPMusicPath.ContainsKey(p.UserId))
            {
                Log.Info("玩家没有配置音乐路径");
                return null;
            }

            if (p == null || !p.IsConnected)
            {
                Log.Info("玩家为空或已断开连接");
                return null;
            }

            List<string> musicList = YuChunMVPMusic.Plugin.Instance.Config.MVPMusicPath[p.UserId];
            if (!musicList.Any())
            {
                Log.Info($"玩家 {p.Nickname} 的音乐路径列表为空");
                return null;
            }

            // 限制最多3首
            if (musicList.Count > 3)
            {
                musicList = musicList.Take(3).ToList();
                Log.Warn($"玩家 {p.Nickname} 的音乐路径超过3个，已限制为前3个");
            }

            // 随机选择音乐
            Random random = new Random();
            string selectedMusic = musicList[random.Next(musicList.Count)];
            string musicName = Path.GetFileNameWithoutExtension(selectedMusic);

            try
            {
                // 为当前MVP玩家创建新播放器
                _globalPlayer = VoicePlayerBase.Get(p.ReferenceHub);
                if (_globalPlayer == null)
                {
                    Log.Error($"无法为玩家 {p.Nickname} 创建播放器");
                    return null;
                }

                _globalPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
                _globalPlayer.Volume = 100f;
                _globalPlayer.Loop = false;
                _globalPlayer.Shuffle = false;

                // 设置广播目标
                _globalPlayer.BroadcastTo.Clear();
                foreach (Player player in Player.List)
                {
                    if (player.IsConnected)
                    {
                        _globalPlayer.BroadcastTo.Add(player.Id);
                    }
                }

                // 播放音乐
                _globalPlayer.Enqueue(selectedMusic, -1);
                _globalPlayer.Play(0);

                // 返回带玩家名的音乐信息
                string displayInfo = $"{p.Nickname} 的专属音乐: {musicName}";
                Log.Info($"播放 {displayInfo}");
                return displayInfo;
            }
            catch (Exception ex)
            {
                Log.Error($"播放音乐时出错: {ex}");
                ReleasePlayer();
                return null;
            }
        }

        // 释放播放器资源
        public static void ReleasePlayer()
        {
            if (_globalPlayer != null)
            {
                try
                {
                    _globalPlayer.Stoptrack(true);
                }
                catch (Exception ex)
                {
                    Log.Error($"停止播放器时出错: {ex}");
                }
                finally
                {
                    _globalPlayer = null;
                }
            }
        }

        public void RoundEnded(RoundEndedEventArgs ev)
        {
            StringBuilder sb = new StringBuilder();
            Player mvpPlayer = null;

            if (YuChunMVPMusic.Plugin.Instance.Config.IsEnableMVP)
            {
                if (MvpEvent.PlayerKillCount.Any())
                {
                    mvpPlayer = MvpEvent.PlayerKillCount
                        .OrderByDescending(kv => kv.Value)
                        .First().Key;
                }

                if (mvpPlayer != null && MvpEvent.PlayerKillCount.ContainsKey(mvpPlayer))
                {
                    int kills = MvpEvent.PlayerKillCount[mvpPlayer];
                    sb.AppendLine($"本局<color=#FC0000>MVP</color>是 {mvpPlayer.Nickname}" +
                                 $" 本局共击杀<color=#FF1493>{kills}人！</color>");

                    Log.Info($"MVP是{mvpPlayer.Nickname}，本局共击杀{kills}人");

                    Timing.CallDelayed(0.5f, () =>
                    {
                        string musicInfo = TryPlayMusic(mvpPlayer);
                        Map.ClearBroadcasts();

                        if (!string.IsNullOrEmpty(musicInfo))
                        {
                            sb.AppendLine($"正在播放: <color=#00FFFF>{musicInfo}</color>");
                            Log.Info($"MVP音乐播放成功: {musicInfo}");
                        }
                        else
                        {
                            Log.Info("MVP音乐播放失败或未配置");
                        }

                        if (YuChunMVPMusic.Plugin.Instance.Config.IsEnableRoundEndedFF)
                        {
                            Server.FriendlyFire = true;
                            sb.AppendLine("友伤已开，尽情的背刺吧！");
                        }

                        Map.Broadcast(30, sb.ToString(), 0, false);

                        // 延长到20秒后释放播放器
                        Timing.CallDelayed(20f, () => ReleasePlayer());
                    });
                }
                else
                {

                    Log.Info("本局没有符合条件的MVP或MVP数据无效");
                    sb.AppendLine("本局<color=#FC0000>MVP</color>：虚位以待");

                    if (YuChunMVPMusic.Plugin.Instance.Config.IsEnableRoundEndedFF)
                    {
                        Server.FriendlyFire = true;
                        sb.AppendLine("友伤已开，尽情的背刺吧！");
                    }

                    Map.ClearBroadcasts();
                    Map.Broadcast(30, sb.ToString(), 0, false);
                }
            }
            else if (YuChunMVPMusic.Plugin.Instance.Config.IsEnableRoundEndedFF)
            {
                Server.FriendlyFire = true;
                sb.AppendLine("友伤已开，尽情的背刺吧！");
                Map.ClearBroadcasts();
                Map.Broadcast(30, sb.ToString(), 0, false);
            }
            else
            {
                Map.ClearBroadcasts();
            }
        }
    }
}
