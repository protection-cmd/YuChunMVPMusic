using AudioApi; // 添加 VoicePlayerBase 的命名空间
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
using YuChunMVPMusic.MVPSystem; // 添加语音频道枚举
using YuChunMVPMusic;

namespace YuRiJianQiuChunYu.MVPSystem
{
    public class MusicPlayer
    {
        // 全局音乐播放器实例
        private static VoicePlayerBase _globalPlayer;

        public void WaitingForPlayer()
        {
            if (_globalPlayer != null)
            {
                _globalPlayer.Stoptrack(true);
            }
        }

        public static string TryPlayMusic(Player p)
        {
            if (!YuChunMVPMusic.Plugin.Instance.Config.MVPMusicPath.ContainsKey(p.UserId))
            {
                Log.Info("玩家没有配置音乐路径");
                return null;
            }

            if (p == null)
            {
                Log.Info("玩家为空");
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

            // 获取或创建全局播放器
            if (_globalPlayer == null)
            {
                // 使用服务器控制台作为音频源
                Player host = Player.Get(Server.Host);
                if (host == null)
                {
                    Log.Error("无法获取服务器控制台玩家");
                    return null;
                }

                _globalPlayer = VoicePlayerBase.Get(host.ReferenceHub);
                _globalPlayer.BroadcastChannel = VoiceChatChannel.Intercom; // 使用全局频道
            }

            // 配置播放器
            _globalPlayer.Stoptrack(true); // 停止并清空当前播放
            _globalPlayer.Volume = 100f;   // 100% 音量
            _globalPlayer.Loop = false;     // 不循环
            _globalPlayer.Shuffle = false;  // 不随机

            // 设置广播目标为所有玩家
            _globalPlayer.BroadcastTo.Clear();
            foreach (Player player in Player.List)
            {
                _globalPlayer.BroadcastTo.Add(player.Id);
            }

            // 添加并播放音乐
            _globalPlayer.Enqueue(selectedMusic, -1);
            _globalPlayer.Play(0);

            Log.Info($"MVP音乐已播放: {musicName}");
            return musicName;
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
                        string musicName = TryPlayMusic(mvpPlayer);
                        Map.ClearBroadcasts();

                        if (!string.IsNullOrEmpty(musicName))
                        {
                            sb.AppendLine($"正在播放MVP专属音乐: <color=#00FFFF>{musicName}</color>");
                            Log.Info($"MVP音乐播放成功: {musicName}");
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
