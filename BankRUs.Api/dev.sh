#!/bin/bash

# Start smtp4dev in the background
echo "Starting smtp4dev..."
smtp4dev &
SMTP4DEV_PID=$!

# Cleanup function to stop smtp4dev when the script exits
cleanup() {
    echo "Stopping smtp4dev..."
    kill $SMTP4DEV_PID 2>/dev/null
}
trap cleanup EXIT

# Run dotnet watch
echo "Starting dotnet watch..."
dotnet watch
