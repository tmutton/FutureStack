# FutureStack
An implementation of the TODO API specification in .NET Core

# To build and run
You need to publish both ToDoAPI and ToDoApp's .csproj files to an **out** directory

dotnet publish ToDoApi.csporj **out**

The Dockerfile expects to find the artefacts in this directory and will fail if it does not

Then you can run the project using docker-compose via

docker-compose up -d --build

# Todo-Backend Tests
Run the tests at: https://www.todobackend.com/specs/index.html
First, change the test target root: 

http://{host:ip}:8080/api/ToDo

where {host:ip} is your docker hosts IP address

# Skipping the reverse proxy
The site should also be running on 

http://{host:ip}:5000/api/ToDo

if you want to skip the proxy for testing
