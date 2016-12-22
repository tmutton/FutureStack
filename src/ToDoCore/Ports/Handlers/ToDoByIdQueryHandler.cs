using System.Linq;
using Darker;
using Microsoft.EntityFrameworkCore;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Queries;

namespace ToDoCore.Ports.Handlers
{
    public class ToDoByIdQueryHandler : QueryHandler<ToDoByIdQuery, ToDoByIdQuery.Result>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public ToDoByIdQueryHandler(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public override ToDoByIdQuery.Result Execute(ToDoByIdQuery request)
        {
           using (var uow = new ToDoContext(_options))
            {
                var toDoItem = uow.ToDoItems.Single(t => t.Id == request.Id);
                return new ToDoByIdQuery.Result(toDoItem);
            }

        }
    }
}