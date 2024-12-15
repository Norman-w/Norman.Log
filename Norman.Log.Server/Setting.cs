using Newtonsoft.Json;
using Norman.Log.Config;
using Norman.Log.Server.Core;

namespace Norman.Log.Server;

/// <summary>
/// 应用程序主设置,也就是Server的设置
/// </summary>
internal class Setting : ICommonConfig<Setting>
{
	/// <summary>
	/// 通过grpc接收日志的端口
	/// </summary>
	public int GrpcPort { get; set; }

	/// <summary>
	/// 通过grpc-web,http,websocket接收日志的端口
	/// </summary>
	public int HttpAndWsAndGrpcWebPort { get; set; }

	/// <summary>
	/// 控制面板设置
	/// </summary>
	public ControlPanelSetting ControlPanel { get; set; } = new();

	/// <summary>
	/// 日志接收器设置,服务器要把日志分发给哪些客户端的相关设置
	/// </summary>
	public ReceiverSetting Receiver { get; set; } = new();

	/// <summary>
	/// 获取默认设置
	/// </summary>
	/// <returns></returns>
	public Setting GetDefault()
	{
		return new Setting
		{
			GrpcPort = 5011,
			HttpAndWsAndGrpcWebPort = 5012,
			ControlPanel = new ControlPanelSetting
			{
				Port = 8080,
				ConnectionType = ConnectionTypeEnum.WebSocket
			},
			Receiver = new ReceiverSetting
			{
				SupportConnectionTypes = new List<ConnectionTypeEnum>
				{
					ConnectionTypeEnum.Internal,
					ConnectionTypeEnum.WebSocket
				}
			}
		};
	}

	/// <summary>
	/// 从json字符串中填充数据
	/// </summary>
	/// <param name="json"></param>
	public void Populate(string json)
	{
		JsonConvert.PopulateObject(json, this);
	}

	/// <summary>
	/// 从文件中读取设置
	/// </summary>
	/// <param name="path"></param>
	/// <param name="tryCreateIfNotExist"></param>
	public void FromFile(string path, bool tryCreateIfNotExist = true)
	{
		if (tryCreateIfNotExist && !File.Exists(path))
		{
			Populate(JsonConvert.SerializeObject(GetDefault()));
			return;
		}

		var json = File.ReadAllText(path);
		JsonConvert.PopulateObject(json, this);
	}

	/// <summary>
	/// 控制面板设置
	/// </summary>
	internal class ControlPanelSetting
	{
		public int Port { get; set; }
		public ConnectionTypeEnum ConnectionType { get; set; }
	}

	/// <summary>
	/// 日志接收器设置
	/// </summary>
	internal class ReceiverSetting
	{
		public List<ConnectionTypeEnum> SupportConnectionTypes { get; set; } = new();
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