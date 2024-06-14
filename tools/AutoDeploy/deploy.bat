@echo off
chcp 65001

set "url_file=urls.txt"

for /f "usebackq tokens=1-3 delims=- " %%a in (`echo %date%`) do (
    set "originDate=%%b"
)


for /f "tokens=1-3 delims=/" %%a in ("%originDate%") do (
    set "date=%%a%%b%%c"
)

set "target_folder=%date%"

mkdir "%target_folder%"
mkdir "%target_folder%\shortcut"

set "starterDir="

for /F "usebackq" %%i in ("%url_file%") do (
    echo Downloading %%i ...
    curl.exe --progress-bar -O "%%i"

    for %%j in (*.zip) do (
        mkdir "temp_folder"
        tar.exe -xf %%~nxj -C "temp_folder" && (

            for /D %%i in ("temp_folder\*") do (
                move /Y "%%i" "%target_folder%"
            )

            rmdir /S /Q "Temp_folder"
            del %%~nxj

            for %%f in ("%target_folder%\%%~nj\*.exe") do (
                if not "%%~nxf"=="UnityCrashHandler64.exe" (
                    setlocal enabledelayedexpansion
                    set "filename=%%~nf"
                    echo FileName !filename!
                    for /f "tokens=1,2 delims=_" %%a in ("!filename!") do set "splitname=%%a"
                    set "shortcutname=!filename!.exe"
                    echo ShortCutName !shortcutname!
                    
                    echo "!filename!" | findstr /C:"meta-starter" >nul
                    if errorlevel 1 (
                        powershell.exe -Command "$s=(New-Object -ComObject WScript.Shell).CreateShortcut('%cd%\%target_folder%\shortcut\!shortcutname!.lnk');$s.TargetPath='%cd%\%target_folder%\%%~nj\%%~nxf';$s.Save()"
                    )
                    echo Successfully created a shortcut for !shortcutname!"
                    
                    endlocal
                )
            )
        )
    )
)

pause
cmd