$env:RabbitMQ:Uri = "amqp://guest:guest@localhost:5672/%2f"
$env:RabbitMQ:Exchange= "future.stack.exchange"
$env:Database:ToDo = "Data Source=./ToDoDb.sqlite"

Get-ChildItem Env:RabbitMQ:Uri 
Get-ChildItem Env:RabbitMQ:Exchange
Get-ChildItem Env:Database:ToDo 
