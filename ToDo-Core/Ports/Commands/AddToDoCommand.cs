
using System;
using  paramore.brighter.commandprocessor;

namespace ToDoCore.Ports.Commands
{
    public class AddToDoCommand : Command
    {
        public AddToDoCommand() : base(Guid.NewGuid())
        {
        }
    }
}