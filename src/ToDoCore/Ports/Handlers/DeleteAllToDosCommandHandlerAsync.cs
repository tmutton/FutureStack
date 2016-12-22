using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class DeleteAllToDosCommandHandlerAsync : RequestHandlerAsync<DeleteAllToDosCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public DeleteAllToDosCommandHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public override  async Task<DeleteAllToDosCommand > HandleAsync(DeleteAllToDosCommand command, CancellationToken? ct = null)
        {
            using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                await repository.DeleteAllAsync(ct ?? default(CancellationToken));
           }

            return  await base.HandleAsync(command, ct);
        }
    }
}