namespace E_CommerceApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> CompleteAsync(); // المسؤول عن SaveChanges
    }
}
