# YuChunMVPMusic  SCP： Secret Laboratory （Exiled） 的 MVP 插件

这是一个为 SCP： Secret Laboratory 服务器设计的 Exiled 插件，用于实现 MVP （Most Valuable Player） 系统

一个重置于SCPSL-ChunYu-MVP的同类型SCP:SLMVP音乐播放插件

从 下载最新版本的插件 文件，并将其放置到服务器的 文件夹中.dllExiled/Plugins

安装AudioApi.dll依赖把这个放在依赖项里面Exiled/Plugins/dependencies

启动服务器，插件会自动生成配置文件

根据需要修改配置文件

插件的配置文件位于 'EXILED\Configs\Plugins\

以下是主要的配置选项：

IsEnabled: 是否启用整个插件 默认为开启

Debug: 是否启用调试模式 默认为关闭

IsEnableMVP： 是否启用 MVP 功能 默认为开启

IsEnableRoundEndedFF: 是否在回合结束时启用友伤 默认为开启

示例: m_v_p_music_path： “@steam”： “<绝对音乐文件路径>”

请确保音乐文件路径是服务器可以访问到的绝对路径，并且音频文件是单声道、48000Hz 频率的 OGG 格式

作者：chunyu（椿雨社区），保障民生（聿日箋秋服务器）
