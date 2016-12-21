using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ToDoController(DbContextOptions<ToDoContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
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
            var handler = new AddToDoCommandHandler(_dbContextOptions);
            handler.Handle(addToDoCommand);

            var retriever = new ToDoByIdQueryHandler(_dbContextOptions);
            var addedToDo = retriever.Execute(new ToDoByIdQuery(addToDoCommand.ToDoItemId));

            return CreatedAtRoute("GetTodo", new { id = addedToDo.Id }, addedToDo);
        }
   }
}


