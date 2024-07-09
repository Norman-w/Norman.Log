# 说明

本项目是使用.net standard 2.0创建的.
通过ef core使用mysql.

使用Rider的Entity Framework Core->搭建DbContext->则可以从数据库创建实体类.

反之通过Rider的Entity Framework Core->添加迁移,更新数据库 则可以创建数据库.

再次添加迁移,更新数据库,则可以对数据库进行更改后以代码优先的方式更新数据库.

该项目需要一个Starter,也就是启动项目,引用这个项目 然后编译引用项目这个项目会自动编译.

引用项目可以是.net core 3.1以上版本的,比如当前我创建了一个 .net core 7的空项目就可以. .net6也测试没有问题.
