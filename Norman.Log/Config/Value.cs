/*
 
 
 配置的值信息,包含配置的Item模板
 配置是否启用
 配置的值


*/

namespace Norman.Log.Config
{
	public interface IConfigValue<T>
	{
		ItemType<T> ItemType { get; set; }
		bool Enabled { get; set; }
		T Value { get; set; }
	}
}