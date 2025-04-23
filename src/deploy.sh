#!/bin/bash

# Stop running containers
docker-compose -f docker-compose.prod.yml down

# Remove old images
docker image prune -f

# Build and start new containers
docker-compose -f docker-compose.prod.yml up -d --build

# Show logs
docker-compose -f docker-compose.prod.yml logs -f