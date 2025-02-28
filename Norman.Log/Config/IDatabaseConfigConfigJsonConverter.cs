using System;
using Newtonsoft.Json;

namespace Norman.Log.Config
{
	/// <summary>
	/// 用于IDatabaseConfig的JsonConverter,由于我们在序列化和反序列化的时候无法使用接口,所以需要一个JsonConverter来帮助我们先判断类型,再序列化和反序列化
	/// 至于为什么使用接口定义属性,是因为这样我们在使用时方便通过 if(instance is XXX)来判断类型
	/// </summary>
	// ReSharper disable once InconsistentNaming
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
			//再序列化实例,
			
			// 这里会递归的 要改进一下...需要用writer手动写入,不能直接使用serializer.Serialize
			// serializer.Serialize(writer, destTypeInstance);
			//当前层的所有字段都写入
			writer.WriteStartObject();
			foreach (var property in configType.GetProperties())
			{
				writer.WritePropertyName(property.Name);
				serializer.Serialize(writer, property.GetValue(destTypeInstance));
			}
			writer.WriteEndObject();
		}

		public override IDatabaseConfig ReadJson(JsonReader reader, Type objectType, IDatabaseConfig existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			/*
			 
			 
			 注意这里不能使用JObject.Load(reader)来读取,因为本身当前这个函数就是在读取的过程中调用的
			 如果还使用JObject的话,则会导致递归调用.我们只能通过reader一点一点的读取才不会触发Attribute上的标记和默认的JsonConvert行为
			
			*/
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.PropertyName &&
				    reader.Value?.ToString() == nameof(IDatabaseConfig.DatabaseType))
				{
					reader.Read();
					// var databaseTypeEnumAsInt = (int)reader.Value;
					var databaseTypeEnumAsInt = Convert.ToInt32(reader.Value);
					var databaseTypeEnum = (DatabaseTypeEnum)databaseTypeEnumAsInt;
					//再根据类型来获取实际的配置类型
					var configType = NormalDatabaseConfig.GetDatabaseConfigType(databaseTypeEnum);
					//再根据类型来反序列化实例
					var destTypeInstance = serializer.Deserialize(reader, configType);
					return destTypeInstance as IDatabaseConfig;
				}

				if (reader.TokenType == JsonToken.EndObject)
				{
					break;
				}
			}

			return null;
		}
	}
}