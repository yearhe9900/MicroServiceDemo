FROM microsoft/aspnetcore:2.0

COPY bin/Release/netcoreapp2.0/publish  /app/

WORKDIR /app

EXPOSE 9991

ENTRYPOINT ["dotnet", "/app/MD.CalendarAlert.Service.dll"]