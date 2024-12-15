using Newtonsoft.Json;
using Norman.Log.Config;
using Norman.Log.Server.Core;

namespace Norman.Log.Server;

/// <summary>
/// 应用程序主设置,也就是Server的设置
/// </summary>
internal class Setting : ICommonConfig<Setting>
{
	public int GrpcPort { get; set; }

	public int GrpcWebPort { get; set; }

	public ControlPanelSetting ControlPanel { get; set; } = new();

	public ReceiverSetting Receiver { get; set; } = new();

	public Setting GetDefault()
	{
		return new Setting
		{
			GrpcPort = 5011,
			GrpcWebPort = 5012,
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

	public void Populate(string json)
	{
		JsonConvert.PopulateObject(json, this);
	}

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