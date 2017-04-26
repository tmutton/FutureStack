# FutureStack
An implementation of the TODO API specification in .NET Core

# To build and run
You need to publish both ToDoAPI and ToDoApp's .csproj files to an **out** directory

dotnet publish ToDoApi.csporj **out**

The Dockerfile expects to find the artefacts in this directory and will fail if it does not

Then you can run the project using docker-compose via

docker-compose up -d --build

