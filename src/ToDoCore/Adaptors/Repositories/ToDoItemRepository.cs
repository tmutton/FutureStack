using ToDoCore.Adaptors.Db;
using ToDoCore.Model;
using ToDoCore.Ports.Repositories;

namespace ToDoCore.Adaptors.Repositories
{
    public class ToDoItemRepository : IRepository<ToDoItem>
    {
        private readonly ToDoContext _uow;

        public ToDoItemRepository(ToDoContext uow)
        {
            _uow = uow;
        }

        public ToDoItem Add(ToDoItem newEntity)
        {
             var savedItem = _uow.ToDoItems.Add(newEntity);
            _uow.SaveChanges();
            return savedItem.Entity;
        }
    }
}