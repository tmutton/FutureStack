using Paramore.Brighter;

namespace ToDoSlackerApp.Port
{
    internal class TaskUpdateEventHandler : RequestHandler<TaskUpdateEvent>
    {
        public override TaskUpdateEvent Handle(TaskUpdateEvent command)
        {
            return base.Handle(command);
        }
    }
}