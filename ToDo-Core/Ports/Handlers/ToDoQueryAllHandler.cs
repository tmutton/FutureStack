using System.Linq;
using Darker;
using Microsoft.EntityFrameworkCore;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Queries;

namespace ToDoCore.Ports.Handlers
{
    public class ToDoQueryAllHandler : QueryHandler<ToDoQueryAll, ToDoQueryAll.Result>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public ToDoQueryAllHandler(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public override ToDoQueryAll.Result Execute(ToDoQueryAll request)
        {
            using (var uow = new ToDoContext(_options))
            {
                var items = uow.ToDoItems.Skip(request.PageNumber - 1 * request.PageSize).Take(request.PageSize).ToArray();
                var todos = new ToDoByIdQuery.Result[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    todos[i] = new ToDoByIdQuery.Result(items[i]);
                }
                return new ToDoQueryAll.Result(todos);
            }
        }
    }
}