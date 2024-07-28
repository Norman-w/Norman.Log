using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Norman.Log.Model
{
	/// <summary>
	/// 日志
	/// </summary>
	public class Log
	{
		public Log(string loggerName)
		{
			if (string.IsNullOrWhiteSpace(loggerName))
			{
				throw new ArgumentNullException(nameof(loggerName));
			}
			LoggerName = loggerName;
		}
		public DateTime CreateTime = DateTime.Now;
		public Guid Id = Guid.NewGuid();
		/// <summary>
		/// 日志记录器的名称,比如 AuthLogger, SessionLogger, RequestLogger等,默认的是DefaultLogger
		/// </summary>
		public string LoggerName { get; private set; }
		/// <summary>
		/// 日志类型,比如:错误,警告,信息等
		/// </summary>
		public LogType Type { get; set; }
		/// <summary>
		/// 日志发生在哪一层,比如系统层,外设层,业务层等
		/// </summary>
		public LogLayer Layer { get; set; }
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
		/// 日志的上下文信息
		/// 比如 角色(类型),站点,用户(name, id, session),客户端信息,请求信息,响应信息等
		/// </summary>
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
		}

		/// <summary>
		/// 日志的上下文信息,比如 角色(类型),站点,用户(name, id, session),客户端信息,请求信息,响应信息等
		/// </summary>
		public Context LogContext { get; set; }

		/// <summary>
		/// 定义一个字段,用于缓存ToString的结果,防止多次调用ToString方法造成的性能损耗
		/// </summary>
		private string _string;

		public override string ToString()
		{
			//如果已经有缓存的字符串,则直接返回
			if(_string != null)
				return _string;
			//如果没有缓存的字符串,则生成一个新的字符串
			
			//尝试将上下文信息转换为json,如果错误,则标记为序列化失败
			var contextJson = "序列化失败";
			try
			{
 				contextJson = JsonConvert.SerializeObject(LogContext);
			}
			catch (Exception e)
			{
				Console.WriteLine($"在序列化日志上下文信息时发生错误:{e}");
			}
			_string = $"{CreateTime}\r\n类型:{Type.Name},层级:{Layer.Name},模块:{Module},摘要:{Summary},详情:{Detail},上下文:{contextJson}";
			return _string;
		}
		
		private byte[] _bytes;
		/// <summary>
		/// 将日志转换为字节数组,用于写入文件或者网络传输,但只会转换一次,之后会直接返回缓存的字节数组
		/// 这样可以避免多次转换造成的性能损耗
		/// </summary>
		/// <returns></returns>
		public byte[] ToBytes()
		{
			if(_bytes != null)
				return _bytes;
			_bytes = System.Text.Encoding.UTF8.GetBytes(ToString());
			return _bytes;
		}

		/// <summary>
		/// 从LogRecord4Net转换为Log
		/// </summary>
		/// <param name="logRecord4Net"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static Log FromLogRecord4Net(LogRecord4Net logRecord4Net)
		{
			if (logRecord4Net == null)
			{
				throw new ArgumentNullException(nameof(logRecord4Net));
			}

			var context = new Context();
			var contextJsonFromLogRecord4Net = logRecord4Net.LogContext == null? "": JsonConvert.SerializeObject(logRecord4Net.LogContext);
			if (!string.IsNullOrWhiteSpace(contextJsonFromLogRecord4Net))
			{
				JsonConvert.PopulateObject(contextJsonFromLogRecord4Net, context);
			}

			var log = new Log(logRecord4Net.LoggerName)
			{
				Type = LogType.FromValue(logRecord4Net.Type),
				Layer = LogLayer.FromValue(logRecord4Net.Layer),
				Module = logRecord4Net.Module,
				Summary = logRecord4Net.Summary,
				Detail = logRecord4Net.Detail,
				LogContext = context, LoggerName = logRecord4Net.LoggerName,
				CreateTime = DateTime.Parse(logRecord4Net.CreateTime), Id = Guid.Parse(logRecord4Net.Id)
			};
			return log;
		}
	}
}