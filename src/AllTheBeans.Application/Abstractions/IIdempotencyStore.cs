namespace AllTheBeans.Application.Abstractions;

public interface IIdempotencyStore
{
    Task<bool> ExistsAsync(string key);
    Task CreateKeyAsync(string key);
}
