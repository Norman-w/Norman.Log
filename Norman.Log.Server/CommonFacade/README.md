# 说明

外部组件/业务/客户端/网页等通过这里与日志服务器联通.

## 网络
Net.cs中定义的是asp.net为核心的一些业务,如果以后有tcp的业务,可以在这里添加.
目前Net facade支持http,ws,grpc,当有对应来源的请求,或者有外发的请求,都会通过这里进行处理,根据消息的类型进行不同模块的消息分发

Net可以根据具体的配置来决定是否开启哪些服务.

#### RESTful目录
存放的都是http的请求处理,每个文件都是一个独立的请求处理器,根据请求的url,会调用对应的处理器进行处理,也就是跟MVC中的Controller类似.
支持swagger,可以通过swagger-ui查看所有的请求,可通过localhost:端口号/swagger/访问
#### Grpc
##### Proto
存放的是标准的grpc的proto文件,用于生成grpc的代码
##### Model
存放的是grpc的数据模型,本项目使用的是代码优先模式,所以要先定义数据模型,然后生成proto文件


## 消息管道
NamedPipe因为不是使用net请求的,所以没有定义在net中,属于消息总线.


## 内部调用
Internal则就是直接使用dll引用等方式的调用,不通过网络请求,直接调用的方式.
