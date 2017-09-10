FROM microsoft/dotnet:latest

# create a directory for the app source code 
RUN mkdir -p /usr/todogitter
WORKDIR /usr/todogitter

# copy the source code
COPY out .

# Run the app
ENTRYPOINT ["dotnet", "ToDoGitterApp.dll"]

