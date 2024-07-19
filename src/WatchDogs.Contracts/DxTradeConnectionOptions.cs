namespace Contracts;

public class DxTradeConnectionOptions
{
    public const string Credentials = "Credentials";

    public string Username { get; set; } = String.Empty;
    public string Domain { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
}
