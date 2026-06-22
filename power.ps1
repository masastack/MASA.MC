param(
    [Parameter(Mandatory = $true)][string]$t,
    [Parameter(Mandatory = $true)][string]$u,
    [Parameter(Mandatory = $true)][string]$p,
    [Alias("H")][string]$DockerHost
)

Write-Host "Tag: $t"

$dockerArgs = @()
if (-not [string]::IsNullOrWhiteSpace($DockerHost))
{
    $dockerArgs += @("-H", $DockerHost)
    Write-Host "Use remote Docker host: $DockerHost"
}

$ServiceDockerfilePath = "./src/Services/Masa.Mc.Service/Dockerfile"
$ServiceServerName = "masa-mc-service-admin"
$ServiceImage = "registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t"

& docker @dockerArgs login --username=$u registry.cn-hangzhou.aliyuncs.com --password=$p
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& docker @dockerArgs build -t $ServiceImage -f $ServiceDockerfilePath .
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& docker @dockerArgs push $ServiceImage
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# $WebDockerfilePath = "./src/Web/Masa.Mc.Web.Admin.Server/Dockerfile"
# $WebServerName = "masa-mc-web-admin"
# & docker @dockerArgs build -t "registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t" -f $WebDockerfilePath .
# & docker @dockerArgs push "registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t"