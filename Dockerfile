

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build



WORKDIR app




# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore


RUN ls
# copy everything else and build app
COPY . ./
RUN ls files


RUN sleep 500




RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
EXPOSE 8080/tcp
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Test002.dll"]
