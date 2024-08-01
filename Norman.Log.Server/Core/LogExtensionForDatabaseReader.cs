using Newtonsoft.Json;
using Norman.Log.Model;

namespace Norman.Log.Server.Core;

public static class LogExtensionForDatabaseReader
{
	/// <summary>
	/// 从数据库模型转换为业务模型
	/// </summary>
	/// <param name="record"></param>
	/// <returns></returns>
	public static Log.Model.Log ToBllModel(this Component.Database.Mysql.Context.Log record)
	{
		return new Log.Model.Log(record.LoggerName)
		{
			Id = record.Id,
			CreateTime = record.CreateTime,
			Type = LogType.FromValue((uint)record.Type),
			Layer = LogLayer.FromValue((uint)record.Layer),
			Module = record.Module,
			Summary = record.Summary,
			Detail = record.Detail,
			LogContext = string.IsNullOrEmpty(record.Context) ? null : JsonConvert.DeserializeObject<Log.Model.Log.Context>(record.Context)
		};
	}
}