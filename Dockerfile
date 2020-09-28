FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY published/Test002.dll ./
ENTRYPOINT ["dotnet", "Test002.dll"]