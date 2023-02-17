ping -n 2 127.0.0.1>nul
"%cd%\7-Zip\7z.exe" x "%cd%\Arma3ServerTools.zip" -y -aoa -o"%cd%\"
ping -n 1 127.0.0.1>nul
start /d "%cd%" Arma3ServerTools.exe
exit