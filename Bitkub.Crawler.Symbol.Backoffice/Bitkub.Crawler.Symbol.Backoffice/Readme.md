# Bitkub.Crawler.Symbol.Backoffice
Template Backoffice

### Runtime Required
* .NET 5.0 Runtime ([Download for Run console apps](https://dotnet.microsoft.com/download/dotnet/5.0/runtime))

### Gasxher.GISC Core
* [Download Git](https://gitlab.gisc.cdg.co.th/e21-5016.ptt-ngd/gasxher.gisc)


### Framework Required
* .NET Core Version 5.0.0+
* .NET Standard 2.0+

### Required Nuget package
* Microsoft.Extensions.Configuration 5.0.0
* Microsoft.Extensions.Configuration.Abstractions 5.0.0
* Microsoft.Extensions.Configuration.Binder 5.0.0
* Microsoft.Extensions.Configuration.FileExtensions 5.0.0
* Microsoft.Extensions.Configuration.Json  5.0.0
* Microsoft.Extensions.DependencyInjection 5.0.0
* Microsoft.Extensions.Http 5.0.0
* Microsoft.Extensions.Logging 5.0.0
* Microsoft.Extensions.Logging.Abstractions
* Microsoft.Extensions.Logging.Configuration
* Microsoft.Extensions.Options
* Microsoft.Extensions.Options.ConfigurationExtensions
* Newtonsoft.Json
* System.Data.SqlClient


### Logging default
> #### รองรับ variable 
> * %appname% = Configuration["Application:Name"]
> * %date% = 2021-08-26 (date now)
> * %longdate% = 20210826 (date now)
> * %day% = วันที่
> * %month% = เดือนที่
> * %year% = ปีที่
> * %format:date:yyyyMMddhhmmss% = รองรับเฉพาะ date format

> #### Setup configuration
> * Name@string = ชื่อ Provider ในการ Get Instance 
> * FileName@string = ชื่อ Logfile
> * FilePath@string = ตำแหน่งของ Logfile หากต้องการที่อยู่อื่นๆ ให้กำหนดเต็มเช่น D:/temp/logs หากให้อยู่ภายใน projects ให้เริ่มที่ logs ได้เลย เช่น FilePath = "logs" เป็นต้น
> * Running@bool = เพิ่ม Running number ในล็อกเพื่อนับจำนวนครั้งในการทำงาน เช่น app-20210826-01.log 
> * LogNamespace@bool = เปิดใช้ Namespace ของ Logger เพื่อดู handler เกิดจากที่ไหนหรือไม่ เช่น ถ้าเปิดใช้งานจะเขียนด้วยชื่อเต็ม Bitkub.Crawler.Symbol.Backoffice.FakeData.Application หากไม่จะเขียนแค่ Application เป็นต้น
> * Enable@bool = เปิดใช้การเขียนไฟล์
> * Console@bool = เปิดใช้การเขียนบน console app

```json
  "Logging": {
    "Providers": [
      {
        "Name": "System",
        "FileName": "sys-%longdate%.log",
        "FilePath": "C:/Windows/Temp",
        "Running": false,
        "LogNamespace": false,
        "Enable": true,
        "Console": false,
      },
      {
        "Name": "Database",
        "FileName": "db-%longdate%.txt",
        "FilePath": "../FilesLocalStorage/logs/db/%year%/%month%",
        "Running": false,
        "LogNamespace": false,
        "Enable": true,
        "Console": false
      },
      {
        "Name": "Application",
        "FileName": "app-%format:date:yy_MMMM_d%.txt",
        "FilePath": "../FilesLocalStorage/logs/%year%/%month%/%day%",
        "Running": true,
        "LogNamespace": false,
        "Enable": false,
        "Console": true
      }
    ],
  }
}
```


### Support Database 
> #### IDbDataAccess สำหรับการเชื่อมต่อฐานข้อมูลด้วย Driver
> * MSSQL Server -> System.Data.SqlCient
> * MySQL Server -> MySql.Data.MySqlClient -> ยังไม่ได้ทดสอบ
> * Oracle -> Oracle.DataAccess.Client -> ยังไม่ได้ทดสอบ
> * PostgreSQL -> Npgsql.NpgsqlConnection -> ยังไม่ได้ทดสอบ

```
{
  "Database": {
    "DataSourceParameter": "APP_DATA_SOURCE",
    "ProcedureParameter": "APP_DATA_PROCEDURE",
    "NotiOutputParameter": "APP_NOTI_OUTPUT",
    "UserIdProcedureParameter": "USER_ID_LOGIN",
    "DefaultDataSource": "SQLServer",
    "DataSource": {
      "Default": {
        "ConnectionString": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={server})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=MONITOR)));; Persist Security Info=True;",
        "UserId": "",
        "Password": "",
        "Provider": "Oracle"
      },
      "SQLServer": {
        "ConnectionString": "Data Source={server};persist security info=True;initial catalog=pttngddb;",
        "UserId": "",
        "Password": "",
        "Provider": "MSSQL"
      }
    }
  }
}
```


### Deployment application
* คำสั่งสำหรับ Publish Application เพิ่มการกำหนด EnvironmentName ได้ดังนี้ Development, Inhouse, NonProduction, Production
``` dotnet
P1 dotnet publish
P2 dotnet publish --configuration Release /p:EnvironmentName=Development
P3 dotnet publish --configuration Release /p:EnvironmentName=Inhouse
P4 dotnet publish --configuration Release /p:EnvironmentName=NonProduction
P5 dotnet publish --configuration Release /p:EnvironmentName=Production
```
#### หมายเหตุ: การรันแบบ P1 จะได้ Environment Default ซึ่งกำหนดเป็น Development ให้

#### สำหรับใครที่ไม่ได้รับ File run-script.bat ภายใน Folder publish
* เพิ่ม Scripting MSBuild ส่วนนี้ใน File *.csproj สำหรับช่วย Generate run-script.bat สำหรับ Environment ต่าง ๆ

### หมายเหตุ: เมื่อ Publish เสร็จจะเกิด Event AfterTargets="Publish" เกิดขึ้นสคริปต์ด้านล่างจะช่วย Trigger การเขียนไฟล์เพื่อใช้ในการรันครับ
```xml
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>
  ...

 <Target Name="PostPublish_1" AfterTargets="Publish">
    <PropertyGroup >
      <BatchScript>$(MSBuildProjectDirectory)\$(PublishDir)run-script.bat</BatchScript>
    </PropertyGroup>
  </Target>

  <Target Name="PostPublish_2" AfterTargets="Publish">
    <Message Importance="high" Text="Create Batch File..." />
  </Target>

  <Target Name="PostPublish_3" AfterTargets="Publish">
    <Message Importance="high" Text="Environment: Development" Condition="'$(EnvironmentName)' == '' and '$(ASPNETCORE_ENVIRONMENT)' == '' and '$(EnvironmentVariable)' == ''" />
    <Message Importance="high" Text="Environment: $(EnvironmentName)" Condition="'$(EnvironmentName)' != ''" />
    <Message Importance="high" Text="Environment: $(ASPNETCORE_ENVIRONMENT)" Condition="'$(ASPNETCORE_ENVIRONMENT)' != ''" />
    <Message Importance="high" Text="Environment: $(EnvironmentVariable)" Condition="'$(EnvironmentVariable)' != ''" />
  </Target>

  <Target Name="PostPublish_4" AfterTargets="Publish">
    <Message Importance="high" Text="Write file..." />
    <Delete Files="$(BatchScript)" />
    <Exec Command="ECHO @echo off &gt;&gt; $(BatchScript)" />
    <Exec Command="ECHO REM force to drive to location path &gt;&gt; $(BatchScript)" />
    <Exec Command="ECHO cd /D %%~dp0 &gt;&gt; $(BatchScript)" />
    <Exec Command="ECHO REM run program &gt;&gt; $(BatchScript)" />
    <Message Importance="high" Text="$(BatchScript) Success..." />
  </Target>

  <Target Name="PostPublish_5" AfterTargets="Publish">
    <Exec Command="ECHO call $(MSBuildProjectName).exe --EnvironmentName=Development >> $(BatchScript)"
      Condition="'$(EnvironmentName)' == '' and '$(ASPNETCORE_ENVIRONMENT)' == '' and '$(EnvironmentVariable)' == ''" />

    <Exec Command="ECHO call $(MSBuildProjectName).exe --EnvironmentName=$(EnvironmentName) >> $(BatchScript)"
      Condition="'$(EnvironmentName)' != ''" />

    <Exec Command="ECHO call $(MSBuildProjectName).exe --EnvironmentName=$(ASPNETCORE_ENVIRONMENT) >> $(BatchScript)"
      Condition="'$(ASPNETCORE_ENVIRONMENT)' != ''" />

    <Exec Command="ECHO call $(MSBuildProjectName).exe --EnvironmentName=$(EnvironmentVariable) >> $(BatchScript)"
      Condition="'$(EnvironmentVariable)' != ''" />
  </Target>

  <Target Name="PostPublish_6" AfterTargets="Publish">
    <Message Importance="high" Text="Finish..." />
  </Target>
 ```

### GasxherGIS.Core
> Core สำหรับ Console application
> Author: 006006