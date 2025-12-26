using System;
using AllTheBeans.Application.Abstractions;

namespace AllTheBeans.Infrastructure.Time;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
