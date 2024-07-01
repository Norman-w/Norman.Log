/*
 
 使用grpc协议对外提供日志接收服务的接收器


*/

using System.ComponentModel;

namespace Norman.Log.Server.Input;

public class GrpcLogReceiver
{
	private readonly BackgroundWorker _worker = new();
	public GrpcLogReceiver()
	{
		_worker.DoWork += WorkerOnDoWork;
		_worker.RunWorkerAsync();
	}

	private static void WorkerOnDoWork(object? sender, DoWorkEventArgs e)
	{
	}
}