using System;

namespace AllTheBeans.Domain.Entities;

public class IdempotencyKey
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public IdempotencyKey() { }

    public IdempotencyKey(string key)
    {
        Key = key;
        CreatedAt = DateTime.UtcNow;
    }
}
