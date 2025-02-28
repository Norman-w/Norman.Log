using System;
using Newtonsoft.Json;

namespace Norman.Log.Config
{
	public class IDatabaseConfigConfigJsonConverter : JsonConverter<IDatabaseConfig>
	{
		public override void WriteJson(JsonWriter writer, IDatabaseConfig value, JsonSerializer serializer)
		{
			//先当做接口,调用接口的获取类型的方法
			var databaseTypeEnum = value.DatabaseType;
			//再根据类型来获取实际的类型
			var configType = NormalDatabaseConfig.GetDatabaseConfigType(databaseTypeEnum);
			//再根据类型来获取实际的实例
			var destTypeInstance = Convert.ChangeType(value, configType);
			//再序列化实例
			serializer.Serialize(writer, destTypeInstance);
		}

		public override IDatabaseConfig ReadJson(JsonReader reader, Type objectType, IDatabaseConfig existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			//先获取类型枚举字段的值
			var jObject = serializer.Deserialize(reader) as Newtonsoft.Json.Linq.JObject;
			if (jObject == null)
			{
				return null;
			}

			var databaseTypeEnumAsInt = jObject.Value<int>("DatabaseType");
			var databaseTypeEnum = (DatabaseTypeEnum)databaseTypeEnumAsInt;
			//再根据类型来获取实际的配置类型
			var configType = NormalDatabaseConfig.GetDatabaseConfigType(databaseTypeEnum);
			//再根据类型来反序列化实例
			var destTypeInstance = serializer.Deserialize(jObject.CreateReader(), configType);
			return destTypeInstance as IDatabaseConfig;
		}
	}
}