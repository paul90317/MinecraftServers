@echo off
mkdir publish
copy bin\Release\*.dll publish\*.dll
copy bin\Release\*.exe publish\*.exe
copy bin\Release\*.config publish\*.config
copy bin\Release\version.txt publish\version.txt
copy bin\Release\version.txt latest.txt
cd publish
7z a ../publish.zip *