/*
 
 
 日志记录器测试程序.
 
 */

using Norman.Log.Config;
using Norman.Log.Logger;
using GrpcNamedLogger = Norman.Log.Logger.gRpc.NamedLogger;

Console.WriteLine("日志记录器测试程序,正在启动...");

#region 参数定义

const int totalLogCount = 10;
var currentLogCount = 0;
//记录程序启动时间
var startTime = DateTime.Now;

#endregion

Console.WriteLine($"日志将输出到路径:{AppConfig.LoggerConfig.LogToFile.RootPath}");

//使用Grpc日志记录器
var grpcNamedLogger = new GrpcNamedLogger("Default", "http://localhost:5011");
LogHelper.UseNamedLogger(grpcNamedLogger);

#region 生成并输出到日志记录器

for (var i = 0; i < totalLogCount; i++)
{
 LogHelper.InfoFormat($"当前正在记录第{i+1}条日志...", totalLogCount, currentLogCount);
 currentLogCount++;
}

#endregion

#region 确保日志都被写入,正常使用时,应该在应用程序退出时或者全局捕获异常时调用

//兜底,确保所有日志都被写入
LogHelper.Flush();

#endregion

#region 计算并输出总共耗时

Console.WriteLine("日志记录器测试程序,已结束.");
var endTime = DateTime.Now;
var timeSpan = endTime - startTime;
Console.WriteLine($"总共记录{totalLogCount}条日志,总共耗时:{timeSpan.TotalSeconds}秒.");


#endregion