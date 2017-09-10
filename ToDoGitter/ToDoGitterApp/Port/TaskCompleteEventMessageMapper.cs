using Newtonsoft.Json;
using Paramore.Brighter;

namespace ToDoGitterApp.Ports
{
    public class TaskCompleteEventMessageMapper : IAmAMessageMapper<TaskCompletedEvent>
    {
        public Message MapToMessage(TaskCompletedEvent request)
        {
            throw new System.NotImplementedException();
        }

        public TaskCompletedEvent MapToRequest(Message message)
        {
            return JsonConvert.DeserializeObject<TaskCompletedEvent>(message.Body.Value);
        }
    }
}