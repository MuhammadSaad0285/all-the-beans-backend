using AllTheBeans.Domain.Constants;
using System;

namespace AllTheBeans.Domain.ValueObjects;

public class Money
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public Money()
    {
        // Default to zero in GBP
        Amount = 0;
        Currency = CurrencyCodes.GBP;
    }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (other.Currency != Currency)
            throw new InvalidOperationException("Cannot add amounts with different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public override string ToString()
    {
        string symbol = Currency == CurrencyCodes.GBP ? "£"
                      : Currency == CurrencyCodes.USD ? "$"
                      : Currency == CurrencyCodes.EUR ? "€"
                      : "";
        return symbol + Amount.ToString("0.00");
    }
}
