#!/bin/sh

#export PLUGIN_DIR=/home/david/.local/share/Steam/steamapps/common/Power\ Rangers\ Battle\ for\ the\ Grid/BepInEx/plugins
PLUGIN_DIR=/mnt/storage/SteamLibrary/steamapps/common/Power\ Rangers\ Battle\ for\ the\ Grid/BepInEx/plugins

dotnet build && \
cp bin/Debug/net6.0/GrimbaHack.dll "$PLUGIN_DIR" -v && \
cp deps/UniverseLib.IL2CPP.Interop.dll "$PLUGIN_DIR" -v
