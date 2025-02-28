namespace Norman.Log.Config
{
	public interface ICommonConfig<out T>
	{
		T GetDefault();
		void Populate(string json);
		void FromFile(string path, bool tryCreateIfNotExist = true);
	}
}