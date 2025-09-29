#!/usr/bin/env bash
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
sqlite3 /tmp/chirp.db < "$SCRIPT_DIR/../data/schema.sql"
sqlite3 /tmp/chirp.db < "$SCRIPT_DIR/../data/dump.sql"
