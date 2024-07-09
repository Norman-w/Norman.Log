/*
 
 应用程序的全局可访问对象

*/

using Norman.Log.Component.DatabaseWriter;
using Norman.Log.Component.FileWriter;
using Norman.Log.Config;

namespace Norman.Log.Logger
{
	/// <summary>
	/// 应用程序的全局可访问对象,内部包含了各种静态实例.
	/// 与各种类的单一实例的访问方式作用相同,但是更加简洁.
	/// </summary>
	public static class App
	{
		private static LogFileWriter _logFileWriter;
		/// <summary>
		/// 日志文件写入器
		/// </summary>
		public static LogFileWriter LogFileWriter => _logFileWriter??(_logFileWriter = new LogFileWriter(AppConfig.LoggerConfig.LogToFile));

		private static LogDatabaseWriter _logDatabaseWriter;
		/// <summary>
		/// 日志数据库写入器
		/// </summary>
		public static LogDatabaseWriter LogDatabaseWriter => _logDatabaseWriter??(_logDatabaseWriter = new LogDatabaseWriter(AppConfig.LoggerConfig.LogToDatabase));
	}
}