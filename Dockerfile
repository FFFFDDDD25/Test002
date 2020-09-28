FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /app/publisZ .
ENTRYPOINT ["dotnet", "Test002.dll"]