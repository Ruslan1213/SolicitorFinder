using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Interfaces;

public interface IBaseRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : BaseEntity
{
}
