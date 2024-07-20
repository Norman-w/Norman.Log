using Newtonsoft.Json;

namespace Norman.Log.Model
{
	public class LogRecord4Net
	{
		public string CreateTime { get; set; }
		public string Id { get; set; }
		/// <summary>
		/// 日志记录器的名称,比如 AuthLogger, SessionLogger, RequestLogger等,默认的是DefaultLogger
		/// </summary>
		public string LoggerName { get; set; } = Constant.DefaultLoggerName;
		/// <summary>
		/// 日志类型,比如:错误,警告,信息等
		/// </summary>
		public uint Type { get; set; }
		/// <summary>
		/// 日志发生在哪一层,比如系统层,外设层,业务层等
		/// </summary>
		public uint Layer { get; set; }
		/// <summary>
		/// 日志发生在哪个模块,比如xxxController, xxxManager, xxxHandler等
		/// </summary>
		public string Module { get; set; }
		/// <summary>
		/// 日志发生时携带的描述消息,最好是一句话可以看明白的.
		/// </summary>
		public string Summary { get; set; }
		/// <summary>
		/// 如果一句话描述不清楚,可以在这里详细描述
		/// </summary>
		public string Detail { get; set; }

		/// <summary>
		/// 日志的上下文信息,比如 角色(类型),站点,用户(name, id, session),客户端信息,请求信息,响应信息等
		/// </summary>
		public object LogContext { get; set; }
		
		public static LogRecord4Net FromLog(Log log)
		{
			return new LogRecord4Net
			{
				CreateTime = log.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
				Id = log.Id.ToString(),
				LoggerName = log.LoggerName,
				Type = log.Type.Value,
				Layer = log.Layer.Value,
				Module = log.Module,
				Summary = log.Summary,
				Detail = log.Detail,
				LogContext = log.LogContext
			};
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}