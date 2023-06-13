FROM hub.nexdev.net/base/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder
WORKDIR /src

COPY ./Fanex.BADailyWinLoss.Service/Fanex.BADailyWinLoss.Business ./Fanex.BADailyWinLoss.Business
COPY ./Fanex.BADailyWinLoss.Service/Fanex.BADailyWinLoss.Service/Fanex.BADailyWinLoss.Service.csproj ./Fanex.BADailyWinLoss.Service/Fanex.BADailyWinLoss.Service.csproj

RUN ls /src -l
WORKDIR /src/Fanex.BADailyWinLoss.Service
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v n

COPY ./Fanex.BADailyWinLoss.Service/Fanex.BADailyWinLoss.Service .

RUN dotnet publish -r linux-x64 --self-contained false -o /app 

FROM hub.nexdev.net/base/dotnet/core/aspnet:5.0-nap-mail-core

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf


ENV TZ="America/La_Paz" 

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.BADailyWinLoss.Service.dll"]
