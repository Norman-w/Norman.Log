using System.Collections.Generic;

namespace Norman.Log.Logger.HTTP
{
	public class Log
	{
		public Log(string loggerName)
		{
			LoggerName = loggerName;
		}
		public string LoggerName { get; set; }
		public LogTypeEnum Type { get; set; }
		public LogLayerEnum Layer { get; set; }
		public string Summary { get; set; }
		public string Detail { get; set; }
		public string Module { get; set; }
		public Context LogContext { get; set; }
		public class Context
		{
			public object Role { get; set; }
			/// <summary>
			/// Site/Portal/Platform
			/// </summary>
			public object Site { get; set; }
			public object User { get; set; }
			public object Client { get; set; }
			public object Request { get; set; }
			public object Response { get; set; }
			/// <summary>
			/// 其他的上下文信息
			/// </summary>
			public List<object> Others { get; set; }
			
			public LogRecordContext4Net ToLogRecordContext4Net()
			{
				return new LogRecordContext4Net
				{
					Role = Role,
					Site = Site,
					User = User,
					Client = Client,
					Request = Request,
					Response = Response,
					Others = Others
				};
			}
		}
		
		public LogRecord4Net ToLogRecord4Net()
		{
			return new LogRecord4Net
			{
				LoggerName = LoggerName,
				Type = Type,
				Layer = Layer,
				Summary = Summary,
				Detail = Detail,
				Module = Module,
				LogContext = LogContext.ToLogRecordContext4Net()
			};
		}
	}
}