# eazy-logger
Logging is a very critical and essential part of any software. This is why easy-logger is here to make log integration much easier.
This library aims to provide easy logging in .netcore web application.

![](https://vistr.dev/badge?repo=mkojoa.eazy-logge&color=0058AD)

## Available Channel Drivers
- [X] [Seq](#eazy-logging)
- [X] [File](#eazy-logging)
- [X] [Database](#eazy-logging)

## Library dependencies
- [X] [Serilog](#Serilog)
- [X] [Microsoft.Extensions.Logging](#Microsoft.Extensions.Logging) 
- [X] [Microsoft.Extensions](#Microsoft.Extensions) 
- [X] [Microsoft.AspNetCore](#Microsoft.AspNetCore)


## Get Started


#### eazy-logger
Sometimes you may wish to log a message to a channel. 
You may use the channel property on the LoggerOption to enable or disable and log to any channel defined ([appsettings](http://example.com)):

###### Note : 
    - By default File logging is set to `true` with path `Logs/logs.txt`.

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
> you're ready to define the `LoggerOptions` in the appsettings.

 `app.settings.json` file
 
```yaml
   "LoggerOptions": {
      "applicationName": "eazy-logging-service",
      "excludePaths": [ "/ping", "/metrics" ],
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
        "name": "DatabaseName",
        "table": "TableName",
        "instance": "SqlInstanceName",
        "userName": "root",
        "password": "root@123",
        "encrypt": "False",
        "trustedConnection": "False",
        "trustServerCertificate": "True",
        "integratedSecurity": "SSPI"
      }
  }
```
