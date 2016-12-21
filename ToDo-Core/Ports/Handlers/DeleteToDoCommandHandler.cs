using System.Linq;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class DeleteToDoCommandHandler : RequestHandler<DeleteToDoCommand>
    {
        private readonly DbContextOptions<ToDoContext> _dbContextOptions;

        public DeleteToDoCommandHandler(DbContextOptions<ToDoContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public override DeleteToDoCommand Handle(DeleteToDoCommand command)
        {
            using (var uow = new ToDoContext(_dbContextOptions))
            {
                var toDoItem = uow.ToDoItems.Single(t => t.Id == command.ToDoId);
                uow.Remove(toDoItem);
                uow.SaveChanges();
            }

            return base.Handle(command);
        }
    }
}