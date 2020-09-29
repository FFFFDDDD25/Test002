

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build



WORKDIR app




# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore
# copy everything else and build app
COPY . ./
CMD touch a04.txt




RUN dotnet publish -c Release -o out
CMD touch a05.txt


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
ENV ASPNETCORE_URLS=http://+:8080



WORKDIR /app
EXPOSE 8080/tcp
COPY --from=build /app/out .
CMD touch a01.txt
CMD touch /app/a02.txt
CMD touch /app/out/a03.txt
ENTRYPOINT ["dotnet", "Test002.dll"]

