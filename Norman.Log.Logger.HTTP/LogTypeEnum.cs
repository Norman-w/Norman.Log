namespace Norman.Log.Logger.HTTP
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum LogTypeEnum
    {
        /// <summary>
        /// 日常信息,一些正常的信息
        /// </summary>
        Info = 1,

        /// <summary>
        /// 调试
        /// </summary>
        Debug = 2,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 3,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 4,

        /// <summary>
        /// 失败
        /// </summary>
        Fail = 5,

        /// <summary>
        /// 警告
        /// </summary>
        Warning = 6,

        /// <summary>
        /// 开始
        /// </summary>
        Start = 7,

        /// <summary>
        /// 结束
        /// </summary>
        Finish = 8,

        /// <summary>
        /// Bug
        /// </summary>
        Bug = 9,

        /// <summary>
        /// 模拟
        /// </summary>
        Simulate = 10
    }
}