//启动内核(日支池处理器等)

//启动gRPC, http, websocket服务

using Norman.Log.Server;
using Norman.Log.Server.CommonFacade;
using Norman.Log.Server.Core;

//创建一个Norman.Log.Server.CommonFacade.Net实例,并启动
Net netFacadeServer = new();
netFacadeServer.Start();

//启动命名管道服务


//接下来就是各种绑定了.
//比如,将grpc接收到grpc客户端消息的回调函数内绑定上core中的server的添加日志到缓存池的函数.
//或者从http服务接收到的消息,解析出来类似发给控制面板的消息,然后调用控制面板的函数,再返回等.

//构建服务
// Server server = new();
// App.Server = server;


//当有新会话创建时
netFacadeServer.SessionCreatedAsync += async session =>
{
	Console.WriteLine($"已创建新会话: {session.ClientType}");
	switch (session.ClientType)
	{
		case ClientTypeEnum.Reporter:
		{
			var client = ReporterClient.FromSession(session);
			client.LogReceived += App.Server.HandleLog;
			client.ClientDisconnected += (c) =>
			{
				Console.WriteLine($"报告者客户端断开连接: {c}");
				App.Server.RemoveReporter(c as ReporterClient ?? throw new InvalidOperationException("Invalid client"));
			};
			App.Server.AddReporter(client);
			await client.StartWorking();
			break;
		}
		case ClientTypeEnum.Receiver:
		{
			var client = ReceiverClient.FromSession(session);
			client.ClientDisconnected += (c) =>
			{
				Console.WriteLine($"接收者客户端断开连接: {c}");
				App.Server.RemoveReceiver(c as ReceiverClient ?? throw new InvalidOperationException("Invalid client"));
			};
			App.Server.AddReceiver(client);
			await client.StartWorking();
			break;
		}
		case ClientTypeEnum.Unknown:
		case ClientTypeEnum.Writer:
		default:
			throw new ArgumentOutOfRangeException($"未知的客户端类型: {session.ClientType}");
	}
	//TODO 其他类型的客户端
};

//http,WebSocket,gRpc 都可以用同一个端口,也可以用不同的端口,看具体需求,但是grpc和grpc web的兼容性问题要求grpc web需要单独的端口使用http1协议
//内部调用就是直接通过Norman.Log.Logger中引用Norman.Log.Server,把Server定义成InternalServer,
//然后记录日志的时候直接就是Norman.Log.Logger.InternalServer.AddLog()就行了.


#region Task:启动日志模拟生成器并广播日志给所有接收者

var mockLogGenerateTask = MockLogGenerator.Start(App.Server);

#endregion

#region Task:等待用户输入,输入exit时退出

var userInputTask = Task.Run(() =>
{
	while (true)
	{
		var input = Console.ReadLine();
		if (input == "exit")
		{
			break;
		}
	}
});

#endregion

await Task.WhenAny(
	mockLogGenerateTask,
	userInputTask
);

Console.WriteLine("正在退出...");