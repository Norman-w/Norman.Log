/*


 日志文件夹和日志文件的检测器.
 启动后添加对日志根目录的监视,如果有文件增加了,则记录到内存中.
 想要增加一个文件夹或者文件的时候要先看内存中是否记录了,如果记录了就放弃创建操作,直接写就行了.
 增加文件后不需要记录到内存中,因为文件监视器会监测到文件增加自动记录到内存中.


*/

using System;
using System.Collections.Generic;
using System.IO;
using Norman.Log.Config;

namespace Norman.Log.Component.FileWriter
{
	/// <summary>
	/// 日志文件夹和日志文件的检测器
	/// 会自动监测日志文件夹和日志文件的创建,并记录到内存中
	/// 会检查磁盘空间,并在空间不足的时候触发提醒
	/// </summary>
	public class LogFolderAndFileWatcher
	{
		#region 类型定义

		/// <summary>
		/// 磁盘空间不足提醒类型
		/// </summary>
		public enum RemainDiskSpaceNoticeTypeEnum
		{
			/// <summary>
			/// 无
			/// </summary>
			None,

			/// <summary>
			/// 通知
			/// </summary>
			Notify,

			/// <summary>
			/// 警告
			/// </summary>
			Warning,

			/// <summary>
			/// 危险
			/// </summary>
			Danger,

			/// <summary>
			/// 需要删除文件
			/// </summary>
			Delete
		}

		#endregion

		#region 属性和字段

		/// <summary>
		/// 文件/文件夹名缓存
		/// </summary>
		private static readonly Dictionary<string, DateTime> CachedFileAndFolderNameAndTime =
			new Dictionary<string, DateTime>();

		/// <summary>
		/// 生命周期的起始时间,基本也就等于说是程序运行了多长时间.
		/// </summary>
		public DateTime CreateTime { get; } = DateTime.Now;

		/// <summary>
		/// 最后一次文件或者文件夹创建的时间,可以用来放在仪表盘上
		/// 设置成这个值而不是使用计算方法的用意是防止每次都要计算,减少计算量
		/// </summary>
		public DateTime LastFileCreatedTime { get; private set; } = DateTime.MinValue;

		private LoggerConfig.LogToFileConfig _config;

		#endregion

		#region 构造和初始化

		/// <summary>
		/// 创建一个日志文件夹和文件监视器,监视日志和文件夹的创建
		/// 当新的日志或文件夹创建后会记录到缓存中
		/// 需要创建新日志文件之前要对比一下缓存中是否已经有了,如果有了就不需要再创建了
		/// </summary>
		/// <param name="config"></param>
		/// <exception cref="ArgumentException"></exception>
		public LogFolderAndFileWatcher(LoggerConfig.LogToFileConfig config)
		{
			if (!UpdateConfig(config, out var errorMessage))
			{
				throw new ArgumentException(errorMessage);
			}

			if (!StartFolderAndFileWatcher(out errorMessage))
			{
				throw new ArgumentException(errorMessage);
			}

			if (!GetAllFilesAndFolders(out var files, out var folders, out errorMessage))
			{
				throw new ArgumentException(errorMessage);
			}

			var now = DateTime.Now;
			foreach (var file in files)
			{
				CachedFileAndFolderNameAndTime.Add(file, now);
			}

			foreach (var folder in folders)
			{
				CachedFileAndFolderNameAndTime.Add(folder, now);
			}
		}

		#endregion

		#region 公有函数和事件
		
		/// <summary>
		/// 获取文件或者文件夹的创建时间,如果没有记录则返回null
		/// 返回null也就是说明这个文件没有在缓存中,那就可以尝试创建文件了.
		/// 先检查缓存会减少IO并提高性能
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DateTime? GetFileOrFolderCreateTime(string name)
		{
			if (CachedFileAndFolderNameAndTime.TryGetValue(name, out var time))
			{
				return time;
			}
			return null;
		}

		/// <summary>
		/// 更新配置
		/// </summary>
		/// <param name="config"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public bool UpdateConfig(LoggerConfig.LogToFileConfig config, out string errorMessage)
		{
			#region 验证配置的有效性,有效才更新,无效抛出异常

			if (config == null)
			{
				errorMessage = "无效的写入文件的配置";
				return false;
			}
			if(config.OnOff == false)
			{
				errorMessage = "写入文件的配置已经关闭";
				return false;
			}

			#endregion

			_config = config;
			errorMessage = string.Empty;
			return true;
		}


		/// <summary>
		/// 磁盘空间不足提醒事件
		/// </summary>
		public delegate void RemainDiskSpaceNoticeEventHandler(RemainDiskSpaceNoticeTypeEnum type, string message);

		/// <summary>
		/// 磁盘空间不足提醒事件
		/// </summary>
		public event RemainDiskSpaceNoticeEventHandler RemainDiskSpaceNoticeEvent;
		
		/// <summary>
		/// 由外部调用,添加文件到缓存中
		/// 如果外部添加了文件,等系统通的话会晚一步记录到缓存中,所以这里手动报送过来添加到缓存中防止重复创建和各种延迟导致的问题
		/// </summary>
		/// <param name="folderName"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void AddFolderToCache(string folderName)
		{
			if (!CachedFileAndFolderNameAndTime.ContainsKey(folderName))
			{
				CachedFileAndFolderNameAndTime.Add(folderName, DateTime.Now);
			}
		}

