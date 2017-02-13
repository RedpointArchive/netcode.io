param()

trap
{
    Write-Host "An error occurred"
    Write-Host $_
    Write-Host $_.Exception.StackTrace
    exit 1
}

$ErrorActionPreference = 'Stop'

cd $PSScriptRoot\..
$root = Get-Location

& "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" /m /p:Platform=Win32 /p:Configuration=Release netcode.io\netcode.Windows.vcxproj
if ($LastExitCode -ne 0) {
  exit 1
}
Copy-Item -Force netcode.io\bin\Windows\Win32\Release\netcode.dll netcode.io-csharp\bin\Windows\AnyCPU\Release\netcode32.dll
Copy-Item -Force netcode.io\bin\Windows\Win32\Release\netcode.dll netcode.io.test\bin\Windows\AnyCPU\Release\netcode32.dll
Copy-Item -Force netcode.io\bin\Windows\Win32\Release\netcode.dll netcode.io-client-example\bin\Windows\AnyCPU\Release\netcode32.dll
Copy-Item -Force netcode.io\bin\Windows\Win32\Release\netcode.dll netcode.io-server-example\bin\Windows\AnyCPU\Release\netcode32.dll
Copy-Item -Force netcode.io\bin\Windows\x64\Release\netcode.dll netcode.io-csharp\bin\Windows\AnyCPU\Release\netcode64.dll
Copy-Item -Force netcode.io\bin\Windows\x64\Release\netcode.dll netcode.io.test\bin\Windows\AnyCPU\Release\netcode64.dll
Copy-Item -Force netcode.io\bin\Windows\x64\Release\netcode.dll netcode.io-client-example\bin\Windows\AnyCPU\Release\netcode64.dll
Copy-Item -Force netcode.io\bin\Windows\x64\Release\netcode.dll netcode.io-server-example\bin\Windows\AnyCPU\Release\netcode64.dll

if (!(Test-Path native-dist)) {
  mkdir native-dist
}

Copy-Item -Force netcode.io\bin\Windows\Win32\Release\netcode.dll native-dist\netcode32.dll
Copy-Item -Force netcode.io\bin\Windows\x64\Release\netcode.dll native-dist\netcode64.dll