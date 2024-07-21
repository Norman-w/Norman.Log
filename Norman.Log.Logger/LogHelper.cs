using System;
using System.Linq;
using Norman.Log.Model;

namespace Norman.Log.Logger
{
	/// <summary>
	/// 日志记录器的帮助类,默认的使用名为"Default"的日志记录器
	/// </summary>
	public static class LogHelper
	{
		private static NamedLogger _defaultNamedLogger;

		/// <summary>
		/// 默认的日志记录器
		/// </summary>
		private static NamedLogger DefaultNamedLogger =>
			_defaultNamedLogger ?? (_defaultNamedLogger = new NamedLogger("Default"));

		/// <summary>
		/// 使用默认的日志记录器记录日志
		/// </summary>
		/// <param name="log"></param>
		public static void Write(Model.Log log)
		{
			DefaultNamedLogger.Write(log);
		}

		/// <summary>
		/// 使用默认的日志记录器记录日志
		/// </summary>
		/// <param name="logType"></param>
		/// <param name="logLayer"></param>
		/// <param name="moduleName"></param>
		/// <param name="summary"></param>
		/// <param name="detail"></param>
		/// <param name="context"></param>
		public static void Write(LogType logType, LogLayer logLayer, string moduleName, string summary, string detail,
			Model.Log.Context context = null)
			=> DefaultNamedLogger.Write(logType, logLayer, moduleName, summary, detail, context);
		
		/// <summary>
		/// 调用一次强制写入日志文件,即便是没有到达缓存阈值或者是时间间隔
		/// </summary>
		public static void Flush()
		{
			try
			{
				App.LogFileWriter.Flush();
			}
			catch (Exception e)
			{
				Console.WriteLine($"强制写入日志文件失败:{e}");
			}

			try
			{
				App.LogDatabaseWriter.Flush();
			}
			catch (Exception e)
			{
				Console.WriteLine($"强制写入日志数据库失败:{e}");
			}
		}

		#region 模拟Orion的调用方式

		/// <summary>
		/// 记录一般信息
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public static void InfoFormat(string message, params object[] args)
		{
			DefaultNamedLogger.Write(
				LogType.Info,
				LogLayer.Business,
				"Default",
				message,
				"日志的详细描述内容",
				new Model.Log.Context
				{
					Others = new object[] { args }.ToList()
				});
		}

		/// <summary>
		/// 记录系统错误
		/// </summary>
		/// <param name="errorMessage"></param>
		/// <param name="exception"></param>
		public static void SystemError(string errorMessage, Exception exception)
		{
			DefaultNamedLogger.Write(LogType.Error, LogLayer.System, null, errorMessage, exception.ToString());
		}

		#endregion
	}
}