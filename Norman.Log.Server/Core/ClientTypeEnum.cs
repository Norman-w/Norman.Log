namespace Norman.Log.Server.Core;

public enum ClientTypeEnum
{
	/// <summary>
	/// 未知
	/// </summary>
	Unknown = 0,
	/// <summary>
	/// 报告者,日志都从这里来
	/// </summary>
	Reporter = 1,
	/// <summary>
	/// 接收者,日志都到这里去
	/// </summary>
	Receiver = 2,
	/// <summary>
	/// 写入者,日志写入到文件或者是数据库
	/// </summary>
	Writer = 3,
}