REM www.mbnq.pl

@echo off
title RED. Renamer
setlocal enabledelayedexpansion
cls

set /a _counter=0

for %%f in (*.png) do (
    ren "%%f" "Red.Pack1.!_counter!.png"
    set /a _counter+=1
)

echo Done.
pause