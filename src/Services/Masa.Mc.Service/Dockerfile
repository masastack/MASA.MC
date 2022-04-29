#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/Masa.Mc.Service/Masa.Mc.Service.Admin.csproj", "src/Services/Masa.Mc.Service/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.Common/Masa.Mc.Infrastructure.Common.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.Common/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.Sms/Masa.Mc.Infrastructure.Sms.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.Sms/"]
COPY ["src/Contracts/Masa.Mc.Contracts.Admin/Masa.Mc.Contracts.Admin.csproj", "src/Contracts/Masa.Mc.Contracts.Admin/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.ObjectExtending/Masa.Mc.Infrastructure.ObjectExtending.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.ObjectExtending/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.Ddd.Application.Contracts/Masa.Mc.Infrastructure.Ddd.Application.Contracts.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.Ddd.Application.Contracts/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.EntityFrameworkCore/Masa.Mc.Infrastructure.EntityFrameworkCore.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.EntityFrameworkCore/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.Sms.Aliyun/Masa.Mc.Infrastructure.Sms.Aliyun.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.Sms.Aliyun/"]
RUN dotnet restore "src/Services/Masa.Mc.Service/Masa.Mc.Service.Admin.csproj"
COPY . .
WORKDIR "/src/src/Services/Masa.Mc.Service"
RUN dotnet build "Masa.Mc.Service.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Masa.Mc.Service.Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Masa.Mc.Service.Admin.dll"]
