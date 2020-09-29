

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build



WORKDIR app




# copy csproj and restore as distinct layers
RUN ls files | grep chrome
COPY *.csproj ./
RUN dotnet restore
RUN ls files | grep chrome
# copy everything else and build app
COPY . ./
RUN ls files | grep chrome




RUN dotnet publish -c Release -o out
RUN ls files | grep chrome


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
RUN ls files | grep chrome
ENV ASPNETCORE_URLS=http://+:8080
RUN ls files | grep chrome
WORKDIR /app
RUN ls files | grep chrome
EXPOSE 8080/tcp
RUN ls | grep files
COPY --from=build /app/out .
RUN ls files | grep chrome
ENTRYPOINT ["dotnet", "Test002.dll"]
RUN ls files | grep chrome
RUN sleep 500