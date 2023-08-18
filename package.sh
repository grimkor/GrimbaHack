#!/bin/bash

# Zip the GrimbaHack.dll and UniverseLib.IL2CPP.Interop.dll files into a zip file
rm -r GrimbaHack.zip .build

mkdir -p textures \
 textures/s01_green \
 textures/s01_greenv2 \
 textures/s21_yellow \
 textures/s01_red \
 textures/s14_white \
 textures/s01_redshield \
 textures/com_rangerslayer \
 textures/s01_pink \
 textures/s07_magnadefender \
 textures/s01_goldar \
 textures/com_drakkon \
 textures/com_drakkonV2 \
 textures/s13_kat \
 textures/com_blacksentry \
 textures/mov_blue \
 textures/com_triniblackdragon \
 textures/s03_gold \
 textures/s01_white \
 textures/s09_pink \
 textures/s02_lordzedd \
 textures/s13_shadow \
 textures/s13_doggie \
 textures/s09_quantum \
 textures/s16_daishi \
 textures/s16_daishi_phantom \
 textures/s16_purple \
 textures/s18_lauren \
 textures/s01_scorpina \
 textures/sf_ryu \
 textures/sf_ryu90s \
 textures/ryu_H_less \
 textures/sf_chunli \
 textures/sf_chunli90s \
 textures/NB \
 textures/poisandra \
 textures/s01_rita \
 textures/s22_sledge

zip -j GrimbaHack.zip bin/Debug/net6.0/publish/GrimbaHack.dll \
 bin/Debug/net6.0/publish/Twitch* \
 bin/Debug/net6.0/publish/Newtonsoft.Json.dll \
 deps/UniverseLib.IL2CPP.Interop.dll \
 textures

zip -r GrimbaHack textures
rm textures -r
