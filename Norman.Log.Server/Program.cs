//启动内核(日支池处理器等)

//启动gRPC, http, websocket服务
Norman.Log.Server.CommonFacade.Net netFacadeServer = new();
netFacadeServer.Start();

//启动命名管道服务


//接下来就是各种绑定了.
//比如,将grpc接收到grpc客户端消息的回调函数内绑定上core中的server的添加日志到缓存池的函数.
//或者从http服务接收到的消息,解析出来类似发给控制面板的消息,然后调用控制面板的函数,再返回等.

//http,WebSocket,gRpc 都可以用同一个端口,也可以用不同的端口,看具体需求.
//内部调用就是直接通过Norman.Log.Logger中引用Norman.Log.Server,把Server定义成InternalServer,
//然后记录日志的时候直接就是Norman.Log.Logger.InternalServer.AddLog()就行了.

//等待用户输入,输入exit后,关闭所有服务.
while (true)
{
	var input = Console.ReadLine();
	if (input == "exit")
	{
		netFacadeServer.Stop();
		break;
	}
}