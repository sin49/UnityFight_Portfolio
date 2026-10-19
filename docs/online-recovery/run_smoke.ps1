param(
    [ValidateSet('file','mysql')] [string]$Storage = 'file',
    [switch]$Transport,
    [int]$Port = 17877
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$workRoot = Join-Path $repoRoot '.online-recovery'
$previousDotnetHome = $env:DOTNET_CLI_HOME
$previousCertificate = $env:DOTNET_GENERATE_ASPNET_CERTIFICATE
Push-Location $repoRoot
try {
    New-Item -ItemType Directory -Path $workRoot -Force | Out-Null
    $env:DOTNET_CLI_HOME = Join-Path $workRoot 'dotnet-home'
    $env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
    dotnet build Server/01Server/Server/Server.csproj --no-restore --nologo -v minimal
    if ($LASTEXITCODE -ne 0) { throw 'Server build failed. No restore was attempted.' }

    if ($Transport) {
        $output = Join-Path $workRoot 'transport'
        New-Item -ItemType Directory -Path $output -Force | Out-Null
        $referenceRoot = Join-Path $env:USERPROFILE '.nuget/packages/microsoft.netcore.app.ref/7.0.20/ref/net7.0'
        if (!(Test-Path -LiteralPath $referenceRoot)) {
            throw 'The smoke harness requires the existing .NET 7 reference pack in the local cache.'
        }
        $dotnetRoot = Split-Path (Get-Command dotnet).Source
        $sdkLine = (dotnet --list-sdks | Select-Object -Last 1)
        $sdkVersion = ($sdkLine -split ' ')[0]
        $compiler = Join-Path $dotnetRoot "sdk/$sdkVersion/Roslyn/bincore/csc.dll"
        $clientRoot = Join-Path $repoRoot 'Client/1_backup/Assets/_Project/Scripts/NetWorkManager'
        $sources = @('NetworkManager.cs','PacketHandler.cs','Packet/01Packet.cs','Packet/ClientPacketManager.cs',
            'Core/Session.cs','Core/RecvBuffer.cs','Core/SendBuffer.cs','Core/ServerSession.cs','Core/Connector.cs','Core/PacketQueue.cs') |
            ForEach-Object { Join-Path $clientRoot $_ }
        $sources += Join-Path $PSScriptRoot 'ClientTransportSmoke.cs'
        $arguments = @('-nostdlib+','-target:exe',('-out:"' + (Join-Path $output 'ClientTransportSmoke.dll') + '"'),'-nowarn:0436')
        $arguments += Get-ChildItem -LiteralPath $referenceRoot -Filter '*.dll' | ForEach-Object { '-r:"' + $_.FullName + '"' }
        $core = Join-Path $repoRoot 'Server/01Server/ServerCore/bin/Debug/net7.0/ServerCore.dll'
        $arguments += '-r:"' + $core + '"'
        $arguments += $sources | ForEach-Object { '"' + $_ + '"' }
        $response = Join-Path $output 'build.rsp'
        $arguments | Set-Content -LiteralPath $response -Encoding UTF8
        dotnet $compiler -noconfig "@$response"
        if ($LASTEXITCODE -ne 0) { throw 'Client transport harness compilation failed.' }
        Copy-Item -LiteralPath $core -Destination (Join-Path $output 'ServerCore.dll') -Force
        '{"runtimeOptions":{"tfm":"net7.0","framework":{"name":"Microsoft.NETCore.App","version":"7.0.10"}}}' |
            Set-Content -LiteralPath (Join-Path $output 'ClientTransportSmoke.runtimeconfig.json') -Encoding UTF8
    }

    $testArguments = @((Join-Path $PSScriptRoot 'test_file_roundtrip.py'), '--data-root',
        (Join-Path $workRoot "$Storage-data"), '--storage', $Storage, '--port', "$Port")
    if ($Transport) { $testArguments += '--transport-smoke' }
    # Prefer the bundled Python when available; otherwise use the user's Python.
    $bundledPython = Join-Path $env:USERPROFILE '.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe'
    $pythonCommand = if (Test-Path -LiteralPath $bundledPython) { $bundledPython } else { 'python' }
    & $pythonCommand @testArguments
    if ($LASTEXITCODE -ne 0) { throw 'Roundtrip test failed; see packet/server log above.' }
}
finally {
    $env:DOTNET_CLI_HOME = $previousDotnetHome
    $env:DOTNET_GENERATE_ASPNET_CERTIFICATE = $previousCertificate
    Pop-Location
}
