# PowerShell 7 on Windows. Run in a fresh PowerShell process.
param([string]$Dotnet = 'dotnet', [string]$Results = "$PSScriptRoot/TestResults", [string]$Method)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
$Results = [IO.Path]::GetFullPath($Results)
[IO.Directory]::CreateDirectory($Results) | Out-Null
$exe = (Get-Command $Dotnet -CommandType Application).Source
$env:DOTNET_ROOT = Split-Path $exe
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:UseOnlyInMemoryDatabase = 'true'
$env:LAB06_CAPTURAS = "$Results/capturas"
$env:Logging__LogLevel__Default = 'Warning'
$services = @()
$scratch = [IO.Directory]::CreateTempSubdirectory('eshop-lab06-').FullName
try {
    foreach ($project in @('src/PublicApi/PublicApi.csproj','src/Web/Web.csproj','tests/Lab06.Selenium/Lab06.Selenium.csproj')) {
        & $exe build "$root/$project" --verbosity quiet
        if ($LASTEXITCODE) { throw "Build failed: $project" }
    }
    # The certificate exists only for the local test run.
    $rsa = [Security.Cryptography.RSA]::Create(2048)
    $request = [Security.Cryptography.X509Certificates.CertificateRequest]::new('CN=localhost',$rsa,[Security.Cryptography.HashAlgorithmName]::SHA256,[Security.Cryptography.RSASignaturePadding]::Pkcs1)
    $san = [Security.Cryptography.X509Certificates.SubjectAlternativeNameBuilder]::new()
    $san.AddDnsName('localhost')
    $request.CertificateExtensions.Add($san.Build())
    $cert = $request.CreateSelfSigned([DateTimeOffset]::Now.AddMinutes(-5),[DateTimeOffset]::Now.AddDays(1))
    $env:Kestrel__Certificates__Default__Path = "$scratch/local.pfx"
    $env:Kestrel__Certificates__Default__Password = [guid]::NewGuid().ToString('N')
    [IO.File]::WriteAllBytes($env:Kestrel__Certificates__Default__Path,$cert.Export('Pfx',$env:Kestrel__Certificates__Default__Password))
    foreach ($app in @(@('PublicApi',5099),@('Web',5001))) {
        $name,$port = $app
        $url = "https://localhost:$port"
        $proc = Start-Process -WindowStyle Hidden -FilePath $exe -WorkingDirectory "$root/src/$name" -ArgumentList "bin/Debug/net10.0/$name.dll","--urls",$url -RedirectStandardOutput "$scratch/$name.log" -RedirectStandardError "$scratch/$name.err" -PassThru
        $services += $proc
        $probe = if ($name -eq 'PublicApi') { "$url/api/catalog-brands" } else { "$url/" }
        $ready = $false
        for ($i=0; $i -lt 60; $i++) {
            if ($proc.HasExited) { throw "$name exited. $(Get-Content "$scratch/$name.err" -Tail 4)" }
            try { $ready = (Invoke-WebRequest -SkipCertificateCheck -Uri $probe -TimeoutSec 3).StatusCode -eq 200 } catch {}
            if ($ready) {break}
            Start-Sleep -Seconds 1
        }
        if (!$ready) { throw "No response from $probe" }
    }
    $filter = if ($Method) { @('-method',$Method) } else { @() }
    & $exe "$PSScriptRoot/bin/Debug/net10.0/Lab06.Selenium.dll" -xml "$Results/xunit.xml" -html "$Results/xunit.html" @filter
    $resultCode = $LASTEXITCODE
} finally {
    foreach ($proc in $services) { if (!$proc.HasExited) { $proc.Kill(); $proc.WaitForExit() } }
    if (Test-Path "$scratch/local.pfx") {Remove-Item -LiteralPath "$scratch/local.pfx"}
}
exit $resultCode
