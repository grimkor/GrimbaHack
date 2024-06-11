#!/bin/bash

# Zip the GrimbaHack.dll and UniverseLib.IL2CPP.Interop.dll files into a zip file
rm  GrimbaHack.zip

cp bin/Debug/net6.0/publish/{GrimbaHack.dll,Twitch*,Newtonsoft.Json.dll} package/BepInEx/plugins
cp deps/UniverseLib.IL2CPP.Interop.dll package/BepInEx/plugins

cd package
zip -r ../GrimbaHack.zip BepInEx
