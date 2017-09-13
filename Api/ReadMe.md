# Commands
## Building the Docker
docker build -t calendarapi .

## Running the Container

## Viewing the logs for the launched container
docker logs calendar_web_1
docker logs calendar_db_1

# View the health check on the web site
http://192.168.99.100:8000/calendar



## Interrogating the postgres db
# Run an interactive terminal session in the DB server
docker exec -ti $(docker ps -sqf "name=calendar_db") psql -U postgres

# connect to the dev database
\c calendar_dev

# list the tables in the dev database
\dt

#quit
\q

# Alternately you can use something DataGrip etc. to connect to the server, if you prefer to manage the Db via a GUI






