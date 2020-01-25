@echo off
set key=%1
for %%i in (%key%\*.png) do (
   echo %%i
   if exist %%i (
      call :convert "%%i"
   ) else (
      "%~dp0cecho" {0c}ERROR:{#} "%%i{#}" isn't exist.{\n}
   )
)
pause
exit /b

:convert
echo "%~dp0nvdxt" -nomipmap -file "%~1" -output "%~dpn1.dds"
"%~dp0nvdxt" -nomipmap -file "%~1" -output "%~dpn1.dds"
if not %errorlevel% == 0 (
   "%~dp0cecho" {0c}ERROR:{#} Can't convert "{09}%~1{#}".{\n}
)