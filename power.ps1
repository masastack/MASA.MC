param($t,$u,$p)

#docker build 
#Write-Host "Hello.$args"
Write-Host "Hello $t"

docker login --username=$u registry.cn-hangzhou.aliyuncs.com --password=$p

$ServiceDockerfilePath="./src/Services/Masa.Mc.Service/Dockerfile"
$ServiceServerName="masa-mc-service-admin"
# $WebDockerfilePath="./src/Web/Masa.Mc.Web.Admin.Server/Dockerfile"
# $WebServerName="masa-mc-web-admin"

docker build -t registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t  -f $ServiceDockerfilePath .
docker push registry.cn-hangzhou.aliyuncs.com/masastack/${ServiceServerName}:$t 

# docker build -t registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t  -f $WebDockerfilePath .
# docker push registry.cn-hangzhou.aliyuncs.com/masastack/${WebServerName}:$t 