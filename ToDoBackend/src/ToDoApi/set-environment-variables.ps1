$env:RabbitMQ:Uri = "amqp://guest:guest@localhost:5672/%2f"
$env:RabbitMQ:Exchange= "future.stack.exchange"
$env:Database:ToDo = "ToDoDb.sqlite"
$env:Database:MessageStore= "messages.sqlite"
$env:Database:MessageTableName= "Messages"

Get-ChildItem Env:RabbitMQ:Uri 
Get-ChildItem Env:RabbitMQ:Exchange
Get-ChildItem Env:Database:ToDo 
Get-ChildItem Env:Database:MessageStore
Get-ChildItem Env:Database:MessageTableName
