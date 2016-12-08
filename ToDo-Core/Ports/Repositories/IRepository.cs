namespace ToDoCore.Ports.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        void Add(T newEntity);
    }
}