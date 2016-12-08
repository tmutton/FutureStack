using System.Collections.Generic;
using ToDoCore.ViewModelRetrievers;
using ToDoCore.ViewModels;

namespace ToDoCore.Adaptors.ViewModelRetrievers
{
    public class ToDoViewModelRetriever : IViewModelRetriever<ToDoViewModel>
    {
        public IEnumerable<ToDoViewModel> Get(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}