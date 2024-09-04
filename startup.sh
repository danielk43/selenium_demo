#!/usr/bin/env bash

chown -R sdk_user: /data
exec runuser -u sdk_user "$@"
