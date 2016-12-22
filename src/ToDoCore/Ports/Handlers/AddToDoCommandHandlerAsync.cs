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
    public class AddToDoCommandHandlerAsync : RequestHandlerAsync<AddToDoCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public AddToDoCommandHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public override async Task<AddToDoCommand> HandleAsync(AddToDoCommand command, CancellationToken? ct = null)
        {
            using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                var savedItem = await repository.AddAsync(
                    new ToDoItem {Title = command.Title, Completed = command.Commpleted, Order = command.Order},
                    ct ?? default(CancellationToken)
                );
                command.ToDoItemId = savedItem.Id;
            }
            return await base.HandleAsync(command, ct);
        }
    }
}