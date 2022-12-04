# Asp.Net Core WebSockets Vs SignalR

In this project we build 2 separate chat applications, one using Asp.Net Core WebSockets and the other using SignalR, allowing us to compare approaches and decide on which one works best. In both cases we build them with C#, .NET Core and JavaScript. We’ll also learn about:

- .NET Core Request Pipeline
- Request Delegates
- Asynchronous Programming in .NET (Async / Await)
- Introduction to Dependency Injection 

## SignalR Client Server

### Create project

<img src="/pictures/create_blazor_app.png" title="create blazor app"  width="400">

### Add Nuget Packages
```
Install-Package Microsoft.AspNetCore.SignalR.Client
```



