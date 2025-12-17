using E_CommerceApi.Data;
using E_CommerceApi.Repositories;
using System.Collections;

namespace E_CommerceApi.Repositories
{
    #region UnitOfWork دور
    //يمسك DbContext 

    //يعمل Repository لأي Entity

    //يخزن الـ Repositories عشان ما يعملش new كل شوية

    //يعمل SaveChanges مرة واحدة

    //يقفل الـ Context في الآخر
    #endregion
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _context;
        private Hashtable _repositories;

        public UnitOfWork(StoreDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null) 
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}