using Norman.Log.Server.Core;

namespace Norman.Log.Server;

/// <summary>
/// 应用程序主设置,也就是Server的设置
/// </summary>
internal class Setting
{
	public int GrpcPort { get; set; } = 5011;
	
	public int GrpcWebPort { get; set; } = 5012;
	/// <summary>
	/// 控制面板设置
	/// </summary>
	internal class ControlPanelSetting
	{
		public int Port { get; set; } = 8080;
		public ConnectionTypeEnum ConnectionType { get; set; } = ConnectionTypeEnum.WebSocket;
	}

	/// <summary>
	/// 日志接收器设置
	/// </summary>
	internal class ReceiverSetting
	{
		public List<ConnectionTypeEnum> SupportConnectionTypes { get; set; } = new()
		{
			ConnectionTypeEnum.Internal, 
			ConnectionTypeEnum.WebSocket
		};
	}
	
	/// <summary>
	/// 日志广播器设置
	/// </summary>
	internal class BroadcasterSetting
	{
		
	}
	
	/// <summary>
	/// 日志池设置
	/// </summary>
	internal class LogPoolSetting
	{
		
	}
}