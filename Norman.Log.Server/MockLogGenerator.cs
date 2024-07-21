/*
 
 
 模拟日志数据生成器,用于测试.


*/


using Norman.Log.Model;
using Norman.Log.Server.CommonFacade;
using Norman.Log.Server.Core;

namespace Norman.Log.Server;

/// <summary>
/// 模拟日志数据生成器,用于测试.
/// </summary>
public static class MockLogGenerator
{
	/// <summary>
	/// 已生成的日志数量
	/// </summary>
	private static int _totalGeneratedLogsCount;
	/// <summary>
	/// 生成的总日志数量
	/// </summary>
	private const int TotalLogsCount = 1000000;
	/// <summary>
	/// 开始生成模拟日志数据,生成后会发送给所有的接收者.
	/// </summary>
	/// <param name="server"></param>
	public static async Task Start(Core.Server server)
	{
		#region 模拟数据生成
        var mockupReporter = new ReporterClient(new SessionCreatedEventArgs("mockup", new object()));
        //单独开一个线程,每秒生成1~20条数据,数据的所有内容都是随机的,然后发送给所有的接收者.
        var random = new Random();
        
        //每秒生成1~20条数据,数据的所有内容都是随机的,然后发送给所有的接收者.
        await Task.Run(function: async () =>
        {
        	while (_totalGeneratedLogsCount < TotalLogsCount)
        	{
        		var count = random.Next(1, 20);
        		Console.WriteLine($"生成{count}条随机日志,总计{_totalGeneratedLogsCount}条");
        		for (var i = 0; i < count; i++)
        		{
        			SendRandomLog(_totalGeneratedLogsCount++, server, mockupReporter);
        		}
        		var randomDelay = random.Next(100, 3000);
        		await Task.Delay(randomDelay);
        	}
        });
        #endregion
	}
	//生成随机字符串的方法
	private static string RandomString(int length)
	{
		var random = new Random();
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
		return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[random.Next(s.Length)]).ToArray());
	}
        
	//生成随机的日志
	private static Model.Log RandomLog()
	{
		var log = new Model.Log
		{
			Summary = RandomString(10),
			Detail = RandomString(200),
			Type = RandomLogType(),
			Layer = RandomLogLayer(), LoggerName = RandomString(5), Module = RandomString(5),
		};
		return log;
	}
        
	//生成随机的日志,并发送给所有的接收者
	private static void SendRandomLog(int index, Core.Server server, ReporterClient mockupReporter)
	{
		var log = RandomLog();
		log.Summary = $"{index}:{log.Summary}";
		server.HandleLog(mockupReporter, log);
	}
	//生成随机的日志类型方法
	private static LogType RandomLogType()
	{
		var random = new Random();
		return LogType.KnownLogTypes[random.Next(LogType.KnownLogTypes.Length)];
	}
        
	//生成随机的日志所在层的方法
	private static LogLayer RandomLogLayer()
	{
		var random = new Random();
		return LogLayer.KnownLogLayers[random.Next(LogLayer.KnownLogLayers.Length)];
	}
}