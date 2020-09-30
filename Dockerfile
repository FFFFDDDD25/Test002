 FROM masteroleary/selenium-dotnetcore3.1-linux:v2 AS base

 WORKDIR /app

 FROM masteroleary/selenium-dotnetcore3.1-linux:v2 AS build WORKDIR /src

 COPY ["Test002.csproj", ""]

 RUN dotnet restore "Test002.csproj"

 COPY . .

 WORKDIR "/src/"

 RUN dotnet build "Test002.csproj" -c Prod -o /app

 FROM build AS publish

 RUN dotnet publish "Test002.csproj" -c Prod -o /app

 FROM base AS final

 WORKDIR /app

 COPY --from=publish /app .

 ENTRYPOINT ["dotnet", "Test002.dll"]