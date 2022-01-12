FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish src/Barista/NCafe.Barista.Api -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS https://*:443
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet NCafe.Barista.Api.dll