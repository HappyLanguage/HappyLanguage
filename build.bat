@echo off
@echo Compiling...
@msbuild /v:m /nologo /t:Build /p:Configuration=Release Happy_language\Happy_language.csproj
@echo Done

@echo Copying binaries to bin folder
if not exist bin md bin
copy /Y Happy_language\Release\Happy_language.exe bin\Happy_language.exe >nul
copy /Y Happy_language\Release\Antlr4.Runtime.dll bin\Antlr4.Runtime.dll >nul
@echo Executable file is in the bin directory
