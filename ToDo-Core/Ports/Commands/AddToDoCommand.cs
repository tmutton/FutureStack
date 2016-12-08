
using System;
using  paramore.brighter.commandprocessor;

namespace ToDoCore.Ports.Commands
{
    public class AddToDoCommand : Command
    {
        public string Title { get; }
        public int ToDoItemId { get; set; }

        public AddToDoCommand(string title)
            : base(Guid.NewGuid())
        {
            Title = title;
        }
    }
}