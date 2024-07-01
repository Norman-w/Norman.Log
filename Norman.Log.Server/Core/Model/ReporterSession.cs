namespace Norman.Log.Server.Core.Model;

/// <summary>
/// 日志报告客户端会话,也就是日志的来源
/// </summary>
public class ReporterSession
{
	public ConnectionTypeEnum ConnectionType { get; set; }
}