FROM node:24-alpine AS build
WORKDIR /app

COPY AssignFlow.UI/package*.json ./
RUN npm ci

COPY AssignFlow.UI/ ./
RUN npm run build

FROM nginx:1.29-alpine AS final
COPY AssignFlow.UI/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/assignflow-ui/browser /usr/share/nginx/html
EXPOSE 80
