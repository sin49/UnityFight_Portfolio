param([Parameter(Mandatory=$true)][string]$Uid)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$output = Join-Path $repoRoot '.online-recovery/db-check'
$server = Join-Path $repoRoot 'Server/01Server/Server/bin/Debug/net7.0'
$referenceRoot = Join-Path $env:USERPROFILE '.nuget/packages/microsoft.netcore.app.ref/7.0.20/ref/net7.0'
New-Item -ItemType Directory -Path $output -Force | Out-Null
$dotnetRoot = Split-Path (Get-Command dotnet).Source
$sdkVersion = ((dotnet --list-sdks | Select-Object -Last 1) -split ' ')[0]
$compiler = Join-Path $dotnetRoot "sdk/$sdkVersion/Roslyn/bincore/csc.dll"
$arguments = @('-nostdlib+','-target:exe',('-out:"' + (Join-Path $output 'MariaDbReadCheck.dll') + '"'))
$arguments += Get-ChildItem -LiteralPath $referenceRoot -Filter '*.dll' | ForEach-Object { '-r:"' + $_.FullName + '"' }
$arguments += @('Server.dll','MySqlConnector.dll','Newtonsoft.Json.dll') | ForEach-Object { '-r:"' + (Join-Path $server $_) + '"' }
$arguments += '"' + (Join-Path $PSScriptRoot 'MariaDbReadCheck.cs') + '"'
$response = Join-Path $output 'build.rsp'
$arguments | Set-Content -LiteralPath $response -Encoding UTF8
dotnet $compiler -noconfig "@$response"
if ($LASTEXITCODE -ne 0) { throw 'Read-only DB verifier compilation failed.' }
Get-ChildItem -LiteralPath $server -Filter '*.dll' | Copy-Item -Destination $output -Force
Copy-Item -LiteralPath (Join-Path $server 'Server.runtimeconfig.json') -Destination (Join-Path $output 'MariaDbReadCheck.runtimeconfig.json') -Force
Copy-Item -LiteralPath (Join-Path $server 'Server.deps.json') -Destination (Join-Path $output 'MariaDbReadCheck.deps.json') -Force
dotnet (Join-Path $output 'MariaDbReadCheck.dll') $Uid
if ($LASTEXITCODE -ne 0) { throw 'Independent MariaDB SELECT failed.' }
