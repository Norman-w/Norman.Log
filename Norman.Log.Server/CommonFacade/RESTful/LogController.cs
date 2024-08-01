/*
 
 
 日志服务RESTful接口
 直接使用get请求访问即可根据条件查询日志
 
 
 
 */

using Microsoft.AspNetCore.Mvc;
using Norman.Log.Component.DatabaseReader;
using Norman.Log.Model;
using Norman.Log.Server.Core;

namespace Norman.Log.Server.CommonFacade;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
	/// <summary>
	/// 日志查询参数
	/// </summary>
	public class LogQueryArgs
	{
		/// <summary>
		/// 查询的开始时间
		/// </summary>
		public DateTime? StartTime { get; set; }
		/// <summary>
		/// 查询的结束时间
		/// </summary>
		public DateTime? EndTime { get; set; }
		/// <summary>
		/// 所需要查询的日志记录器名称,可能是多个,用逗号或者是空格分隔,需要拆分出来依次匹配,如"logger1,logger2"
		/// </summary>
		public string? LoggerNameList { get; set; }
		/// <summary>
		/// 所需要查询的日志类型,可能是多个,用逗号或者是空格分隔,需要拆分出来依次匹配,如0,1,3
		/// </summary>
		public string? TypeList { get; set; }
		/// <summary>
		/// 所需要查询的日志层级,可能是多个,用逗号或者是空格分隔,需要拆分出来依次匹配,如1,2,4
		/// </summary>
		public string? LayerList { get; set; }
		/// <summary>
		/// 所需要查询的模块名称,可能是多个,用逗号或者是空格分隔,需要拆分出来依次匹配,如"模块1,模块2"
		/// </summary>
		public string? ModuleList { get; set; }
		/// <summary>
		/// 搜索概要和详情时的关键字集,可能用逗号或者是空格分隔,需要拆分出来依次匹配,如"关键字1,关键字2"
		/// </summary>
		public string? SummaryAndDetailTags { get; set; }
		/// <summary>
		/// 上下文信息中包含的关键字集,可能用逗号或者是空格分隔,需要拆分出来依次匹配,如"关键字1,关键字2"
		/// </summary>
		public string? ContextTags { get; set; }
		/// <summary>
		/// 查询的页码
		/// </summary>
		public int PageNumber { get; set; }
		/// <summary>
		/// 查询的每页大小
		/// </summary>
		public int PageSize { get; set; }
	
		/// <summary>
		/// 转换为数据库查询参数
		/// </summary>
		/// <returns></returns>
		public LogQueryDatabaseArgs ToLogQueryDatabaseArgs()
		{
			//type list 和 layer list 里面的数据必须是有效的0+整数,如果不是,则抛出错误
			if (!string.IsNullOrWhiteSpace(TypeList))
			{
				if (!TypeList.Split(',', ' ').All(s => int.TryParse(s, out _)))
				{
					throw new ArgumentException("TypeList must be a list of integers.");
				}
			}
			if (!string.IsNullOrWhiteSpace(LayerList))
			{
				if (!LayerList.Split(',', ' ').All(s => int.TryParse(s, out _)))
				{
					throw new ArgumentException("LayerList must be a list of integers.");
				}
			}
			var args = new LogQueryDatabaseArgs
			{
				StartTime = StartTime,
				EndTime = EndTime,
				Skip = (PageNumber - 1) * PageSize,
				Take = PageSize
			};
			if (!string.IsNullOrWhiteSpace(LoggerNameList))
			{
				args.LoggerNameList = LoggerNameList.Split(',', ' ').ToList();
			}
			if (!string.IsNullOrWhiteSpace(TypeList))
			{
				args.TypeList = TypeList.Split(',', ' ').Select(int.Parse).ToList();
			}
			if (!string.IsNullOrWhiteSpace(LayerList))
			{
				args.LayerList = LayerList.Split(',', ' ').Select(int.Parse).ToList();
			}
			if (!string.IsNullOrWhiteSpace(ModuleList))
			{
				args.ModuleList = ModuleList.Split(',', ' ').ToList();
			}
			if (!string.IsNullOrWhiteSpace(SummaryAndDetailTags))
			{
				args.SummaryTags = SummaryAndDetailTags.Split(',', ' ').ToList();
				args.DetailTags = SummaryAndDetailTags.Split(',', ' ').ToList();
			}
			if (!string.IsNullOrWhiteSpace(ContextTags))
			{
				args.ContextTags = ContextTags.Split(',', ' ').ToList();
			}
			return args;
		}
	}
	[HttpPost]
	public IActionResult Index(LogQueryArgs query)
	{
		LogQueryDatabaseArgs queryDatabaseArgs;
		try
		{
			queryDatabaseArgs = query.ToLogQueryDatabaseArgs();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return BadRequest(e.Message);
		}
		var dbLogs =DatabaseReader.GetLogsFromDb(queryDatabaseArgs);
		//转换成业务模型
		var logs = dbLogs.Select(x=>x.ToBllModel()).ToList();
		//转换成网络模型
		var logRecords = logs.Select(LogRecord4Net.FromLog).ToList();
		return Ok(logRecords);
	}
}