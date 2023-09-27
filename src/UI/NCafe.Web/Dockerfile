FROM nginx:alpine
WORKDIR /user/share/nginx/html
EXPOSE 80

COPY output/wwwroot .

COPY nginx.conf /etc/nginx/nginx.conf
