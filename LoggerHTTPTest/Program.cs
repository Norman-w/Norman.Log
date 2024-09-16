using Norman.Log.Logger.HTTP;

namespace LoggerHTTPTest
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			Logger.UpdateConfig("http://localhost:5012/Log/Report", "DefaultLogger");
			Logger.Write(LogTypeEnum.Info, LogLayerEnum.Business, "TestModule", "TestSummary", "TestDetail");
		}
	}
}