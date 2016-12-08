namespace ToDoCore.Ports.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        T Add(T newEntity);
    }
}