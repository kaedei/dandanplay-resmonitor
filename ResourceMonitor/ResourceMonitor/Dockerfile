FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY . /app
EXPOSE 80
ENTRYPOINT ["dotnet", "ResourceMonitor.dll"]