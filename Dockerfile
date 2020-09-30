

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build



WORKDIR app


CMD touch a06txt


# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore
# copy everything else and build app
COPY . ./
CMD touch a04txt




RUN dotnet publish -c Release -o out
CMD touch a05txt


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
ENV ASPNETCORE_URLS=http://+:8080



WORKDIR /app
CMD touch a07txt

EXPOSE 8080/tcp


CMD ls /app/out
COPY --from=build /app/out .
CMD ls /app/out
CMD sleep 500


CMD touch a01txt
CMD touch /app/a02txt
CMD touch /app/out/a03txt
ENTRYPOINT ["dotnet", "Test002.dll"]

