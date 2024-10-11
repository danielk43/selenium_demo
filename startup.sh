#!/usr/bin/env bash

# Write to mounted volume
# TODO: better way
chown -R sdk_user: /data

echo "CMD=${@}"

# Print Chrome version
chromium --version

# Compile and run NUnit tests
# Or pass non-default cmds as args
runuser -u sdk_user -- "${@}"
