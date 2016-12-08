using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;
using ToDoCore.ViewModels;

namespace FutureStack.Controllers
{
    [Route("api/[controller]")]
    public class ToDoController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {"value1", "value2"};
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(int id)
        {
            var retriever = new ViewModelRetriever<ToDoViewModel>();
            var toDo = retriever.Get(id);

            return Ok(toDo);

            //TODO: Needs error handling for Not Found etc.
        }

        [HttpPost]
        public IActionResult Post([FromBody]ToDoRequest request)
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseSqlServer(@"Server=localhost;Database=ToDo;Trusted_Connection=True;")
                .Options;
            var addToDoCommand = new AddToDoCommand(title: request.Title);

            // Need to do this by command processor send, but baby steps, don't want to mess with
            var handler = new AddToDoCommandHandler(options);
            handler.Handle(addToDoCommand);

            var retriever = new ViewModelRetriever<ToDoViewModel>();
            var addedToDo = retriever.Get(addToDoCommand.ToDoItemId);

            return CreatedAtRoute("GetTodo", new { id = addedToDo.Id }, addedToDo);
        }
   }
}


