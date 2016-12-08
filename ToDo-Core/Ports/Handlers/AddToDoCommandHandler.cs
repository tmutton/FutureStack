using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
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
                var repository = new ToDoItemRepository(uow);
                var savedItem = repository.Add(new ToDoItem {Title = command.Title});
                command.ToDoItemId = savedItem.Id;
            }
            return base.Handle(command);
        }
    }
}