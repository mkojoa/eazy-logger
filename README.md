# simple-logger
This library aims to provide easy way of logging in .netcore web application.

![](https://vistr.dev/badge?repo=mkojoa.simple-logge&color=0058AD)

## Features
- [X] [simple-logging](#simple-logging)

## Library dependencies
- [X] [Serilog](#Serilog)
- [X] [Microsoft.Extensions.Logging](#Microsoft.Extensions.Logging) 
- [X] [Microsoft.Extensions](#Microsoft.Extensions) 
- [X] [Microsoft.AspNetCore](#Microsoft.AspNetCore)


## Get Started


#### simple-logging
simple-logging provides File Log and Seq Log options. See logging configuration below. `UseSimpleLogging()` must be used in Program.cs. By default File logging is set to `true`.
```c#
  public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSimpleLogging()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```

 `app.settings.json` file
 
```yaml
   "LoggerOptions": {
    "applicationName": "demo-service",
    "excludePaths": [ "/ping", "/metrics" ],
    "level": "information",
    "file": {
      "enabled": true,
      "path": "Logs/logs.txt",
      "interval": "day"
    },
    "seq": {
      "enabled": true,
      "url": "http://localhost:5341",
      "token": "secret"
    }
  }
```
