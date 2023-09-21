FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
EXPOSE 80

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

RUN apk update && apk add --upgrade apk-tools && apk upgrade --available
RUN apk add --no-cache icu-libs
RUN apk add --no-cache tzdata

RUN addgroup -S app -g 1000 \
    && adduser -S app -G app -u 1000 \
    && mkdir -p /app \
    && chown -R app:app /app

WORKDIR /app
COPY output .
ENTRYPOINT dotnet NCafe.Barista.Api.dll
