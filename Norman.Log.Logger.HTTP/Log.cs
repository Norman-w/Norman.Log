using System;
using System.Collections.Generic;

namespace Norman.Log.Logger.HTTP
{
	public class LogContext
	{
		/// <summary>
		/// 角色
		/// </summary>
		public object Role { get; set; }
		/// <summary>
		/// Site/Portal/Platform
		/// </summary>
		public object Site { get; set; }
		/// <summary>
		/// 用户
		/// </summary>
		public object User { get; set; }
		/// <summary>
		/// 客户端
		/// </summary>
		public object Client { get; set; }
		/// <summary>
		/// 请求
		/// </summary>
		public object Request { get; set; }
		/// <summary>
		/// 响应
		/// </summary>
		public object Response { get; set; }
		/// <summary>
		/// 其他的上下文信息
		/// </summary>
		public List<object> Others { get; set; }
			
		/// <summary>
		/// 转换为网络传输模型
		/// </summary>
		/// <returns></returns>
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
	/// <summary>
	/// 日志对象
	/// </summary>
	public class Log
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		/// <summary>
		/// 使用日志记录器名称初始化一个日志对象.其他属性通过对象的Setter设置
		/// </summary>
		/// <param name="loggerName"></param>
		public Log(string loggerName)
		{
			LoggerName = loggerName;
		}
		/// <summary>
		/// 日志记录器名称
		/// </summary>
		public string LoggerName { get; set; }
		/// <summary>
		/// 日志类型,如Info,Error,Warn等
		/// </summary>
		public LogTypeEnum Type { get; set; }
		/// <summary>
		/// 日志所在的层,如系统层,应用层,服务层,IO层等
		/// </summary>
		public LogLayerEnum Layer { get; set; }
		/// <summary>
		/// 概要描述文本
		/// </summary>
		public string Summary { get; set; }
		/// <summary>
		/// 详情描述文本
		/// </summary>
		public string Detail { get; set; }
		/// <summary>
		/// 所在模块文本,如用户模块,认证模块,网关模块等等,自定义的字符串.
		/// </summary>
		public string Module { get; set; }
		
		/// <summary>
		/// 日志的创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 日志的山下文信息,可选的
		/// </summary>
		public LogContext Context { get; set; }
		
		
		/// <summary>
		/// 转换为网络传输模型.
		/// </summary>
		/// <returns></returns>
		public LogRecord4Net ToLogRecord4Net()
		{
			return new LogRecord4Net
			{
				Id = Id.ToString(),
				LoggerName = LoggerName,
				Type = Type,
				Layer = Layer,
				Summary = Summary,
				Detail = Detail,
				Module = Module,
				LogContext = Context?.ToLogRecordContext4Net(),
				CreateTime = CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
			};
		}
	}
}