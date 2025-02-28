namespace Norman.Log.Config
{
	/// <summary>
	/// 记录到日志服务器的设置
	/// </summary>
	public class LogToLogServerConfig
	{
		public bool OnOff { get; set; }

		public enum SendToLogServerProtocolEnum
		{
			WebSocket = 1,
			Grpc = 2,
			Http = 3,
		}

		public SendToLogServerProtocolEnum Protocol { get; set; }

		public class LogServerWebSocketSetting
		{
			public string Url { get; set; }
		}

		public class LogServerGrpcSetting
		{
			public string Url { get; set; }
		}

		public class LogServerHttpSetting
		{
			public string Url { get; set; }
		}

		public LogServerWebSocketSetting WebSocket { get; set; }
		public LogServerGrpcSetting Grpc { get; set; }
		public LogServerHttpSetting Http { get; set; }

		public static LogToLogServerConfig Default { get; } = new LogToLogServerConfig
		{
			OnOff = false,
			Protocol = LogToLogServerConfig.SendToLogServerProtocolEnum.WebSocket,
			WebSocket = new LogToLogServerConfig.LogServerWebSocketSetting
			{
				Url = "ws://localhost:5000/ws",
			},
			Grpc = new LogToLogServerConfig.LogServerGrpcSetting
			{
				Url = "localhost:5000",
			},
			Http = new LogToLogServerConfig.LogServerHttpSetting
			{
				Url = "http://localhost:5000",
			},
		};
	}
}