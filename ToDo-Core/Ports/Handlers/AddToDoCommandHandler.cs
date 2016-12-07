using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Model;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class AddToDoCommandHandler : RequestHandler<AddToDoCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public AddToDoCommandHandler(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public override AddToDoCommand Handle(AddToDoCommand command)
        {
            using (var uow = new ToDoContext(_options))
            {
                var todo = new ToDoItem {Title = command.Title};
                uow.ToDoItems.Add(todo);
                uow.SaveChanges();
            }
            return base.Handle(command);
        }
    }
}