/*
 
 
 
 日志到数据库的记录器.
 当到达指定数量或者定时器到达时间时,将日志写入到数据库中.
 缓存中的可存放日志数量和单次最大可写入到数据库的数量是不同的:
	如果单次写入到数据库的数量大于缓存中可存放的数量,则到达写入数量的阈值后会立即写入到数据库中.
	如果单次写入到数据库的数量小于缓存中可存放的数量,则会分批写入到数据库中.
 比如,单次写入数量配置为1000,而缓存最高数量为100,则不会有1000条才写入到数据库的情况
 比如,单次写入数量配置为100,而缓存最高数量为1000,则如果大于100不够1000时,分批写入到数据库中.
 
 不管到不到缓存数量阈值,只要到达定时器时间,都会写入到数据库中.但写入的数量不会超过单次写入数量阈值.

*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Norman.Log.Component.Database.Mysql.Context;
using Norman.Log.Config;

namespace Norman.Log.Component.DatabaseWriter
{
	#region 对业务模型日志类的扩展方法,从业务模型转换为数据库模型以便写入数据库

	/// <summary>
	/// 业务模型日志类的扩展方法
	/// </summary>
	public static class LogExtension
	{
		/// <summary>
		/// 转换为数据库模型
		/// </summary>
		/// <param name="log"></param>
		/// <returns></returns>
		public static Database.Mysql.Context.Log ToDbModel(this Model.Log log)
		{
			return new Database.Mysql.Context.Log
			{
				Id = log.Id,
				CreateTime = log.CreateTime,
				LoggerName = log.LoggerName,
				Type = (int)log.Type.Value,
				Layer = (int)log.Layer.Value,
				Module = log.Module,
				Summary = log.Summary,
				Detail = log.Detail,
				Context = log.LogContext == null ? null : JsonConvert.SerializeObject(log.LogContext)
			};
		}
	}

	#endregion
	
	/// <summary>
	/// 日志到数据库的写入器.
	/// </summary>
	public class LogDatabaseWriter : IDisposable
	{
		#region 私有字段

		/// <summary>
		/// 待写入队列
		/// </summary>
		private readonly ConcurrentQueue<Model.Log> _waitingToWriteQueue = new ConcurrentQueue<Model.Log>();
		/// <summary>
		/// 定时器,用于定时写入日志到数据库
		/// </summary>
		private readonly Timer _timer;

		/// <summary>
		/// 正在执行的写入任务
		/// </summary>
		private readonly ConcurrentDictionary<int, Task> _flushTasks =
			new ConcurrentDictionary<int, Task>();

		/// <summary>
		/// 写入数据库的配置
		/// </summary>
		private LoggerConfig.LogToDatabaseConfig _config;

		#endregion

		#region 构造函数

		/// <summary>
		/// 使用写入数据库的配置初始化一个日志到数据库的写入器
		/// </summary>
		/// <param name="config"></param>
		/// <exception cref="InvalidDataException"></exception>
		public LogDatabaseWriter(LoggerConfig.LogToDatabaseConfig config)
		{
			if (!UpdateConfig(config, out var errorMessage))
			{
				throw new InvalidDataException(errorMessage);
			}
			#region 启动定时器
			//如果定时器时间间隔为0,则不启动定时器
			if (_config.WriteToDatabaseInterval == TimeSpan.Zero)
			{
				Console.WriteLine("日志到数据库的写入器,定时器时间间隔为0,不启动定时器.");
				return;
			}
			_timer = new Timer(state =>
				{
					if (_waitingToWriteQueue.Count <= 0) return;
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine($"时间到,开始写入日志到数据库...待写入日志数量:{_waitingToWriteQueue.Count}");
					Console.ResetColor();
					CreateAndRunFlushTask();
				}, 
				null, 
				_config.WriteToDatabaseInterval,//第一次启动要延迟相同的时间间隔而不是上来就启动
				_config.WriteToDatabaseInterval);
			#endregion
		}

		#endregion

		#region 对外,公共函数,包括更新配置,添加日志到队列,强制写入日志到数据库,释放资源等

		/// <summary>
		/// 更新配置,如果配置无效,则返回false,并且返回错误信息
		/// </summary>
		/// <param name="config"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		public bool UpdateConfig(LoggerConfig.LogToDatabaseConfig config, out string errorMessage)
		{
			#region 验证配置的有效性,有效才更新,无效抛出异常

			if (config == null || config.OnOff == false)
			{
				errorMessage = "无效的配置或者配置未开启写入到数据库";
				return false;
			}

			#endregion
			_config = config;
			errorMessage = string.Empty;
			return true;
		}
		/// <summary>
		/// 将日志添加到待写入队列中
		/// 如果够缓冲区大小,则写入到数据库中
		/// 另外加入到队列中的日志如果没到数量阈值,也会在定时器到达时间之后写入到数据库中
		/// </summary>
		/// <param name="log"></param>
		public void AddLogToWaitingToWriteQueue(Model.Log log)
		{
			_waitingToWriteQueue.Enqueue(log);
			if (_waitingToWriteQueue.Count >= _config.MaxLogCountInCache)
			{
				//取出最大单次写入数量的日志,如果没有那么多,就全部取出来
				//因为取出的时候可能又有别的进程塞进来了,或者是别的进程都取走了去写了,所以如果指定取出大小会出问题,要用TakeoutLogs常识性的去取
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine($"日志缓冲区已满,开始写入日志到数据库...待写入日志数量:{_waitingToWriteQueue.Count}");
				Console.ResetColor();
				CreateAndRunFlushTask();
			}
		}
		
		
		/// <summary>
		/// 将队列中的日志写入到数据库中
		/// </summary>
		public void Flush()
		{
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine($"强制写入日志到数据库...待写入日志数量:{_waitingToWriteQueue.Count}");
			Console.WriteLine($"未完成的任务数量:{_flushTasks.Count}");
			//等待所有的task执行完成
			Task.WaitAll(_flushTasks.Values.ToArray());
			Console.WriteLine("强制写入日志到数据库完成.");
			if (_flushTasks.Count <= 0) return;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("强制写入日志到数据库完成后,仍然有未完成的任务.");
			Console.ResetColor();
			throw new InvalidOperationException("强制写入日志到数据库完成后,仍然有未完成的任务.");
		}

		/// <summary>
		/// 释放资源,关闭定时器并且强制写入日志到数据库
		/// </summary>
		public void Dispose()
		{
			Flush();
			_timer?.Dispose();
		}

		#endregion

		#region 私有,内部业务

		/// <summary>
		/// 创建并运行一个写入数据库的任务
		/// 当任务完成后,会从任务字典中移除
		/// </summary>
		private async void CreateAndRunFlushTask()
		{
			//已取出的日志集
			var takeoutLogs = TakeoutLogs();
			var task = Task.Run(() => WriteLogsToDatabase(takeoutLogs));
			// var taskId = Guid.NewGuid();
			var taskId = task.Id;
			_flushTasks.TryAdd(taskId, task);
			await task;
			if (task.IsFaulted)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"写入数据库任务出现异常:{task.Exception}");
				Console.ResetColor();
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("写入数据库任务完成.");
				Console.ResetColor();
			}
			//注意这里是立即调用
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"创建并运行写入数据库任务...任务Id:{task.Id},待写入日志数量:{takeoutLogs.Count}");
			Console.ResetColor();
			_flushTasks.TryRemove(taskId, out _);
		}
		
		/// <summary>
		/// 将日志写入到数据库
		/// </summary>
		/// <param name="logs"></param>
		private static void WriteLogsToDatabase(IEnumerable<Model.Log> logs)
		{
			using (var dbContext = new NormanLogDbContext())
			{
				foreach (var log in logs)
				{
					dbContext.Log.Add(log.ToDbModel());
				}

				dbContext.SaveChanges();
			}
		}

		/// <summary>
		/// 取出最大数量的日志(单次可以写入到数据库的最大量),如果没有那么多,则取出全部
		/// </summary>
		/// <returns></returns>
		private List<Model.Log> TakeoutLogs()
		{
			var logs = new List<Model.Log>();
			while (_waitingToWriteQueue.TryDequeue(out var log))
			{
				logs.Add(log);
				if (logs.Count < _config.MaxFlushCountPerTime) continue;
				break;
			}

			Console.WriteLine($"本次取出日志数量:{logs.Count}");

			return logs;
		}
		

		#endregion
	}
}