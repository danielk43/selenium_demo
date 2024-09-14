FROM debian:bookworm-slim

ARG UID=9876
ARG USER=sdk_user
ARG HOME=/data
ARG DEBIAN_FRONTEND=noninteractive
ARG SCREEN_HEIGHT=1080
ARG SCREEN_WIDTH=1920

ENV DISPLAY=:99
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_INTERACTIVE_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1
ENV LOGIN_USER=""
ENV LOGIN_PASS=""
ENV PATH="$PATH:/opt/chrome-linux64:/opt/chromedriver-linux64"
ENV SCREEN_HEIGHT=$SCREEN_HEIGHT
ENV SCREEN_WIDTH=$SCREEN_WIDTH

WORKDIR $HOME

COPY startup.sh startup.sh

RUN addgroup --gid ${UID} ${USER} \
 && adduser --home ${HOME} --shell /sbin/nologin \
    --ingroup ${USER} --uid ${UID} \
    --disabled-password ${USER} --gecos "" \
 && apt update \
 && apt -y upgrade \
 && apt -y install --no-install-recommends ca-certificates \
                                           curl \
                                           fonts-liberation \
                                           jq \
                                           libasound2 \
                                           libatk-bridge2.0-0 \
                                           libatk1.0-0 \
                                           libc6 \
                                           libcairo2 \
                                           libcups2 \
                                           libdbus-1-3 \
                                           libdrm2 \
                                           libexpat1 \
                                           libfontconfig1 \
                                           libgbm1 \
                                           libgcc1 \
                                           libglib2.0-0 \
                                           libgtk-3-0 \
                                           libnspr4 \
                                           libnss3 \
                                           libpango-1.0-0 \
                                           libpangocairo-1.0-0 \
                                           libstdc++6 \
                                           libx11-6 \
                                           libx11-xcb1 \
                                           libxcb1 \
                                           libxcomposite1 \
                                           libxcursor1 \
                                           libxdamage1 \
                                           libxext6 \
                                           libxfixes3 \
                                           libxi6 \
                                           libxkbcommon0 \
                                           libxrandr2 \
                                           libxrender1 \
                                           libxss1 \
                                           libxtst6 \
                                           lsb-release \
                                           unzip \
                                           wget \
                                           x11vnc \
                                           xdg-utils \
                                           xvfb \
 && mkdir /root/.vnc \
 && x11vnc -storepasswd 1234 /root/.vnc/passwd \
 && curl -LO https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb \
 && dpkg -i packages-microsoft-prod.deb \
 && apt update \
 && apt install -y dotnet-sdk-8.0 \
                   aspnetcore-runtime-8.0 \
 && dotnet dev-certs https -ep /usr/local/share/ca-certificates/aspnet/https.crt --format PEM \
 && update-ca-certificates \
 && CHROME_STABLE_BROWSER=$(curl -L https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json | jq -r '.channels.Stable.downloads.chrome[] | select(.platform == "linux64") | .url') \
 && CHROME_STABLE_DRIVER=$(curl -L https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json | jq -r '.channels.Stable.downloads.chromedriver[] | select(.platform == "linux64") | .url') \
 && curl -LO ${CHROME_STABLE_BROWSER} \
 && curl -LO ${CHROME_STABLE_DRIVER} \
 && unzip ${CHROME_STABLE_BROWSER##*/} -d /opt \
 && unzip ${CHROME_STABLE_DRIVER##*/} -d /opt \
 && chromedriver_var=$(LC_ALL=C tr -dc A-Za-z </dev/urandom | head -c 26)_ \
 && sed -i "s/cdc_.*_/${chromedriver_var}/g" /opt/chromedriver-linux64/chromedriver \
 && rm -rf /var/cache/apt/* *.deb *.zip \
 && chown -R ${USER}: ${HOME}

EXPOSE 5900

ENTRYPOINT ["/bin/bash", "startup.sh"]

CMD ["dotnet", "test", "--verbosity", "normal"]
