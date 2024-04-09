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

mkdir -p combos \
combos/__EXPORT \
combos/COM_BlackSentry \
combos/COM_Drakkon \
combos/COM_RangerSlayer \
combos/COM_TriniBlackDragon \
combos/MOV_BlueRanger \
combos/S01_AdamPark \
combos/S01_Goldar \
combos/S01_GreenRanger \
combos/S01_RedRanger \
combos/S01_Rita \
combos/S01_Scorpina \
combos/S02_LordZedd \
combos/S03_GoldRanger \
combos/S07_MagnaDefender \
combos/S09_PinkRanger \
combos/S09_QuantumRanger \
combos/S13_KatRanger \
combos/S13_ShadowRanger \
combos/S14_WhiteRanger \
combos/S16_DaiShi \
combos/S16_PurpleRanger \
combos/S18_Lauren \
combos/S21_YellowRanger \
combos/S22_Poisandra \
combos/SF_ChunLiRanger \
combos/SF_RyuRanger

zip -j GrimbaHack.zip bin/Debug/net6.0/publish/GrimbaHack.dll \
 bin/Debug/net6.0/publish/Twitch* \
 bin/Debug/net6.0/publish/Newtonsoft.Json.dll \
 deps/UniverseLib.IL2CPP.Interop.dll \
 textures

zip -r GrimbaHack textures
zip -r GrimbaHack combos
rm textures -r
