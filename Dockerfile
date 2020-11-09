
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore


# Copy everything else and build
COPY . ./




RUN dotnet publish -c Release -o out


CMD ls
RUN sleep 10
CMD ls
RUN sleep 11
CMD ls
RUN sleep 12
CMD ls
RUN sleep 13
CMD ls
RUN sleep 14
CMD ls
RUN sleep 15
CMD ls
RUN sleep 16
                            

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime


                        ####### Install the latest versions of Google Chrome and Chromedriver:
                        RUN export DEBIAN_FRONTEND=noninteractive \
                        && apt-get update \
                        && apt-get install --no-install-recommends --no-install-suggests -y \
                            unzip \
                            gnupg \
                        && GOOGLE_LINUX_DL=https://dl.google.com/linux \
                        && curl -sL "$GOOGLE_LINUX_DL/linux_signing_key.pub" | apt-key add - \
                        && curl -sL "$GOOGLE_LINUX_DL/direct/google-chrome-stable_current_amd64.deb" \
                            > /tmp/chrome.deb \
                        && apt install --no-install-recommends --no-install-suggests -y \
                            /tmp/chrome.deb \
                        && CHROMIUM_FLAGS='--no-sandbox --disable-dev-shm-usage' \
                        ######## Patch Chrome launch script and append CHROMIUM_FLAGS to the last line:
                        && sed -i '${s/$/'" $CHROMIUM_FLAGS"'/}' /opt/google/chrome/google-chrome \
                        && BASE_URL=https://chromedriver.storage.googleapis.com \
                        && VERSION=$(curl -sL "$BASE_URL/LATEST_RELEASE") \
                        && curl -sL "$BASE_URL/$VERSION/chromedriver_linux64.zip" -o /tmp/driver.zip \
                        && unzip /tmp/driver.zip \
                        && chmod 755 chromedriver \
                        && mv chromedriver /usr/local/bin/ \
                        ######## Remove obsolete files:
                        && apt-get autoremove --purge -y \
                            unzip \
                            gnupg \
                        && apt-get clean \
                        && rm -rf \
                            /tmp/* \
                            /usr/share/doc/* \
                            /var/cache/* \
                            /var/lib/apt/lists/* \
                            /var/tmp/*

########
ENV ASPNETCORE_URLS=http://+:8080 
########



WORKDIR /app



########
EXPOSE 8080/tcp
########




COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Test002.dll"]