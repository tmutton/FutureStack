pushd .\ToDoBackend
dotnet restore
dotnet build
cd .\src\ToDoApi
rm "out" -recurse -force
dotnet publish -c Release -o out
popd
cd ToDoGitter
dotnet restore
dotnet build
cd ToDoGitterApp
rm "out" -recurse -force
dotnet publish -c Release -o out
cd ../../..