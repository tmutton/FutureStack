using System;
using paramore.brighter.commandprocessor;

namespace ToDoCore.Ports.Commands
{
    public class UpdateToDoCommand : Command
    {
        public string Title { get; }
        public bool? Complete { get; }
        public int ToDoId { get; }

        public UpdateToDoCommand(int id, string title = null, bool? complete = null) : base(Guid.NewGuid())
        {
            ToDoId = id;
            Title = title;
            Complete = complete;
        }
    }
}