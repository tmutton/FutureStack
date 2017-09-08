using System;
using Paramore.Brighter;

namespace ToDoSlackerApp.Port
{
    internal class TaskUpdateEvent : Event
    {
        public TaskUpdateEvent() : base(Guid.NewGuid())
        {
        }
    }
}