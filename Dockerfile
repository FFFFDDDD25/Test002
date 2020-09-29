

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build



WORKDIR app




# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore
# copy everything else and build app
COPY . ./
RUN ls  | grep files




RUN dotnet publish -c Release -o out
RUN ls  | grep files


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
RUN ls  | grep files
EXPOSE 8080/tcp
RUN ls | grep files
COPY --from=build /app/out .
RUN ls  | grep files
ENTRYPOINT ["dotnet", "Test002.dll"]
RUN ls  | grep files
RUN sleep 500