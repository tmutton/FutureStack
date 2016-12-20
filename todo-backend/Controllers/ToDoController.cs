using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.ViewModelRetrievers;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;
using ToDoCore.ViewModels;

namespace FutureStack.Controllers
{
    [Route("api/[controller]")]
    public class ToDoController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseSqlServer(@"Server=localhost;Database=ToDo;Trusted_Connection=True;")
                .Options;
            var retriever = new ToDoViewModelRetriever(options);
            var toDos = retriever.Get(1, 10);

            return Ok(toDos);
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(int id)
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseSqlServer(@"Server=localhost;Database=ToDo;Trusted_Connection=True;")
                .Options;
             var retriever = new ToDoViewModelRetriever(options);
            var toDo = retriever.Get(id);

            return Ok(toDo);

            //TODO: Needs error handling for Not Found etc.
        }

        [HttpPost]
        public IActionResult Post([FromBody]AddToDoRequest request)
        {
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseSqlServer(@"Server=localhost;Database=ToDo;Trusted_Connection=True;")
                .Options;
            var addToDoCommand = new AddToDoCommand(title: request.Title);

            // Need to do this by command processor send, but baby steps, don't want to mess with
            var handler = new AddToDoCommandHandler(options);
            handler.Handle(addToDoCommand);

            var retriever = new ToDoViewModelRetriever(options);
            var addedToDo = retriever.Get(addToDoCommand.ToDoItemId);

            return CreatedAtRoute("GetTodo", new { id = addedToDo.Id }, addedToDo);
        }
   }
}


