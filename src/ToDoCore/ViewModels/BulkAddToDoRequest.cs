using System.Collections.Generic;

namespace ToDoCore.ViewModels
{
    public class BulkAddToDoRequest
    {
        public List<AddToDoRequest> ItemsToAdd { get; set; }
    }
}