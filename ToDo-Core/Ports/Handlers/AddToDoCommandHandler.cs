using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;
using ToDoCore.Adaptors.Db;
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
            return base.Handle(command);
        }
    }
}