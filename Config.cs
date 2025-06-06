using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using YamlDotNet.Serialization;

namespace YuChunMVPMusic
{
    // Token: 0x02000002 RID: 2
    public class Config : IConfig
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
        // (set) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
        [Description("是否启用MVP插件")]
        public bool IsEnabled { get; set; } = true;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002059 File Offset: 0x00000259
        // (set) Token: 0x06000004 RID: 4 RVA: 0x00002061 File Offset: 0x00000261
        public bool Debug { get; set; } = false;

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000005 RID: 5 RVA: 0x0000206A File Offset: 0x0000026A
        // (set) Token: 0x06000006 RID: 6 RVA: 0x00002072 File Offset: 0x00000272
        public bool IsEnableMVP { get; set; } = true;

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000007 RID: 7 RVA: 0x0000207B File Offset: 0x0000027B
        // (set) Token: 0x06000008 RID: 8 RVA: 0x00002083 File Offset: 0x00000283
        [Description("对局结束友伤")]
        public bool IsEnableRoundEndedFF { get; set; } = true;

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000009 RID: 9 RVA: 0x0000208C File Offset: 0x0000028C
        // (set) Token: 0x0600000A RID: 10 RVA: 0x00002094 File Offset: 0x00000294
        [Description("MVP配置 - 每个用户最多3个音乐路径")]
        [YamlMember(Alias = "mvp_music_path")]
        public Dictionary<string, List<string>> MVPMusicPath { get; set; }

        // Token: 0x0600000B RID: 11 RVA: 0x000020A0 File Offset: 0x000002A0
        public Config()
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            string text = "someone@steam";
            List<string> list = new List<string>();
            list.Add("音乐路径1");
            list.Add("音乐路径2");
            list.Add("音乐路径3");
            dictionary.Add(text, list);
            this.MVPMusicPath = dictionary;
        }
    }
}