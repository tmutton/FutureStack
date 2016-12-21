using System;
using paramore.brighter.commandprocessor;

namespace ToDoCore.Ports.Commands
{
    public class DeleteToDoCommand : Command
    {
        public int ToDoId { get; }

        public DeleteToDoCommand(int toDoId) : base(Guid.NewGuid())
        {
            ToDoId = toDoId;
        }
    }
}
