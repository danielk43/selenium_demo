#!/usr/bin/env bash

rm -f /tmp/.X0-lock

# Run Xvfb on display 99
Xvfb :99 -screen 0 1920x1080x16 &

# Run x11vnc on display 99
x11vnc -display :99 -forever -ncache 10 -usepw &

sleep 1

# Write to mounted volume
# TODO: better way
chown -R sdk_user: /data

echo "CMD=${@}"

# Print Chrome version
chromium --version

# Compile and run NUnit tests
# Or pass non-default cmds as args
runuser -u sdk_user -- "${@}"
