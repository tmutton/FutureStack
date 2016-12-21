using System;
using paramore.brighter.commandprocessor;

namespace ToDoCore.Ports.Commands
{
    public class DeleteAllToDosCommand : Command
    {
        public DeleteAllToDosCommand() : base(Guid.NewGuid())
        {
        }
    }
}