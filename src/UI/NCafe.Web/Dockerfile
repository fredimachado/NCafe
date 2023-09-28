FROM nginx:1.25-alpine
WORKDIR /user/share/nginx/html
EXPOSE 80

COPY nginx.conf /etc/nginx/nginx.conf

COPY prepare-appsettings.sh /docker-entrypoint.d/
RUN chmod +x /docker-entrypoint.d/prepare-appsettings.sh

COPY output/wwwroot .
