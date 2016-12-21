
using System;
using  paramore.brighter.commandprocessor;

namespace ToDoCore.Ports.Commands
{
    public class AddToDoCommand : Command
    {
        public string Title { get; }
        public bool Commpleted { get; }

        //out
        public int ToDoItemId { get; set; }

        public AddToDoCommand(string title, bool completed = false)
            : base(Guid.NewGuid())
        {
            Title = title;
            Commpleted = completed;
        }

    }
}