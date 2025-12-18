# Build stage
FROM node:22-alpine AS builder

# Set working directory
WORKDIR /app

# Copy package files for better caching
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy source code
COPY . .

# Accept build argument and set as environment variable
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

# Build the application
RUN npm run build

# Production stage
FROM node:22-alpine

# Install a simple static file server
RUN npm install -g serve

# Set working directory
WORKDIR /app

# Copy built app from builder stage
COPY --from=builder /app/dist ./dist

# Expose port 80
EXPOSE 80

# Serve the static files
CMD ["serve", "-s", "dist", "-l", "80"]