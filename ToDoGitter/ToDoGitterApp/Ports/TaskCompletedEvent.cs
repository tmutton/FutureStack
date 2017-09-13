using System;
using Paramore.Brighter;

namespace ToDoGitterApp.Ports
{
    public class TaskCompletedEvent : Event
    {
        public string Title { get; private set; }
        public TaskCompletedEvent(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }
    }
}