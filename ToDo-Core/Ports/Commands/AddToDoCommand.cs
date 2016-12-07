
using System;
using  paramore.brighter.commandprocessor;

namespace ToDoCore.Ports.Commands
{
    public class AddToDoCommand : Command
    {
        public string Title { get; }

        public AddToDoCommand(string title)
            : base(Guid.NewGuid())
        {
            Title = title;
        }
    }
}