namespace ToDoCore.ViewModels
{
    public class ToDoViewModel : IViewModel
    {
        public ToDoViewModel() {}

        public ToDoViewModel(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; set; }
        public string Title { get; set; }
    }
}