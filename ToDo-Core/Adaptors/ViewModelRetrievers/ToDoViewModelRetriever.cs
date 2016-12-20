using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToDoCore.Adaptors.Db;
using ToDoCore.ViewModelRetrievers;
using ToDoCore.ViewModels;

namespace ToDoCore.Adaptors.ViewModelRetrievers
{
    public class ToDoViewModelRetriever : IViewModelRetriever<ToDoViewModel>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public ToDoViewModelRetriever(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        public IEnumerable<ToDoViewModel> Get()
        {
            throw new System.NotImplementedException();
        }

        public ToDoViewModel Get(int id)
        {
            using (var uow = new ToDoContext(_options))
            {
                var toDoItem = uow.ToDoItems.Single(t => t.Id == id);
                return new ToDoViewModel(toDoItem);
            }
        }
    }
}