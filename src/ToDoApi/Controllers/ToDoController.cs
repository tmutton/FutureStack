using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;
using ToDoCore.Ports.Queries;
using ToDoCore.ViewModels;

namespace ToDoApi.Controllers
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
        public async Task<IActionResult> Get()
        {
            var retriever = new ToDoQueryAllHandlerAsync(_dbContextOptions);
            var toDos = await retriever.ExecuteAsync(new ToDoQueryAll(1, 10));

            foreach (var toDoItem in toDos.ToDoItems)
            {
                toDoItem.Url = Url.RouteUrl("GetTodo", new { id = toDoItem.Id }, protocol: Request.Scheme);
            }

            return Ok(toDos.ToDoItems);
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public async Task<IActionResult> GetById(int id)
        {
            var retriever = new ToDoByIdQueryHandlerAsync(_dbContextOptions);
            var toDo = await retriever.ExecuteAsync(new ToDoByIdQuery(id));
            toDo.Url = Url.RouteUrl("GetTodo", new { id = toDo.Id }, protocol: Request.Scheme);

            return Ok(toDo);

            //TODO: Needs error handling for Not Found etc.
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AddToDoRequest request)
        {
            var addToDoCommand = new AddToDoCommand(request.Title, request.Completed, request.Order);

            await _commandProcessor.SendAsync(addToDoCommand);

            var retriever = new ToDoByIdQueryHandlerAsync(_dbContextOptions);
            var addedToDo = await retriever.ExecuteAsync(new ToDoByIdQuery(addToDoCommand.ToDoItemId));

            addedToDo.Url = Url.RouteUrl("GetTodo", new { id = addedToDo.Id }, protocol: Request.Scheme);
            return CreatedAtRoute("GetTodo", new { id = addedToDo.Id }, addedToDo);
        }


        [HttpDelete("{id}") ]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteToDoCommand = new DeleteToDoByIdCommand(id);

            await _commandProcessor.SendAsync(deleteToDoCommand);

            return Ok();
        }

        [HttpDelete]
        public async Task <IActionResult> Delete()
        {
            var deleteAllToDosCommand = new DeleteAllToDosCommand();

            await _commandProcessor.SendAsync(deleteAllToDosCommand);

            return Ok();
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody]UpdateToDoRequest request)
        {
            var updatedCommand = new UpdateToDoCommand(id, request.Title, request.Completed, request.Order);
            await _commandProcessor.SendAsync(updatedCommand);

            var retriever = new ToDoByIdQueryHandlerAsync(_dbContextOptions);
            var addedToDo = await retriever.ExecuteAsync(new ToDoByIdQuery(id));

            addedToDo.Url = Url.RouteUrl("GetTodo", new { id = addedToDo.Id }, protocol: Request.Scheme);

            return Ok(addedToDo);
        }
    }
}


