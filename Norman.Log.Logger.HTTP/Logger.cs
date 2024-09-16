using System;

namespace Norman.Log.Logger.HTTP
{
	/// <summary>
	/// 日志记录器,通过Logger.UpdateConfig方法配置URL和名称,然后调用Logger.Write方法写日志
	/// 日志将会通过HTTP POST请求发送到指定的URL.
	/// 如果不使用默认的Logger实例,可以自定义NamedLogger实例并调用Write方法
	/// </summary>
	public static class Logger
	{
		#region Private
		
		#region 内部字段,属性

		/// <summary>
		/// 日志记录器的URL
		/// </summary>
		private static string _reportLogUrl;
		/// <summary>
		/// 日志记录器的名称
		/// </summary>
		private static string _loggerName;

		#endregion

		#region 私有的带有默认LoggerName和ReportLogUrl的Logger实例定义和初始化

		private static NamedLogger _staticDefaultLoggerInstance;
		/// <summary>
		/// 创建默认的Logger实例,用于静态方法,如果没有配置URL和名称,则抛出异常
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private static NamedLogger CreateStaticDefaultLoggerInstance()
		{
			if (!string.IsNullOrEmpty(_loggerName) && !string.IsNullOrEmpty(_reportLogUrl))
				return new NamedLogger(_loggerName, _reportLogUrl);
			var message = $"请先调用{nameof(Logger)}.{nameof(UpdateConfig)}方法配置日志记录器的URL和名称,或者使用你自定义的NamedLogger实例";
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
			throw new SystemException(message);
		}
		/// <summary>
		/// 命名的Logger的实例
		/// </summary>
		private static NamedLogger StaticDefaultLoggerInstance => _staticDefaultLoggerInstance ?? (_staticDefaultLoggerInstance = CreateStaticDefaultLoggerInstance());
		

		#endregion

		#endregion
		
		#region Public

		#region 对外公共方法,更新配置
		
		/// <summary>
		/// 更新配置
		/// </summary>
		/// <param name="reportLogUrl">日志记录器的URL</param>
		/// <param name="loggerName">日志记录器的名称</param>
		public static void UpdateConfig(string reportLogUrl, string loggerName)
		{
			_reportLogUrl = reportLogUrl;
			_loggerName = loggerName;
		}

		#endregion

		#region 对外公共方法,记录日志

		/// <summary>
		/// 记录/写日志,传入Log对象
		/// </summary>
		/// <param name="log"></param>
		public static bool Write(Log log)
		{
			try
			{
				return StaticDefaultLoggerInstance.Write(log);
			}
			catch (Exception e)
			{
				if (e is SystemException)
					throw;
				var message = $"写日志失败,错误信息:{e.Message}";
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(message);
				Console.ResetColor();
			}

			return false;
		}
		/// <summary>
		/// 记录/写日志,传入参数,生成Log对象,最终会调用Write(Log log)方法
		/// </summary>
		/// <param name="logType">日志类型</param>
		/// <param name="logLayer">日志层级</param>
		/// <param name="moduleName">模块名称</param>
		/// <param name="summary">摘要</param>
		/// <param name="detail">详情</param>
		/// <param name="context">上下文</param>
		public static bool Write(LogTypeEnum logType, LogLayerEnum logLayer, string moduleName, string summary, string detail,
			LogContext context = null)
		{
			return Write(new Log(_loggerName)
			{
				Type = logType,
				Layer = logLayer,
				Summary = summary,
				Detail = detail,
				Module = moduleName,
				Context = context,
				CreateTime = DateTime.Now
			});
		}

		#endregion
		
		#endregion
	}
}