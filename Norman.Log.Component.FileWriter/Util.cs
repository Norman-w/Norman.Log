using System;
using System.IO;
using Norman.Log.Config;

namespace Norman.Log.Component.FileWriter
{
	public static class Util
	{
		/// <summary>
		/// 计算日志文件名和文件夹名
		/// </summary>
		/// <param name="loggerName"></param>
		/// <param name="time"></param>
		/// <param name="config"></param>
		/// <param name="fileName"></param>
		/// <param name="folderName"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		internal static void CalcLogFileAndFolderName(string loggerName, DateTime time,
			LogToFileConfig config,
			out string fileName, out string folderName)
		{
			var yearString = time.Year.ToString();
			var monthString = time.Month.ToString().PadLeft(2, '0');
			var dayString = time.Day.ToString().PadLeft(2, '0');
			//文件名字的时间部分
			string timeOfFileString;
			switch (config.CreateFolderRule)
			{
				case LogToFileConfig.CreateFolderRuleEnum.YearFolderMonthFolderDayFolder:
					folderName = Path.Combine(yearString, monthString, dayString);
					timeOfFileString = time.Hour.ToString().PadLeft(2, '0');
					break;
				case LogToFileConfig.CreateFolderRuleEnum.YearFolderMonthDayFolder:
					folderName = Path.Combine(yearString, monthString + dayString);
					timeOfFileString = time.Hour.ToString().PadLeft(2, '0');
					break;
				case LogToFileConfig.CreateFolderRuleEnum.YearMonthFolderDayFolder:
					folderName = Path.Combine(yearString + monthString, dayString);
					timeOfFileString = time.Hour.ToString().PadLeft(2, '0');
					break;
				case LogToFileConfig.CreateFolderRuleEnum.YearMonthDayFolder:
					folderName = Path.Combine(yearString + monthString + dayString);
					timeOfFileString = time.Hour.ToString().PadLeft(2, '0');
					break;
				case LogToFileConfig.CreateFolderRuleEnum.YearFolderMonthFolder:
					folderName = Path.Combine(yearString, monthString);
					timeOfFileString = dayString;
					break;
				case LogToFileConfig.CreateFolderRuleEnum.YearFolderMonthDay:
					folderName = Path.Combine(yearString, monthString + dayString);
					timeOfFileString = time.Hour.ToString();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			switch (config.CreateFileNameRule)
			{
				case LogToFileConfig.CreateFileNameRuleEnum.LoggerNameAndTime:
					fileName =
						$"{loggerName}_{timeOfFileString}.{Model.Constant.DefaultLogFileExtension}";
					break;
				case LogToFileConfig.CreateFileNameRuleEnum.LoggerName:
					fileName = $"{loggerName}.{Model.Constant.DefaultLogFileExtension}";
					break;
				case LogToFileConfig.CreateFileNameRuleEnum.Time:
					fileName = $"{timeOfFileString}.{Model.Constant.DefaultLogFileExtension}";
					break;
				case LogToFileConfig.CreateFileNameRuleEnum.TimeAndLoggerName:
					fileName =
						$"{timeOfFileString}_{loggerName}.{Model.Constant.DefaultLogFileExtension}";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}