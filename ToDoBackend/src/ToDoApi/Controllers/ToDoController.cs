using System.Threading.Tasks;
using Darker;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Queries;
using ToDoCore.ViewModels;

namespace ToDoApi.Controllers
{
    [Route("api/[controller]")]
    public class ToDoController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public ToDoController(
            IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var toDos = await _queryProcessor.ExecuteAsync(new ToDoQueryAll());

            foreach (var toDoItem in toDos.ToDoItems)
            {
                toDoItem.Url = Url.RouteUrl("GetTodo", new { id = toDoItem.Id }, protocol: Request.Scheme);
            }

            return Ok(toDos.ToDoItems);
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public async Task<IActionResult> GetById(int id)
        {
            // Get the ToDo by id via the query processor
            var toDo = await _queryProcessor.ExecuteAsync(new ToDoByIdQuery(id));
            toDo.Url = Url.RouteUrl("GetTodo", new { id = toDo.Id }, protocol: Request.Scheme);

            return Ok(toDo);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AddToDoRequest request)
        {
            // Create a AddToDoCommand

            // Use the command processor to Send command  

            // Get the ToDo by id via the query processor
            
            // Return the newly created ToDo
            //return CreatedAtRoute("GetTodo", new { id = addedToDo.Id }, addedToDo);

            //Remove this once done
            return Ok();
        }


//        [HttpDelete("{id}") ]
//        public async Task<IActionResult> Delete(int id)
//        {
//            // Create a DeleteToDoByIdCommand
//            
//
//            // Use the command processor to Send command
//            
//
//            return Ok();
//        }

        [HttpDelete]
        public async Task <IActionResult> Delete()
        {
            // Create a DeleteAllToDosCommand
            var deleteAllToDosCommand = new DeleteAllToDosCommand();

            // Use the command processor to Send command
            await _commandProcessor.SendAsync(deleteAllToDosCommand);

            return Ok();
        }


//        [HttpPatch("{id}")]
//        public async Task<IActionResult> Patch(int id, [FromBody]UpdateToDoRequest request)
//        {
//            // Create a UpdateToDoCommand
        
//            // Use the command processor to Send command
//
//            // Get the ToDo by id via the query processor
//
//            return Ok(updatedToDo);
//        }
    }
}


