FROM debian:bookworm-slim

ARG UID=9876
ARG USER=sdk_user
ARG HOME=/data
ARG DEBIAN_FRONTEND=noninteractive
ARG SCREEN_HEIGHT=1080
ARG SCREEN_WIDTH=1920

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_INTERACTIVE_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1
ENV LOGIN_USER=""
ENV LOGIN_PASS=""
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
 && apt -y install --no-install-recommends ca-certificates chromium-driver curl xauth xvfb \
 && curl -LO https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb \
 && dpkg -i packages-microsoft-prod.deb \
 && apt update \
 && apt install -y dotnet-sdk-8.0 aspnetcore-runtime-8.0 \
 && dotnet dev-certs https -ep /usr/local/share/ca-certificates/aspnet.crt --format PEM \
 && update-ca-certificates \
 && chromedriver_var=$(LC_ALL=C tr -dc A-Za-z </dev/urandom | head -c 26)_ \
 && sed -i "s/cdc_.*_/${chromedriver_var}/g" /usr/bin/chromedriver \
 && rm -rf /var/cache/apt/* *.deb \
 && chown -R ${USER}: ${HOME}

ENTRYPOINT ["/bin/bash", "startup.sh"]

CMD ["xvfb-run", "dotnet", "test", "--verbosity", "normal"]
