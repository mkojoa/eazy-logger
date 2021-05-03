# eazy-logger
[![made-with-csharp](https://img.shields.io/badge/csharp-1f425f?logo=c#)](https://microsoft.com/csharp)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Logging is an important part of modern application development, regardless of the platform targeted. This is why easy-logger is here to make log integration much easier.
This library aims to provide easy logging in .netcore ^3.1 web application.

![](https://vistr.dev/badge?repo=mkojoa.eazy-logge&color=0058AD)

## Available Channel Drivers
- [X] [Seq](#eazy-logging)
- [X] [File](#eazy-logging)
- [X] [Database](#eazy-logging)


## Fuel my efforts with a cup of Coffee.
<a href="https://www.buymeacoffee.com/mkojoa" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height:30px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;" ></a>

## Get Started

#### Installation (Not Yet)
    - Install-Package eazy-logger
#### eazy-logger
eazy-logger is based on `channels`. Each channel represents a specific way of writing application logs information
Use the channel property on the LoggerOption to enable or disable and log 
to any channel defined here - [appsettings]

###### Note : 
    - By default File logging is set to true with path Logs/logs.txt.

Add `UseEazyLogging()` to the web host builder in BuildWebHost().
```c#
  public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseEazyLogging()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```


 
> Once you have configured the `UseEazyLogging()` in the Program.cs file, 
> you're ready to define the `LoggerOptions` in the `app.settings.json`.

###### appsettings
 
```yaml
  "LoggerOptions": {
    "name": "eazy-api",
    "ignoredPaths": [ "logs", "/ping", "/metrics" ],
    "level": "information",
    "file": {
      "enabled": true,
      "path": "Logs/logs.txt",
      "interval": "day"
    },
    "seq": {
      "enabled": false,
      "url": "http://localhost:5341",
      "token": "secret"
    },
    "database": {
      "enabled": true,
      "name": "Db-Name",
      "table": "Logs",
      "instance": "SQL-Instant-Name",
      "userName": "sudo",
      "password": "root",
      "encrypt": "False",
      "trustedConnection": "False",
      "trustServerCertificate": "True",
      "integratedSecurity": "SSPI"
    }
  }
```
