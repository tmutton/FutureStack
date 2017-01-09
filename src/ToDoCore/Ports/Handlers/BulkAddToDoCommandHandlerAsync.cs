using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Model;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class BulkAddToDoCommandHandlerAsync: RequestHandlerAsync<BulkAddToDoCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public BulkAddToDoCommandHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public override async Task<BulkAddToDoCommand> HandleAsync(BulkAddToDoCommand command,
            CancellationToken? ct = null)
        {
            using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                foreach (var todo in command.ToDos)
                {
                    var savedItem = await repository.AddAsync(
                        new ToDoItem {Title = todo.Title, Completed = todo.Completed, Order = todo.Order},
                        ct ?? default(CancellationToken)
                    );

                    command.ToDoItemIds.Add(savedItem.Id);
                }

                return await base.HandleAsync(command, ct);
            }
        }
    }
}