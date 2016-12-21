using System.Linq;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class DeleteToDoByIdCommandHandler : RequestHandler<DeleteToDoByIdCommand>
    {
        private readonly DbContextOptions<ToDoContext> _dbContextOptions;

        public DeleteToDoByIdCommandHandler(DbContextOptions<ToDoContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public override DeleteToDoByIdCommand Handle(DeleteToDoByIdCommand command)
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