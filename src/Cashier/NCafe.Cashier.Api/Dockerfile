FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
EXPOSE 8080

WORKDIR /app
COPY output .
USER $APP_UID
ENTRYPOINT dotnet NCafe.Cashier.Api.dll
