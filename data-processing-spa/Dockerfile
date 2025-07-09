FROM node:20 as build
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build -- --configuration=production