		/// <summary>
		/// 由外部调用,添加文件到缓存中
		/// 如果外部添加了文件,等系统通的话会晚一步记录到缓存中,所以这里手动报送过来添加到缓存中防止重复创建和各种延迟导致的问题
		/// </summary>
		/// <param name="fileName"></param>
		public void AddFileToCache(string fileName)
		{
			if (!CachedFileAndFolderNameAndTime.ContainsKey(fileName))
			{
				CachedFileAndFolderNameAndTime.Add(fileName, DateTime.Now);
			}
		}

		#endregion

		#region 私有内部业务逻辑

		/// <summary>
		/// 启动文件夹监测器
		/// </summary>
		/// <returns></returns>
		private bool StartFolderAndFileWatcher(out string errorMessage)
		{
			try
			{
				var watcher = new FileSystemWatcher(_config.RootPath);
				watcher.IncludeSubdirectories = true;
				watcher.Created += OnFolderOrFileCreated;
				watcher.EnableRaisingEvents = true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				errorMessage = e.Message;
				return false;
			}

			errorMessage = string.Empty;
			return true;
		}

		/// <summary>
		/// 获取所有文件和文件夹
		/// </summary>
		/// <param name="files"></param>
		/// <param name="folders"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		private bool GetAllFilesAndFolders(out string[] files, out string[] folders, out string errorMessage)
		{
			try
			{
				files = Directory.GetFiles(_config.RootPath, "*", SearchOption.AllDirectories);
				//获取到的路径如 2024, 06, 30这样的,但是需要获取的是 2024, 2024/06, 2024/06/30这样的,所以不能用这句
				// folders = Directory.GetDirectories(_config.RootPath, "*", SearchOption.AllDirectories);
				folders = Directory.GetDirectories(_config.RootPath, "*", SearchOption.AllDirectories);
				errorMessage = string.Empty;
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				errorMessage = e.Message;
				files = null;
				folders = null;
				return false;
			}
		}

		/// <summary>
		/// 当文件夹或者文件被创建的时候
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFolderOrFileCreated(object sender, FileSystemEventArgs e)
		{
			//如果在缓存中没有记录,则记录
			if (!CachedFileAndFolderNameAndTime.ContainsKey(e.FullPath))
			{
				if(!CachedFileAndFolderNameAndTime.ContainsKey(e.Name))
				{
					CachedFileAndFolderNameAndTime.Add(e.FullPath, DateTime.Now);
				}
				LastFileCreatedTime = DateTime.Now;
			}

			//顺便检查一下磁盘空间
			CheckDiskSpace();
		}

		/// <summary>
		/// 检查磁盘空间并触发提醒
		/// </summary>
		private void CheckDiskSpace()
		{
			//日志目录所在的磁盘
			var drive = new DriveInfo(_config.RootPath);
			//磁盘的剩余kb
			var remaining = drive.AvailableFreeSpace / 1024;
			//如果磁盘的剩余空间小于阈值,触发提醒
			if (remaining < _config.RemainKbTriggerNotify)
			{
				RemainDiskSpaceNoticeEvent?.Invoke(RemainDiskSpaceNoticeTypeEnum.Notify,
					$"提示:磁盘剩余空间不足,剩余{remaining}KB");
			}

			if (remaining < _config.RemainKbTriggerWarn)
			{
				RemainDiskSpaceNoticeEvent?.Invoke(RemainDiskSpaceNoticeTypeEnum.Warning,
					$"警告:磁盘剩余空间不足,剩余{remaining}KB");
			}

			if (remaining < _config.RemainKbTriggerDanger)
			{
				RemainDiskSpaceNoticeEvent?.Invoke(RemainDiskSpaceNoticeTypeEnum.Danger,
					$"危险:磁盘剩余空间不足,剩余{remaining}KB");
			}

			if (remaining < _config.RemainKbDelete)
			{
				RemainDiskSpaceNoticeEvent?.Invoke(RemainDiskSpaceNoticeTypeEnum.Delete,
					$"需要删除日志文件或者清理磁盘:磁盘剩余空间不足,剩余{remaining}KB");
			}
		}

		#endregion

		#region 供外部调用的公共方法,询问入参的文件或文件夹是否在缓存中

		/// <summary>
		/// 获取文件或者文件夹是否在缓存中
		/// </summary>
		/// <param name="fileFullPath"></param>
		/// <param name="isFolderExist"></param>
		/// <param name="isFileExist"></param>
		public static void GetIsFolderAndFileInCache(string fileFullPath, out bool isFolderExist,
			out bool isFileExist)
		{
			isFolderExist = CachedFileAndFolderNameAndTime.ContainsKey(Path.GetDirectoryName(fileFullPath) ?? throw new InvalidOperationException());
			isFileExist = CachedFileAndFolderNameAndTime.ContainsKey(fileFullPath);
		}

		#endregion

		
	}
}