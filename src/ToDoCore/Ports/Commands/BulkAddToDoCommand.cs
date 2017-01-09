using System;
using System.Collections.Generic;
using paramore.brighter.commandprocessor;
using ToDoCore.ViewModels;

namespace ToDoCore.Ports.Commands
{
    public class BulkAddToDoCommand : Command
    {
        public IEnumerable<AddToDoRequest> ToDos { get; }
        public IList<int> ToDoItemIds { get; } //out parameter

        public BulkAddToDoCommand(IEnumerable<AddToDoRequest> todos) : base(Guid.NewGuid())
        {
            ToDos = todos;
            ToDoItemIds = new List<int>();
        }
    }
}