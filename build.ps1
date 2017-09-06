pushd .\ToDoBackend
dotnet restore
dotnet build
cd .\src\ToDoApi
rm "out" -recurse -force
dotnet publish -c Release -o out
cd ..
cd .\ToDoApp
rm "out" -recurse -force
dotnet publish -c Release -o out
popd