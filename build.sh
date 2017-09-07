cd ToDoBackend
dotnet restore
dotnet build
cd src/ToDoApi
rm -rf out
dotnet publish -c Release -o out
cd ..
cd ToDoApp
rm -rf out
dotnet publish -c Release -o out
cd ../../..