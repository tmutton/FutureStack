# Commands
## Building the Docker
docker build -t calendarapi .

## Running the Container
docker run -d --name mycalendarapi -p 8080:5000 calendarapi  # Binds port 8080 on host to port 80 on container

## Viewing the logs for the launched container
docker logs mycalendarapi



