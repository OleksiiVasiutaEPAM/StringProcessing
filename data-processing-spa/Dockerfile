FROM node:24.4-alpine as build
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build -- --configuration=production