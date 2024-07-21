namespace Norman.Log
{
	public interface IReceiver
	{
		void Receive(Model.Log logEntry);
	}
}