using System;
using Paramore.Brighter;

namespace ToDoSlackerApp.Port
{
    internal class TaskUpdateEventMessageMapper : IAmAMessageMapper<TaskUpdateEvent>
    {
        public Message MapToMessage(TaskUpdateEvent request)
        {
            throw new NotImplementedException();
        }

        public TaskUpdateEvent MapToRequest(Message message)
        {
            throw new NotImplementedException();
        }
    }
}