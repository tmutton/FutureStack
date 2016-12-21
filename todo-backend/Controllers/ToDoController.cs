using Microsoft.AspNetCore.Mvc;
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

            return Ok(toDos);
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(int id)
        {
             var retriever = new ToDoByIdQueryHandler(_dbContextOptions);
            var toDo = retriever.Execute(new ToDoByIdQuery(id));

            return Ok(toDo);

            //TODO: Needs error handling for Not Found etc.
        }

        [HttpPost]
        public IActionResult Post([FromBody]AddToDoRequest request)
        {
            var addToDoCommand = new AddToDoCommand(title: request.Title);

            // Need to do this by command processor send, but baby steps, don't want to mess with
            //var handler = new AddToDoCommandHandler(_dbContextOptions);
            //handler.Handle(addToDoCommand);
            _commandProcessor.Send(addToDoCommand);

            var retriever = new ToDoByIdQueryHandler(_dbContextOptions);
            var addedToDo = retriever.Execute(new ToDoByIdQuery(addToDoCommand.ToDoItemId));

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
    }
}


