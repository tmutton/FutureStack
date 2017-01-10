FROM microsoft/dotnet:latest

# create a directory for the app source code 
RUN mkdir -p /usr/todo
WORKDIR /usr/todo

# copy the source code
COPY . /usr/todo

# restore the dependencies
WORKDIR /usr/todo/src/ToDoCore
RUN dotnet restore
WORKDIR /usr/todo/src/ToDoAPI
RUN dotnet restore

# Build the site
RUN dotnet build

# Expose the port
EXPOSE 5000
CMD ["dotnet", "run"]

