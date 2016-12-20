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

        public IEnumerable<ToDoViewModel> Get(int pageIndex, int pageSize)
        {
            using (var uow = new ToDoContext(_options))
            {
                var items = uow.ToDoItems.Skip(pageIndex - 1 * pageSize).Take(pageSize).ToArray();
                var viewmodels = new ToDoViewModel[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    viewmodels[i] = new ToDoViewModel(items[i]);
                }
                return viewmodels;
            }
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