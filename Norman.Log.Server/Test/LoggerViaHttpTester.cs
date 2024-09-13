using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Norman.Log.Model;

namespace Norman.Log.Server.Test;

public static class LoggerViaHttpTester
{
	public static void TestReportLog()
	{
		/*
		 
		 
		 本测试将使用http post请求到server的日志记录器,进行日志记录.
		 
		 请求地址是:http://localhost:5012/Log/Report
		 发送请求的格式为logRecord4Net
		 
		 测试时需要先build并运行Norman.Log.Server,然后使用Rider执行此函数
		 
        */
		
		//模拟生成一条日志
		var stopWatch = new Stopwatch();
		stopWatch.Start();
		var mockLog = MockLogGenerator.RandomLog();
		var logRecord4Net = LogRecord4Net.FromLog(mockLog);
		var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5012/Log/Report");
		request.Content = new StringContent(JsonConvert.SerializeObject(logRecord4Net), Encoding.UTF8, "application/json");
		//设置Header,接收格式为json,这样服务器会返回json格式的数据以指示日志记录结果
		request.Headers.Add("Accept", "application/json");
		
		var client = new HttpClient();
		var response = client.SendAsync(request).Result;
		Console.WriteLine($"发送日志到服务器,返回结果 Code:{response.StatusCode},内容:{response.Content.ReadAsStringAsync().Result}");
		stopWatch.Stop();
		Console.WriteLine($"发送日志到服务器,总共耗时:{stopWatch.Elapsed.TotalSeconds}秒.");
	}
}