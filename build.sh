#!/bin/sh

export PLUGIN_DIR=/home/david/.local/share/Steam/steamapps/common/Power\ Rangers\ Battle\ for\ the\ Grid/BepInEx/plugins
#PLUGIN_DIR=/mnt/storage/SteamLibrary/steamapps/common/Power\ Rangers\ Battle\ for\ the\ Grid/BepInEx/plugins
rm -rf bin/Debug
dotnet publish && \
cp bin/Debug/net6.0/publish/GrimbaHack.dll "$PLUGIN_DIR" -v && \
cp bin/Debug/net6.0/publish/Twitch* "$PLUGIN_DIR" -v && \
cp bin/Debug/net6.0/publish/Newtonsoft.Json.dll "$PLUGIN_DIR" -v && \
cp deps/UniverseLib.IL2CPP.Interop.dll "$PLUGIN_DIR" -v
