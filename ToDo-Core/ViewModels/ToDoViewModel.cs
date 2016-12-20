using ToDoCore.Model;

namespace ToDoCore.ViewModels
{
    public class ToDoViewModel : IViewModel
    {
        public ToDoViewModel(ToDoItem item)
        {
            Id = item.Id;
            Title = item.Title;
        }

        public ToDoViewModel(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; set; }
        public string Title { get; set; }
    }
}