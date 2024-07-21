namespace Norman.Log
{
	public interface IWriter
	{
		void Write(Model.Log logEntry);
	}
}