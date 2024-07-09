# 说明

本项目中存储ef core生成的数据层代码(DAL).

`下面假设有一个启动项目,有一个ef的独立项目`

### 这是如何创建的?
创建一个.net7的类库项目

添加nuget包:
#### 在本项目中
* Pomelo.EntityFrameworkCore.MySql 7.0.0
  * 这是在Rider中使用ef core工具时,搭建DbContext的必要包,用于作为提供程序

#### 在启动项目中
* Microsoft.EntityFrameworkCore.Design 7.0.0
  * 这是在Rider中使用ef core工具时,搭建DbContext的必要包,这样就可以作为启动程序

在Norman.Log.Component.DatabaseWriter中添加对本项目的引用

build启动者项目,也就是Norman.Log.Component.DatabaseWriter,会自动build本项目

在rider中使用ef工具生成DbContext:

项目右键(或工具->EF Core->Add DbContext)
搭建DbContext
* 连接:设置连接的connection string,如`server=localhost;port=3306;database=norman.log;user=root;password=`
* 提供程序:选择`Pomelo.EntityFrameworkCore.MySql`
* 迁移项目:选择`Norman.Log.Component.Database.Mysql`项目
* 启动项目:选择`Norman.Log.Component.DatabaseWriter`项目
其他都保持默认即可

如果生成有using的错误,删掉没用的using就行了.

编译项目,为了保险,每次都编译启动项目时我的常用做法,而不是单独编译本项目.


### 如何使用?

#### 一,初始化数据库

##### 添加一个基础的迁移,用于同步代码到数据库
在ef core菜单中,选择 添加迁移,输入迁移名称,确认迁移项目和启动项目,确定即可
会在迁移目录下生成迁移时候的代码,这个时候还没有同步到数据库,只是生成了迁移文件.

##### 同步代码到数据库
在ef core菜单中,选择 更新数据库,选择迁移项目和启动项目,确定即可
这个时候在Context文件夹下的NormanLogContextModelSnapshot.cs文件中会生成数据库的结构,这个时候数据库中就有了对应的表.
但是如果这个类中没有对应的表,那么数据库中的数据就会被删除,所以在更新数据库之前,一定要确认这个类中的表是否正确.

⚠️注意:在更新数据库之前,一定要确认这个类中的表是否正确,否则会导致数据丢失.
⚠️注意:在更新数据库之前,一定要确认这个类中的表是否正确,否则会导致数据丢失.
⚠️注意:在更新数据库之前,一定要确认这个类中的表是否正确,否则会导致数据丢失.




#### 二,更新数据库/迁移/修改表结构 等

添加迁移:
在ef core菜单中,选择 添加迁移,输入迁移名称,确认迁移项目和启动项目,确定即可
会在迁移目录下生成迁移文件.
具体迁移的时候怎么做,可以修改迁移文件,比如我现在有一个20240705120446_重命名Log表中的字段.cs文件,里面的代码如下:
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Norman.Log.Component.Database.Mysql.Imigration
{
    /// <inheritdoc />
    public partial class 重命名Log表中的字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //这个方法是在迁移时调用的,将Log表中的LogType字段重命名为Type,将LogLevel字段重命名为Level 
            migrationBuilder.RenameColumn(
                name: "LogType",
                table: "Logs",
                newName: "Type");
            migrationBuilder.RenameColumn(
                name: "LogLevel",
                table: "Logs",
                newName: "Level");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //这个方法是在回滚迁移时调用的
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Logs",
                newName: "LogType");
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Logs",
                newName: "LogLevel");
        }
    }
}
```

这个时候,我就可以在这个文件中修改表结构,比如我要将Log表中的LogType字段重命名为Type,将LogLevel字段重命名为Level.
到这一步只是生成了迁移文件,还没有同步到数据库.

同步代码到数据库:
在ef core菜单中,选择 更新数据库,选择迁移项目和启动项目,确定即可


### 备注

1. 实际上不需要单独的配置一个启动项目, 本项目本身添加了Microsoft.EntityFrameworkCore.Design包的话就可以在ef工具中选择本项目作为启动项目.

2. 如果项目本身可以使用.net core的 ef作为启动项目,则本项目的.net版本必须要大于3.1, 参考连接: https://plugins.jetbrains.com/plugin/18147-entity-framework-core-ui/f-a-q#why-i-cant-see-my-project-in-a-startup-projects-field


### 关于.net Standard 2.0项目的ef core使用:

由于ef core必须要使用.net core 3.1以上的项目作为启动项目,
所以如果ef的DbContext所在项目如希望是.net standard 2.0的话,
则需要在项目中添加一个.net core 3.1以上的项目作为启动项目,
并且在这个项目中添加对ef的项目(.net Standard)的引用,这样就可以使用ef core工具了.

也就是说
DbContext 所在项目(比如创建于.net standard 2.0 from .net core 6)需要安装 
* Pomelo.EntityFrameworkCore.MySql 3.1.2(对.net standard 2.0支持的最后一个版本)
* NetStandard.Library 2.0.3 (自动添加的,不需要手动添加)

启动项目(比如创建于.net core 6)需要安装
* Microsoft.EntityFrameworkCore.Design 3.1.2 (与DbContext所在项目的ef core版本一致)
* Pomelo.EntityFrameworkCore.MySql 3.1.2 (与DbContext所在项目的ef core版本一致)

使用.net standard 2.0 作为迁移项目和使用.net core 6作为启动项目的方案在执行搭建DbContext时,是可以正常使用的,而且默认迁移也会自动创建.

整体使用过程比较繁琐,但是可以正常使用,不过不推荐使用.net standard 2.0作为ef core的DbContext所在项目,因为.net standard 2.0的ef core版本已经不再维护,可能会有一些问题.

但是在一些希望更好的兼容性的项目中,可能会使用.net standard 2.0作为ef core的DbContext所在项目,这个时候就需要使用上面的方案了.

`启动项目可以就是一个空壳,目的是为了让.net standard 2.0的ef core项目可以使用ef core工具生成DbContext,并且可以正常使用,且作为其他项目的引用.`