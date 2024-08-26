/*


 日志到文件的写入器.

*/

/*

 一个防止写文件冲突的方案但是暂时不考虑的:

	保证是单例模式,也就是说只能有一个实例,如果有多个实例则会出现文件写入冲突的问题.
   初始化的时候检测在日志根目录下有没有一个Instance文件,如果没有的话创建一个,创建完了以后要进程独占的使用.
   监测这个文件能否被写入,如果不能写入则抛出异常.如果能写入则写入相关的LogFileWriter的信息,包括但不限于初始化时间,初始化的用途,用来干啥的之类的
   说明有其他的实例在使用.抛出异常的时候会显示出是哪个实例在使用,给出Pid和进程名,实例时间等信息,也就是进程本身的信息和Instance中的信息

 不考虑的原因是,当一个logger创建文件的时候,可以检测同名的文件是不是存在,如果存在则可以添加冲突标记来标识.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Norman.Log.Config;

namespace Norman.Log.Component.FileWriter
{
	#region 类型定义(回调和结构体)

	#region 回调事件,当新文件夹或者新文件创建时候触发

	/// <summary>
	/// 新文件夹创建时候触发
	/// </summary>
	public delegate void NewFolderCreatedEventHandler(string folderName);

	/// <summary>
	/// 新文件创建时候触发
	/// </summary>
	public delegate void NewFileCreatedEventHandler(string fileName);

	#endregion

	/// <summary>
	/// 待写入的日志缓冲池信息,应按照文件夹名和文件名索引
	/// </summary>
	internal partial class LogsPoolInfo
	{
		public string LoggerName { get; set; }
		/// <summary>
		/// 文件夹名
		/// </summary>
		public string FolderName { get; set; }
		
		/// <summary>
		/// 追加分段,如果有的话,默认从2开始
		/// </summary>
		public uint? ContinuePart { get; private set; }

		/// <summary>
		/// 待写入的日志
		/// </summary>
		private List<Model.Log> Logs { get; set; }
		
		/// <summary>
		/// 日支池的创建时间
		/// </summary>
		public DateTime CreateTime { get; } = DateTime.Now;
		
		/// <summary>
		/// 该日支池自创建以来共记录过多少条日志
		/// </summary>
		public uint SincePoolCreatedLogsCount { get; set; }

		/// <summary>
		/// 最后一次写入时间
		/// </summary>
		public DateTime LastWriteTime { get; set; }

		/// <summary>
		/// 文件的当前大小
		/// </summary>
		private ulong FileSizeBytes { get; set; }
	}

	internal partial class LogsPoolInfo
	{
		/// <summary>
		/// 添加日志到缓冲池
		/// 如果需要分段则返回true,这时候应当写入文件
		/// </summary>
		/// <param name="log"></param>
		/// <param name="maxSizeKbPerFile"></param>
		/// <returns></returns>
		public bool Push(Model.Log log, uint maxSizeKbPerFile)
		{
			if (Logs == null)
				Logs = new List<Model.Log>();
			FileSizeBytes += (uint)log.ToBytes().Length;
			//默认最少需要1段,也就是一个默认分段.
			var howManyPartsNeed = (uint)(FileSizeBytes / 1024 / maxSizeKbPerFile) + 1;
			//如果需要额外分段则返回true,这时候应当写入文件
			//比如一个文件只能1k,现在是1025字节,则需要2个文件,这时候howManyPartsNeed应该是2,然后ContinuePart是null,所以返回true,ContinuePart变成2
			//如果是2049,则需要3个文件,此时ContinuePart是2,所以返回true,ContinuePart变成3
			if (howManyPartsNeed > 1 && ContinuePart == null || howManyPartsNeed > ContinuePart)
			{
				ContinuePart = howManyPartsNeed;
				return true;
			}

			Logs.Add(log);
			SincePoolCreatedLogsCount++;
			return false;
		}

		/// <summary>
		/// 获取缓冲池中的日志的字节数组
		/// </summary>
		/// <returns></returns>
		public byte[] GetAllLogsBytes()
		{
			var bytes = new List<byte>();
			foreach (var log in Logs)
			{
				bytes.AddRange(log.ToBytes());
				//添加换行符,连续换两行便于阅读,即两组\r\n
				bytes.AddRange(new byte[] {0x0D, 0x0A, 0x0D, 0x0A});
			}

			return bytes.ToArray();
		}

		/// <summary>
		/// 清空缓冲池中的日志
		/// </summary>
		public void ClearLogs()
		{
			Logs.Clear();
			//不用重置文件大小,因为文件大小是累加的,只有写入文件的时候才会重置
		}

		/// <summary>
		/// 获取缓冲池中是否有待写入的日志
		/// </summary>
		/// <returns></returns>
		public bool HasWaitingToWriteLogs()
		{
			return Logs.Any();
		}

		/// <summary>
		/// 获取缓冲池中待写入的日志数量
		/// </summary>
		/// <returns></returns>
		public uint GetWaitingToWriteLogsCount()
		{
			return (uint)Logs.Count;
		}
	}

	#endregion

	/// <summary>
	/// 文件写入器,用于将日志写入到文件
	/// </summary>
	public sealed class LogFileWriter : IDisposable
	{
		#region 对外

		#region 提供给外部注册的事件

		public event NewFolderCreatedEventHandler NewFolderCreated;

		public event NewFileCreatedEventHandler NewFileCreated;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数,初始化时候传入配置文件,将会根据配置文件初始化文件夹和文件监视器
		/// </summary>
		/// <param name="config"></param>
		/// <param name="createRootPathIfNotExist"></param>
		/// <exception cref="ArgumentException"></exception>
		public LogFileWriter(LogToFileConfig config, bool createRootPathIfNotExist = true)
		{
			if (!UpdateConfig(config, out var errorMessage, createRootPathIfNotExist))
			{
				Console.WriteLine($"{nameof(LogFileWriter)}中捕获到错误:{errorMessage}");
			}

			//初始化文件夹和文件监视器
			_logFolderAndFileWatcher = new LogFolderAndFileWatcher(_config);
			// 初始化计时器
			_timer = new Timer(config.WriteToFileInterval.TotalMilliseconds);
			_timer.Elapsed += OnTimerElapsedWriteLogs;
			_timer.AutoReset = true;
			_timer.Enabled = true;
		}

		#endregion

		/// <summary>
		/// 将日志对象添加到待写入的日志缓冲中
		/// 如果缓冲中的日志条数达到最大值则写入文件
		/// </summary>
		/// <param name="log"></param>
		public void AddLogToWaitingToWriteQueue(Model.Log log)
		{
			Util.CalcLogFileAndFolderName(log.LoggerName, DateTime.Now, _config, out var fileName, out var folderName);
			var fileFullPathForIndex = Path.Combine(_config.RootPath, folderName, fileName);
			var hasThisFilePool = IndexedByFileFullPathLogsPoolInfos.ContainsKey(fileFullPathForIndex);
			var currentPool = hasThisFilePool
				? IndexedByFileFullPathLogsPoolInfos[fileFullPathForIndex]
				: new LogsPoolInfo
				{
					LoggerName = log.LoggerName,
					FolderName = folderName,
					LastWriteTime = DateTime.MinValue
				};
			//应当分段了,之前的日志要写入文件
			var needFlushToFile = currentPool.Push(log, _config.MaxSizeKbPerFile);
			//这里添加到字典一定要等到push之后,因为struct是值类型,如果push之前添加到字典,那么push之后的修改不会影响到字典中的值
			if (!hasThisFilePool)
			{
				IndexedByFileFullPathLogsPoolInfos.Add(fileFullPathForIndex, currentPool);
			}

			//追加log以后,文件的路径可能会有所改变,所以要重新获取一次
			var fileFullPathForWrite = GetFullPathForWrite(log.LoggerName, currentPool.ContinuePart);

			var currentWaitingToWriteLogsCount = currentPool.GetWaitingToWriteLogsCount();
			//检查条数,如果够了则写入
			//检查是否分段了需要写入文件
			if (
				_config.MaxLogCountInCache <= currentWaitingToWriteLogsCount
				|| needFlushToFile
			)
			{
				WriteLogsToFileAutoCreateFolderOrFileIfNeeded(folderName, fileFullPathForWrite, currentPool);
				//写完了就清空缓冲
				currentPool.ClearLogs();
				//记录最后一次写入时间
				currentPool.LastWriteTime = DateTime.Now;
			}
		}
		
		/// <summary>
		/// 强制写入日志文件,即便是没有到达缓存阈值或者是时间间隔
		/// </summary>
		public void Flush()
		{
			//可用于程序退出时或者用户手动调用，遍历所有的日志池，并将其中的日志写入文件
			foreach (var poolInfo in IndexedByFileFullPathLogsPoolInfos.Values)
			{
				if (poolInfo.HasWaitingToWriteLogs())
				{
					var fileFullPathForWrite = GetFullPathForWrite(poolInfo.LoggerName, poolInfo.ContinuePart);
					WriteLogsToFileAutoCreateFolderOrFileIfNeeded(poolInfo.FolderName, fileFullPathForWrite, poolInfo);
					poolInfo.ClearLogs();
				}
			}
		}

		/// <summary>
		/// 更新配置文件,如果配置文件无效则返回false并且返回错误信息
		/// 在初始化时候调用,如果配置文件有变动则调用
		/// </summary>
		/// <param name="config"></param>
		/// <param name="errorMessage"></param>
		/// <param name="createRootPathIfNotExist"></param>
		/// <returns></returns>
		public bool UpdateConfig(LogToFileConfig config, out string errorMessage,
			bool createRootPathIfNotExist = false)
		{
			#region 验证配置的有效性,有效才更新,无效抛出异常

			if (config == null || config.OnOff == false)
			{
				errorMessage = "无效的文件写入配置文件或未启用文件写入功能";
				return false;
			}

			#endregion

			if (createRootPathIfNotExist)
			{
				//如果根目录不存在则创建根目录
				if (!Directory.Exists(config.RootPath))
				{
					Directory.CreateDirectory(config.RootPath);
					Console.WriteLine($"在{nameof(LogFileWriter)}中更新配置时候,根目录不存在,已经创建根目录:{config.RootPath}");
				}
			}

			_config = config;
			errorMessage = string.Empty;
			return true;
		}

		/// <summary>
		/// 当程序退出时,将缓冲中的日志写入文件
		/// 防止程序退出时日志丢失
		/// </summary>
		public void Dispose()
		{
			Flush();

			// 停止计时器
			_timer.Stop();
			_timer.Dispose();
		}

		#endregion

		#region 内部字段,属性

		/// <summary>
		/// 文件写入配置文件
		/// </summary>
		private LogToFileConfig _config;

		/// <summary>
		/// 定时器,用于定时检查缓冲中的日志是否需要写入文件
		/// </summary>
		private readonly Timer _timer;

		/// <summary>
		/// 文件夹和文件监视器
		/// </summary>
		private readonly LogFolderAndFileWatcher _logFolderAndFileWatcher;

		/// <summary>
		/// 待写入的日志缓冲池,应按照文件夹名和文件名索引
		/// </summary>
		private Dictionary<string, LogsPoolInfo> IndexedByFileFullPathLogsPoolInfos { get; } =
			new Dictionary<string, LogsPoolInfo>();

		#endregion

		#region 内部逻辑,方法
		
		/// <summary>
		/// 获取待写入的文件全路径,如果有分段则返回分段的路径
		/// </summary>
		/// <param name="loggerName"></param>
		/// <param name="continuePart"></param>
		/// <returns></returns>
		private string GetFullPathForWrite(string loggerName, uint? continuePart)
		{
			Util.CalcLogFileAndFolderName(loggerName, DateTime.Now, _config, out var fileName, out var folderName);
			if (continuePart == null)
			{
				return Path.Combine(_config.RootPath, folderName, fileName);
			}

			return Path.Combine(_config.RootPath, folderName, fileName).Replace(Model.Constant.DefaultLogFileExtension,
				$"_part{continuePart}.{Model.Constant.DefaultLogFileExtension}");
		}

		/// <summary>
		/// 将缓冲中的日志写入到文件
		/// 写入之前会先计算日志文件名和文件夹名,然后检查是否需要创建文件夹或者文件,如果需要则自动创建
		/// 如果文件夹或者文件创建成功则尝试追加到文件,如果遇到文件锁之类的则控制台输出异常
		/// </summary>
		/// <param name="folderName"></param>
		/// <param name="fullPath"></param>
		/// <param name="logsPoolInfo"></param>
		private void WriteLogsToFileAutoCreateFolderOrFileIfNeeded(string folderName, string fullPath, LogsPoolInfo logsPoolInfo)
		{
			var fileName = Path.GetFileName(fullPath);

			#region 不推荐的方法,每次都询问一下是否存在文件夹和文件,这样会增加IO,降低性能

			// //如果文件夹不存在则创建文件夹
			// if (!Directory.Exists(Path.Combine(_config.Path, folderName)))
			// {
			// 	Directory.CreateDirectory(Path.Combine(_config.Path, folderName));
			// 	OnNewFolderCreated(folderName);
			// }
			// //如果文件不存在则创建文件
			// if (!File.Exists(Path.Combine(_config.Path, folderName, fileName)))
			// {
			// 	File.Create(Path.Combine(_config.Path, folderName, fileName));
			// 	OnNewFileCreated(fileName);
			// }

			#endregion

			#region 推荐的方法,先检查缓存表,如果有则不创建,如果没有则创建,因为缓存表的数据是启动检测器之前就检查一次,后面有了改动以后还在缓存中更新,所以缓存表就是实际的文件夹和文件的状态

			LogFolderAndFileWatcher.GetIsFolderAndFileInCache(fullPath, out var isFolderInCache,
				out var isFileInCache);
			if (!isFolderInCache)
			{
				Directory.CreateDirectory(Path.Combine(_config.RootPath, folderName));
				OnNewFolderCreated(folderName);
			}

			if (!isFileInCache)
			{
				var fileStream = File.Create(fullPath);
				OnNewFileCreated(fullPath);
				var fileHeaderBytes = GenerateFileHeader(logsPoolInfo);
				//写入文件头
				fileStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
				//不能忘记关闭文件流
				fileStream.Close();
			}

			#endregion

			//尝试追加到文件,如果遇到文件锁之类的则控制台输出异常
			try
			{
				//使用WriteAllBytes会导致文件被覆盖,所以要使用FileStream
				// File.WriteAllBytes(fullPath, bytes);
				using (var fileStream =
				       new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
				{
					var bytes = logsPoolInfo.GetAllLogsBytes();
					fileStream.Write(bytes, 0, bytes.Length);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"在{nameof(LogFileWriter)}中写入文件时候发生异常,文件路径:{fullPath},异常信息:{e.Message}");
				//TODO: 如果写入文件失败,是否重试,这个概率虽然很低,但是还是有可能的
			}
		}

		private static byte[] GenerateFileHeader(LogsPoolInfo logsPoolInfo)
		{
			var headerStringBuilder = new StringBuilder();
			headerStringBuilder.AppendLine("----------------------------------------------------------------------------");
			headerStringBuilder.AppendLine($"日志记录器名称	:	{logsPoolInfo.LoggerName}");
			headerStringBuilder.AppendLine($"日志目录		:	{logsPoolInfo.FolderName}");
			headerStringBuilder.AppendLine($"日志池创建时间	:	{logsPoolInfo.CreateTime}");
			headerStringBuilder.AppendLine($"当前文件创建时间	:	{DateTime.Now}");
			//这个数值不准,因为是异步的,所以可能会有一些日志还没有写入
			// headerStringBuilder.AppendLine($"自创建以来已记录日志数(不含当前文件):	{logsPoolInfo.SincePoolCreatedLogsCount}");
			if (logsPoolInfo.ContinuePart != null)
				headerStringBuilder.AppendLine($"当前分段		:	{logsPoolInfo.ContinuePart}");
			//分割线
			headerStringBuilder.AppendLine("----------------------------------------------------------------------------");
			headerStringBuilder.AppendLine();
			headerStringBuilder.AppendLine();
			return Encoding.UTF8.GetBytes(headerStringBuilder.ToString());
		}

		#endregion

		#region 内部事件相关

		#region 实例内部事件的触发(当新文件夹或者新文件创建时候触发)

		private void OnNewFolderCreated(string folderName)
		{
			Console.WriteLine($"在{nameof(LogFileWriter)}中创建了新文件夹:{folderName}");
			NewFolderCreated?.Invoke(folderName);
			_logFolderAndFileWatcher.AddFolderToCache(folderName);
		}

		private void OnNewFileCreated(string fileFullPath)
		{
			Console.WriteLine($"在{nameof(LogFileWriter)}中创建了新文件:{fileFullPath}");
			NewFileCreated?.Invoke(fileFullPath);
			_logFolderAndFileWatcher.AddFileToCache(fileFullPath);
		}

		#endregion

		/// <summary>
		/// 当计时器到时,将缓冲中的日志写入文件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTimerElapsedWriteLogs(object sender, ElapsedEventArgs e)
		{
			// 获取当前时间
			var now = DateTime.Now;

			// 遍历所有的日志池
			var keys = IndexedByFileFullPathLogsPoolInfos.Keys.ToList();
			foreach (var key in keys)
			{
				var poolInfo = IndexedByFileFullPathLogsPoolInfos[key];

				// 检查是否应当写入
				if (now - poolInfo.LastWriteTime >= _config.WriteToFileInterval)
				{
					// 检查是否有日志需要写入
					if (poolInfo.HasWaitingToWriteLogs())
					{
						var fileFullPathForWrite = GetFullPathForWrite(poolInfo.LoggerName, poolInfo.ContinuePart);
						// 写入日志
						WriteLogsToFileAutoCreateFolderOrFileIfNeeded(poolInfo.FolderName,fileFullPathForWrite, poolInfo);

						// 清空日志池
						poolInfo.ClearLogs();

						// 更新最后一次写入时间
						poolInfo.LastWriteTime = now;

						// 更新字典中的值
						IndexedByFileFullPathLogsPoolInfos[key] = poolInfo;
					}
				}
			}

			Console.WriteLine(
				$"在{nameof(LogFileWriter)}中定时器到时,检查了所有的日志池,写入了需要写入的日志,共计{IndexedByFileFullPathLogsPoolInfos.Count}个日志池");
		}

		#endregion
	}
}