using System.Linq;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class UpdateToDoCommandHandler : RequestHandler<UpdateToDoCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public UpdateToDoCommandHandler(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public override UpdateToDoCommand Handle(UpdateToDoCommand command)
        {

            using (var uow = new ToDoContext(_options))
            {
                var toDoItem = uow.ToDoItems.Single(t => t.Id == command.ToDoId);

                if (command.Title != null)
                    toDoItem.Title = command.Title;

                if (command.Complete.HasValue)
                    toDoItem.Completed = command.Complete.Value;

                if (command.Order.HasValue)
                    toDoItem.Order = command.Order.Value;

                uow.SaveChanges();
            }

            return base.Handle(command);
        }
    }
}