using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class DeleteAllToDosCommandHandler : RequestHandler<DeleteAllToDosCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public DeleteAllToDosCommandHandler(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public override DeleteAllToDosCommand Handle(DeleteAllToDosCommand command)
        {
            using (var uow = new ToDoContext(_options))
            {
                uow.ToDoItems.RemoveRange(uow.ToDoItems);
                uow.SaveChanges();
            }


            return base.Handle(command);

        }
    }
}