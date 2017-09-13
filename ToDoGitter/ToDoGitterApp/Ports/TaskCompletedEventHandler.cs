using System.Threading.Tasks;
using Paramore.Brighter;
using ToDoGitterApp.Adapter.Gitter;

namespace ToDoGitterApp.Ports
{
    internal class TaskCompletedEventHandler : RequestHandler<TaskCompletedEvent>
    {
        public override TaskCompletedEvent Handle(TaskCompletedEvent command)
        {
            var message = $"Task \"{command.Title}\" is completed";

            var gitterClient = new GitterClient();

            gitterClient.Send(message).Wait();

            return base.Handle(command);
        }
    }
}