namespace WatchDogs.Contracts;
public interface IEntityFrameworkUnitOfWorkFactory
{
    IUnitOfWork Create();
}