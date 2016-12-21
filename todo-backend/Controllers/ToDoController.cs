using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;
using ToDoCore.Ports.Queries;
using ToDoCore.ViewModels;

namespace FutureStack.Controllers
{
    [Route("api/[controller]")]
    public class ToDoController : Controller
    {
        private readonly DbContextOptions<ToDoContext> _dbContextOptions;
        private readonly IAmACommandProcessor _commandProcessor;

        public ToDoController(DbContextOptions<ToDoContext> dbContextOptions, IAmACommandProcessor commandProcessor)
        {
            _dbContextOptions = dbContextOptions;
            _commandProcessor = commandProcessor;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var retriever = new ToDoQueryAllHandler(_dbContextOptions);
            var toDos = retriever.Execute(new ToDoQueryAll(1, 10));

            foreach (var toDoItem in toDos.ToDoItems)
            {
                toDoItem.Url = Url.RouteUrl("GetTodo", new { id = toDoItem.Id }, protocol: Request.Scheme);
            }

            return Ok(toDos.ToDoItems);
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(int id)
        {
             var retriever = new ToDoByIdQueryHandler(_dbContextOptions);
            var toDo = retriever.Execute(new ToDoByIdQuery(id));
            toDo.Url = Url.RouteUrl("GetTodo", new { id = toDo.Id }, protocol: Request.Scheme);

            return Ok(toDo);

            //TODO: Needs error handling for Not Found etc.
        }

        [HttpPost]
        public IActionResult Post([FromBody]AddToDoRequest request)
        {
            var addToDoCommand = new AddToDoCommand(request.Title);

            _commandProcessor.Send(addToDoCommand);

            var retriever = new ToDoByIdQueryHandler(_dbContextOptions);
            var addedToDo = retriever.Execute(new ToDoByIdQuery(addToDoCommand.ToDoItemId));

            addedToDo.Url = Url.RouteUrl("GetTodo", new { id = addedToDo.Id }, protocol: Request.Scheme);
            return CreatedAtRoute("GetTodo", new { id = addedToDo.Id }, addedToDo);
        }


        [HttpDelete("{id}") ]
        public IActionResult Delete(int id)
        {
            var deleteToDoCommand = new DeleteToDoByIdCommand(id);

            _commandProcessor.Send(deleteToDoCommand);

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            var deleteAllToDosCommand = new DeleteAllToDosCommand();

            _commandProcessor.Send(deleteAllToDosCommand);

            return Ok();
        }


        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]UpdateToDoRequest request)
        {
            var updatedCommand = new UpdateToDoCommand(id, request.Title, request.Completed);
            _commandProcessor.Send(updatedCommand);

            var retriever = new ToDoByIdQueryHandler(_dbContextOptions);
            var addedToDo = retriever.Execute(new ToDoByIdQuery(id));

            addedToDo.Url = Url.RouteUrl("GetTodo", new { id = addedToDo.Id }, protocol: Request.Scheme);

            return Ok(addedToDo);
        }
    }

    public class UpdateToDoRequest
    {
        public string Title { get; set; }
        public bool? Completed { get; set; }

    }
}


