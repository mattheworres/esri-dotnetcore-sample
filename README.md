# ESRI Dotnet Core Sample App

Sample dotnet app for ESRI purposes of debugging proxy code running on a dotnetcore server.

#### Prerequisites

- [.NET Core 2.2 SDK installed locally](https://dotnet.microsoft.com/download/dotnet-core/2.2)

###Running the app

1. Clone repository
2. Using terminal/command prompt, change into base directory of cloned repository
3. Run `dotnet restore`
4. Run `dotnet run` which will start a new API at `localhost:5050`
5. For purposes of this repo, create a proxy rule in your ArcGIS SDK of choice with the `proxyUrl` of `http://localhost:5050/proxy/proxy.ashx`

##FIN
