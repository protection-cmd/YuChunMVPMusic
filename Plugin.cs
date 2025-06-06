using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using System;
using System.Net.Http;
using YuChunMVPMusic.MVPSystem;
using YuRiJianQiuChunYu.MVPSystem;

namespace YuChunMVPMusic
{
    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "聿日箋秋&ChunYu椿雨";
        public override string Name { get; } = "聿日箋秋&ChunYu椿雨MVP";
        public static HttpClient HttpClient { get; private set; }

        
        public override void OnEnabled()
        {
            base.OnEnabled();
            DisplayYCMVPLogo();
            Log.Info("聿日箋秋&ChunYu:MVP插件开启");
            Plugin.Instance = this;
            Plugin.Singleton = this;
            this.musicPlayer = new MusicPlayer();
            this.mvpEvent = new MvpEvent();
            Exiled.Events.Handlers.Player.Verified += new CustomEventHandler<VerifiedEventArgs>(this.mvpEvent.Verified);
            Exiled.Events.Handlers.Player.Dying += new CustomEventHandler<DyingEventArgs>(this.mvpEvent.Dying);
            Exiled.Events.Handlers.Server.WaitingForPlayers += new CustomEventHandler(this.mvpEvent.WaitingForPlayer);
            Exiled.Events.Handlers.Server.RoundEnded += new CustomEventHandler<RoundEndedEventArgs>(this.musicPlayer.RoundEnded);
            Exiled.Events.Handlers.Server.WaitingForPlayers += new CustomEventHandler(this.musicPlayer.WaitingForPlayer);
        }
        private void DisplayYCMVPLogo()
        {
            Log.Info("\n" +
                "  ██╗   ██╗  ██████╗ ███╗   ███╗██████╗ ██████╗ \n" +
                "   ╚██╗ ██╔╝ ██╔════╝ ████╗ ████║██╔══██╗██╔══██╗\n" +
                "    ╚████╔╝  ██║      ██╔████╔██║██████╔╝██████╔╝\n" +
                "     ╚██╔╝   ██║      ██║╚██╔╝██║██╔═══╝ ██╔═══╝ \n" +
                "      ██║    ╚██████╗ ██║ ╚═╝ ██║██║     ██║     \n" +
                "      ╚═╝     ╚═════╝ ╚═╝     ╚═╝╚═╝     ╚═╝     \n" +
                "\n" +
                "  ╔══════════════════════════════╗\n" +
                "  ║       YCMVP 音乐系统         ║\n" +
                "  ║       聿日箋秋 & ChunYu      ║\n" +
                "  ║       版本:1.2.0             ║\n" +
                "  ╚══════════════════════════════╝\n");
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Player.Verified -= new CustomEventHandler<VerifiedEventArgs>(this.mvpEvent.Verified);
            Exiled.Events.Handlers.Player.Dying -= new CustomEventHandler<DyingEventArgs>(this.mvpEvent.Dying);
            Exiled.Events.Handlers.Server.WaitingForPlayers -= new CustomEventHandler(this.mvpEvent.WaitingForPlayer);
            Exiled.Events.Handlers.Server.RoundEnded -= new CustomEventHandler<RoundEndedEventArgs>(this.musicPlayer.RoundEnded);
            Exiled.Events.Handlers.Server.WaitingForPlayers -= new CustomEventHandler(this.musicPlayer.WaitingForPlayer);
            Plugin.Instance = null;
            Plugin.Singleton = null;
            this.mvpEvent = null;
            this.musicPlayer = null;
        }
        public static Plugin Instance;

        public static Plugin Singleton;

        private MusicPlayer musicPlayer;

        private MvpEvent mvpEvent;
    }
}

