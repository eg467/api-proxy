# Api Proxy
A .NET web API that connects to Pocket. 
It exists to bypass CORS and token security issues from accessing it directly from JS.

# Instructions

1) Create a [Pocket account](https://getpocket.com/developer/)

2) Create `<root application directory>/AppSettings.config` with the following:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
  <add key="PocketConsumerKey" value="<YOUR CONSUMER KEY FROM GETPOCKET.COM>" />
</appSettings>
```

3) Build the solution using VS.