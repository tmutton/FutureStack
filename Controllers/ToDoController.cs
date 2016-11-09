using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace FutureStack.Controllers
{
    [Route("todo-backend")]
    public class ToDoController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {"value1", "value2"};
        }
   }
}


