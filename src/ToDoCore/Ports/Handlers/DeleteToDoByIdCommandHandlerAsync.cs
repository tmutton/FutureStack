using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Attributes;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class DeleteToDoByIdCommandHandlerAsync : RequestHandlerAsync<DeleteToDoByIdCommand>
    {
        private readonly DbContextOptions<ToDoContext> _dbContextOptions;

        public DeleteToDoByIdCommandHandlerAsync(DbContextOptions<ToDoContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        public override async Task<DeleteToDoByIdCommand > HandleAsync(DeleteToDoByIdCommand command, CancellationToken? ct = null)
        {
            using (var uow = new ToDoContext(_dbContextOptions))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                await repository.DeleteAsync(command.ToDoId, ct ?? default(CancellationToken));
           }

            return await base.HandleAsync(command, ct);
       }
    }
}