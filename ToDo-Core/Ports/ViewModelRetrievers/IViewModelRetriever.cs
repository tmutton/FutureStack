using System.Collections.Generic;
using ToDoCore.ViewModels;

namespace ToDoCore.ViewModelRetrievers
{
    public interface IViewModelRetriever<T> where T : IViewModel
    {
        T Get(int id);
        IEnumerable<T> Get(int pageIndex, int pageSize);
    }
}