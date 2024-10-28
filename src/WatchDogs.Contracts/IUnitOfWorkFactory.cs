namespace WatchDogs.Contracts;
public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}