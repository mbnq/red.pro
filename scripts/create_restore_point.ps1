<#
	This script create Windows Restore Point 
	named with current date and time.
	
	https://www.mbnq.pl

#>

Write-Host "This script creates Windows Restore Point named with current date and time."
Read-Host -Prompt "Press Enter to continue"

if (!([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Start-Process powershell "-Command `"& {Start-Process powershell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"' -Verb RunAs}`"" -Verb RunAs
    exit
}

	$currentDateTime = Get-Date -Format 'yyyy-MM-dd_HH-mm'
	$restorePointName = "RestorePoint_$currentDateTime"
	Checkpoint-Computer -Description $restorePointName -RestorePointType MODIFY_SETTINGS

Write-Host "Done."
Read-Host -Prompt "Press Enter to exit"