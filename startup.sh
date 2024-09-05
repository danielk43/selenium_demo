#!/usr/bin/env bash

rm -f /tmp/.X0-lock

# Run Xvfb on display 99
Xvfb :99 -screen 0 1920x1080x16 &

# Run x11vnc on display 99
x11vnc -display :99 -forever -ncache 10 -usepw &

sleep 1

# Compile and run NUnit tests
# Or pass non-default cmds as args
chown -R sdk_user: /data
echo "CMD=${@}"
runuser -u sdk_user -- "${@}"
