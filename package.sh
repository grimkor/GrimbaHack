#!/bin/bash

# Zip the GrimbaHack.dll and UniverseLib.IL2CPP.Interop.dll files into a zip file
rm GrimbaHack.zip

zip -j GrimbaHack.zip bin/Debug/net6.0/publish/GrimbaHack.dll \
 bin/Debug/net6.0/publish/Twitch* \
 bin/Debug/net6.0/publish/Newtonsoft.Json.dll \
 deps/UniverseLib.IL2CPP.Interop.dll
