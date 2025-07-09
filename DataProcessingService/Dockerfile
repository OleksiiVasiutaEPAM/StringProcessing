FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY DataProcessingService/ ./DataProcessingService/
COPY DataProcessingService.Business/ ./DataProcessingService.Business/
COPY DataProcessingService.Business.Contracts/ ./DataProcessingService.Business.Contracts/

RUN dotnet restore DataProcessingService/DataProcessingService.csproj
RUN dotnet publish DataProcessingService/DataProcessingService.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out ./
ENTRYPOINT ["dotnet", "DataProcessingService.dll"]
