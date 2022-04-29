#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Web/Masa.Mc.Web.Admin.Server/Masa.Mc.Web.Admin.Server.csproj", "src/Web/Masa.Mc.Web.Admin.Server/"]
COPY ["src/Web/Masa.Mc.Web.Admin/Masa.Mc.Web.Admin.csproj", "src/Web/Masa.Mc.Web.Admin/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.Common/Masa.Mc.Infrastructure.Common.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.Common/"]
COPY ["src/ApiGateways/Caller/Masa.Mc.Caller/Masa.Mc.Caller.csproj", "src/ApiGateways/Caller/Masa.Mc.Caller/"]
COPY ["src/Contracts/Masa.Mc.Contracts.Admin/Masa.Mc.Contracts.Admin.csproj", "src/Contracts/Masa.Mc.Contracts.Admin/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.ObjectExtending/Masa.Mc.Infrastructure.ObjectExtending.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.ObjectExtending/"]
COPY ["src/Infrastructure/Masa.Mc.Infrastructure.Ddd.Application.Contracts/Masa.Mc.Infrastructure.Ddd.Application.Contracts.csproj", "src/Infrastructure/Masa.Mc.Infrastructure.Ddd.Application.Contracts/"]
RUN dotnet restore "src/Web/Masa.Mc.Web.Admin.Server/Masa.Mc.Web.Admin.Server.csproj"
COPY . .
WORKDIR "/src/src/Web/Masa.Mc.Web.Admin.Server"
RUN dotnet build "Masa.Mc.Web.Admin.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Masa.Mc.Web.Admin.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Masa.Mc.Web.Admin.Server.dll"]
