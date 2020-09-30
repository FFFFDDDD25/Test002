 FROM mcr.microsoft.com/dotnet/core/sdk:3.1-stretch

 # Install Chrome
 RUN apt-get update && apt-get install -y \
 apt-transport-https \
 ca-certificates \
 curl \
 gnupg \
 hicolor-icon-theme \
 libcanberra-gtk* \
 libgl1-mesa-dri \
 libgl1-mesa-glx \
 libpango1.0-0 \
 libpulse0 \
 libv4l-0 \
 fonts-symbola \
 --no-install-recommends \
 && curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
 && echo "deb [arch=amd64] https://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list \
 && apt-get update && apt-get install -y \
 google-chrome-stable \
 --no-install-recommends \
 && apt-get purge --auto-remove -y curl \
 && rm -rf /var/lib/apt/lists/*
 
 
 FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Test002.dll"]