using System.Collections.Generic;
using Darker;

namespace ToDoCore.Ports.Queries
{
    public class ToDoQueryAll : IQueryRequest<ToDoQueryAll.Result>
    {

        public sealed class Result : IQueryResponse
        {
            public IEnumerable<ToDoByIdQuery.Result> ToDoItems { get; }

            public Result(IEnumerable<ToDoByIdQuery.Result> items)
            {
                ToDoItems = items;
            }
        }
    }
}