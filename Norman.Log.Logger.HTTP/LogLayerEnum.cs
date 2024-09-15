namespace Norman.Log.Logger.HTTP
{
    /// <summary>
    /// 日志层枚举
    /// </summary>
    public enum LogLayerEnum
    {
        /// <summary>
        /// 未知,无法描述的,不知道属于哪一层的都可以临时的使用这个
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 系统层,比如应用程序的启动,关闭,配置等
        /// </summary>
        System = 1,

        /// <summary>
        /// 业务层,比如一些正常的逻辑,像用户登录,注册,购买等
        /// </summary>
        Business = 2,

        /// <summary>
        /// 数据层,比如数据库的操作,文件的读写等
        /// </summary>
        Data = 3,

        /// <summary>
        /// 一些对外提供的服务如API,WebSocket,gRPC等
        /// 或者是看门狗服务,定时器服务,消息队列服务等
        /// </summary>
        Service = 4,

        /// <summary>
        /// 控制器层,比如MVC的控制器,WebAPI的控制器,防抖动控制器,定时器控制器等
        /// </summary>
        Controller = 5,

        /// <summary>
        /// 外设层,比如一些外设的操作,比如打印机,扫描仪,摄像头等
        /// 或者是一些业务上的外设,如调用第三方的api,邮件,短信,或者什么Encompass,Surge,各种外部SDK等
        /// </summary>
        Peripheral = 6
    }
}