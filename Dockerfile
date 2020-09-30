FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY Test002/*.csproj ./Test002/
RUN dotnet restore

# copy everything else and build app
COPY Test002/. ./Test002/
WORKDIR /app/Test002
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/Test002/out ./
ENTRYPOINT ["dotnet", "Test002.dll"]