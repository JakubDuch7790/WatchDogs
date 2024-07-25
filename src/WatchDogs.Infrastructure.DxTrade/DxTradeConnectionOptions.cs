namespace Contracts;

public record DxTradeConnectionOptions
{
    public string Username { get; init; } = String.Empty;
    public string Vendor { get; init; } = String.Empty;
    public string Password { get; init; } = String.Empty;
}
