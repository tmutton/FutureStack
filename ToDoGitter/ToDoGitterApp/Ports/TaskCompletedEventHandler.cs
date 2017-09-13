using Paramore.Brighter;

namespace ToDoGitterApp.Ports
{
    internal class TaskCompletedEventHandler : RequestHandler<TaskCompletedEvent>
    {
        public override TaskCompletedEvent Handle(TaskCompletedEvent command)
        {
            System.Console.WriteLine($"Task {command.Title} is completed");
            return base.Handle(command);
        }
    }
}