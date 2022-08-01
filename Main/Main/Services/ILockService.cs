namespace Main.Services;

public interface ILockService
{
    Task LockResource();

    Task UnlockResource();
}