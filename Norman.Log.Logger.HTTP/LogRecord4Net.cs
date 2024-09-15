using System.Collections.Generic;
using Newtonsoft.Json;

namespace Norman.Log.Logger.HTTP
{
	public class LogRecord4Net
	{ 
		public string LoggerName { get; set; }
		public LogTypeEnum Type { get; set; }
		public LogLayerEnum Layer { get; set; }
		public string Summary { get; set; }
		public string Detail { get; set; }
		public string Module { get; set; }
		public LogRecordContext4Net LogContext { get; set; }
		
		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
	public class LogRecordContext4Net
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
	}
}