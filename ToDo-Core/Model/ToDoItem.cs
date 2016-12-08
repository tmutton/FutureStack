using ToDoCore.Ports.Repositories;

namespace ToDoCore.Model
{
    public class ToDoItem : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}