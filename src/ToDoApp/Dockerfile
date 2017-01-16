FROM microsoft/dotnet:latest

# create a directory for the app source code 
RUN mkdir -p /usr/todo
WORKDIR /usr/todo

# copy the source code
COPY . /usr/todo

# restore the dependencies
WORKDIR /usr/todo/
RUN dotnet restore

# Build the site
RUN dotnet build

# Expose the port
CMD ["dotnet", "run"]

