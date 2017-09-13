#!/bin/sh

# Apply database migrations
echo "Apply database migrations"
python manage.py db migrate -m "docker initiated migration"

# Start server
echo "Starting server"
python manage.py runserver -h 0.0.0.0 -p 8000

