#!/bin/sh

# Apply database migrations
# echo "Apply database migrations"
# python /code/manage.py db migrate -m "docker initiated migration"
python /code/manage.py recreate_db

# Start server
echo "Starting server"
python /code/manage.py runserver -h 0.0.0.0 -p 8000

