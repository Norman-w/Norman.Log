namespace Norman.Log.Server.Core;

/// <summary>
/// 连接的类型
/// </summary>
public enum ConnectionTypeEnum
{
	/// <summary>
	/// 未知
	/// </summary>
	Unknown = 0,
	/// <summary>
	/// Http
	/// </summary>
	Http = 1,
	/// <summary>
	/// WebSocket
	/// </summary>
	WebSocket = 2,
	/// <summary>
	/// gRPC
	/// </summary>
	Grpc = 3,
	/// <summary>
	/// 命名管道
	/// </summary>
	NamedPipe = 4,
	/// <summary>
	/// 应用程序内部直接调用,也就是进程内部调用
	/// </summary>
	Internal = 5,
}