namespace Main.Services;

public interface IResiliencyService
{
    Task ExhaustResource();
}