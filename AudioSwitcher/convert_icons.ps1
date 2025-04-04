# Settings
$inkscapeExe = "C:\Program Files\WindowsApps\25415Inkscape.Inkscape_1.3.2.0_x64__9waqn51p1ttv2\VFS\ProgramFilesX64\Inkscape\bin\inkscape.exe" 
$fontName = "Segoe Fluent Icons"
$size = 128
$dpi = 300

function Export-Char {
    param (
        [string]$code,
        [string]$name
    )
    
    $tempSvg = New-TemporaryFile
    $outputPng = "Images/Generated/$name.png"
    
    # Create a simple SVG file with just the character
    $svgContent = @"
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<svg xmlns="http://www.w3.org/2000/svg" width="$size" height="$size">
    <text x="50%" y="$($size - 1)" text-anchor="middle" 
    font-family="$fontName" font-size="$($size - 2)">&#x$code;</text>
</svg>
"@

    # Save the SVG content to a file
    $svgContent | Out-File -FilePath $tempSvg -Encoding utf8
    
    # Convert SVG to PNG using Inkscape
    Start-Process -Wait $inkscapeExe -ArgumentList --export-filename="$outputPng", --export-dpi=$dpi, $tempSvg
    if ($?) {
        Write-Host "Exported $code to $outputPng"
    } else {
        Write-Host "Failed to export $code"
    }
    
    # Clean up the temporary SVG file
    Remove-Item $tempSvg
}

Export-Char "e9ce" "Unknown"
Export-Char "e7f5" "Speakers"
Export-Char "e720" "Microphone"
Export-Char "e95b" "Headset"
Export-Char "e7f6" "Headphones"
