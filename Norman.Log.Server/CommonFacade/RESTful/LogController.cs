/*
 
 
 日志服务RESTful接口
 直接使用get请求访问即可根据条件查询日志
 
 
 
 */

using Microsoft.AspNetCore.Mvc;
using Norman.Log.Component.DatabaseReader;
using Norman.Log.Model;
using Norman.Log.Server.Core;

namespace Norman.Log.Server.CommonFacade.RESTful;

#region 日志查询接口
[ApiController]
[Route("[controller]")]
public partial class LogController : ControllerBase
{
	/// <summary>
	/// 日志查询参数
	/// </summary>
	public class LogQueryRequest
	{
		/// <summary>
		/// 查询的开始时间
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public DateTime? StartTime { get; set; }
		/// <summary>
		/// 查询的结束时间
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public DateTime? EndTime { get; set; }
		/// <summary>
		/// 所需要查询的日志记录器名称,可能是多个,用逗号或者是空格分隔,需要拆分出来依次匹配,如"logger1,logger2"
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public string? LoggerNameList { get; set; }
		/// <summary>
		/// 所需要查询的日志类型,可能是多个,用逗号或者是空格分隔,需要拆分出来依次匹配,如0,1,3
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public string? TypeList { get; set; }
		/// <summary>
		/// 所需要查询的日志层级,可能是多个,用逗号或者是空格分隔,需要拆分出来依次匹配,如1,2,4
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public string? LayerList { get; set; }
		/// <summary>
		/// 所需要查询的模块名称,可能是多个,用逗号或者是空格分隔,需要拆分出来依次匹配,如"模块1,模块2"
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public string? ModuleList { get; set; }
		/// <summary>
		/// 搜索概要和详情时的关键字集,可能用逗号或者是空格分隔,需要拆分出来依次匹配,如"关键字1,关键字2"
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public string? SummaryAndDetailTags { get; set; }
		/// <summary>
		/// 上下文信息中包含的关键字集,可能用逗号或者是空格分隔,需要拆分出来依次匹配,如"关键字1,关键字2"
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public string? ContextTags { get; set; }
		/// <summary>
		/// 查询的页码,从1开始
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
		public int PageNumber { get; set; }
		/// <summary>
		/// 查询的每页大小
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json反序列化时使用
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
	public class LogQueryResponse
	{
		// ReSharper disable once MemberCanBePrivate.Global 将被序列化后返回到客户端
		public string ErrMsg { get; set; } = string.Empty;
		public bool IsError => !string.IsNullOrWhiteSpace(ErrMsg);

		/// <summary>
		/// 本次查询到的日志
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json序列化时使用
		public List<LogRecord4Net> Logs { get; set; } = new();
		/// <summary>
		/// 符合条件的总日志数量
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global 将被json序列化时使用
		public uint TotalResultCount { get; set; }
	}
	/// <summary>
	/// 通过给定的参数来查询日志,返回符合条件的日志列表,包含详细信息,不精简字段
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpPost("")]//这里必须要写成[HttpPost + ("")]的形式或其他的每个POST都明确请求的路径,否则在默认不写POST参数的情况下会导致Swagger因路径冲突无法生成
	public IActionResult GetLogs(LogQueryRequest request)
	{
		LogQueryDatabaseArgs queryDatabaseArgs;
		try
		{
			queryDatabaseArgs = request.ToLogQueryDatabaseArgs();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return BadRequest(e.Message);
		}
		var dbLogs =DatabaseReader.GetLogsFromDb(queryDatabaseArgs, out var totalResultCount);
		//转换成业务模型
		var logs = dbLogs.Select(x=>x.ToBllModel()).ToList();
		//转换成网络模型
		var logRecords = logs.Select(LogRecord4Net.FromLog).ToList();
		//TODO 移除如下Region代码:
		#region 如果dbLogs没搜到,生成一些mock日志然后转换成网络模型返回
		if (logRecords.Count == 0)
		{
			for (var i = 0; i < 10; i++)
			{
				var randomLog = MockLogGenerator.RandomLog();
				logRecords.Add(LogRecord4Net.FromLog(randomLog));
			}
		}

		totalResultCount = 1000;
		#endregion
		var response = new LogQueryResponse
		{
			Logs = logRecords,
			TotalResultCount = totalResultCount,
		};
		return Ok(response);
	}

	/// <summary>
	/// 通过给定的参数来查询日志,返回符合条件的日志列表,包含详细信息,不精简字段
	/// </summary>
	/// <param name="id"></param>
	/// <param name="createTime"></param>
	/// <param name="type"></param>
	/// <param name="layer"></param>
	/// <param name="loggerNameTags"></param>
	/// <param name="moduleTags"></param>
	/// <param name="contentTags"></param>
	/// <returns></returns>
	[HttpGet("")]//这里可写成[HttpGet],也可以写成[HttpGet("Index")]
	public IActionResult GetLogs(string? id, string? createTime, uint? type, uint? layer, string? loggerNameTags,
		string? moduleTags, string? contentTags)
	{
		var result = new List<LogRecord4Net>();
		for (var i = 0; i < 10; i++)
		{
			var randomLog = MockLogGenerator.RandomLog();
			result.Add(LogRecord4Net.FromLog(randomLog));
		}
		return Ok(new { data=result, success = true, total = 10000, message = $"你的查询参数是: id={id}, createTime={createTime}, type={type}, layer={layer}, loggerNameTags={loggerNameTags}, moduleTags={moduleTags}, contentTags={contentTags}"});
	}
}
#endregion

#region 客户端上报日志给服务端接口
public partial class LogController
{
	/// <summary>
	/// 客户端通过HTTP POST请求上报日志给服务器
	/// 地址如:http://localhost:5012/Log/Report
	/// </summary>
	/// <param name="logRecord4Net">日志的网络传输模型</param>
	/// <returns></returns>
	[HttpPost("Report")]
	public IActionResult Report(LogRecord4Net logRecord4Net)
	{
		var log = Model.Log.FromLogRecord4Net(logRecord4Net);
		App.Server.HandleLog(this,log);
		//如果他的请求结果说是要返回json(header里面设置的),那么给它返回json
		var needJsonResponse = Request.Headers["Accept"].Contains("application/json");
		if (!needJsonResponse) return Ok();
		var rspJson = new { Success = true, Message = $"日志{logRecord4Net.Id}已经被成功接收于{DateTime.Now}" };
		return Ok(rspJson);
	}
}

#endregion
