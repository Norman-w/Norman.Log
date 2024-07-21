/*
 
 
 日志服务器中,每一个连接,如日志报送器,日志接收器(监视器),日志记录器(文件/数据库),都视为一个客户端
 一个客户端可以是一个WebSocket连接创建而来,也可以是gRcp等连接的方式.
 
 所有的客户端都支持断开连接事件,当客户端断开连接时,会触发ClientDisconnected事件.
 
 比如当接收端的WebSocket客户端断开连接时,会触发ClientDisconnected事件,然后关闭WebSocket连接.
 销毁客户端对象,从接收者列表中移除该客户端,以后不会再给他发送消息.
 


*/


namespace Norman.Log.Server.Core;
/// <summary>
/// 客户端断开连接事件处理委托
/// </summary>
public delegate void ClientDisconnectedEventHandler(IClient sender);

public interface IClient
{
	/// <summary>
	/// 客户端断开连接事件
	/// </summary>
	public event ClientDisconnectedEventHandler? ClientDisconnected;
	/// <summary>
	/// 启动运行(如果是WebSocket连接,则用于接收消息的回调函数)
	/// </summary>
	/// <returns></returns>
	public Task StartWorking();
}