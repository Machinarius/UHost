FROM microsoft/aspnetcore:1.1
WORKDIR /app
EXPOSE 5000
COPY ./publish .
ENV ASPNETCORE_URLS="http://*:5000"
ENV ASPNETCORE_ENVIRONMENT="Staging"
ENTRYPOINT ["dotnet", "UHost.dll"]
