Push-Location
Set-Location $PSScriptRoot

# sudo powershell {
	Start-Job { Stop-Process -Name PowerToys* } | Wait-Job > $null

	# change this to your PowerToys installation path
	$ptPath = "$env:LOCALAPPDATA\PowerToys"
	$projectName = 'AudioSwitcher'
	$safeProjectName = 'AudioSwitcher'
	$debug = '.\bin\x64\Debug\net8.0-windows'
	$dest = "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins\$projectName"
	$files = @(
		"Community.PowerToys.Run.Plugin.$safeProjectName.deps.json",
		"Community.PowerToys.Run.Plugin.$safeProjectName.dll",
		'plugin.json',
		'Images'
	)

	Set-Location $debug
	Remove-Item -LiteralPath "$dest" -Force -Recurse -ErrorAction Ignore
	mkdir $dest -Force -ErrorAction Ignore | Out-Null
	Copy-Item $files $dest -Force -Recurse

	& "$ptPath\PowerToys.exe"
# }

Pop-Location
