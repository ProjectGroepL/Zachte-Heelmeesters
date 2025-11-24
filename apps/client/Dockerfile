# Stage 1: build the app
FROM node:20-alpine AS build
WORKDIR /app

# Copy root package files for dependency installation
COPY package*.json ./
COPY turbo.json ./

# Copy workspace package files
COPY apps/client/package*.json ./apps/client/

# Install all dependencies using npm workspaces
RUN npm ci

# Copy all source code
COPY . .

# Build the client app using Turborepo
RUN npx turbo run build --filter=@zhm/client

# Stage 2: serve with nginx
FROM nginx:stable-alpine
COPY --from=build /app/apps/client/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]