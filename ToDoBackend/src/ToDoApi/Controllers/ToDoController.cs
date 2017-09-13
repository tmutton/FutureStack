using System.Threading.Tasks;
using Darker;
using Microsoft.AspNetCore.Mvc;
using ToDoCore.Ports.Queries;

namespace ToDoApi.Controllers
{
    [Route("api/[controller]")]
    public class ToDoController : Controller
    {
        private readonly IQueryProcessor _queryProcessor;

        public ToDoController(
            IQueryProcessor queryProcessor)
        {
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

    }
}


