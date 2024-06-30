/*

日志监控器设置.
日志监控器是本套日志系统的三大件中的一个，主要负责日志的监控、分析、统计等功能。
日志监控器可以是多端的,比如
用Flutter开发的多平台客户端,
用WinForm开发的科技感日志显示器,
使用C#本身的Console输出,
使用Web网页通过WebSocket查看.

本设置为C#本身的Console输出时的设置.
在C#中还可以定义一个为WinForm的设置,这个根据到时候的具体情况写即可.
客户端和网页有对应的他们的设置,设置在对应的平台使用对应的语言编写即可.
 */

namespace Norman.Log.Config
{
	/// <summary>
	/// 日志监控器设置,基于C#的Console输出
	/// </summary>
	public class LogConsoleMonitorConfig
	{
		
	}
}