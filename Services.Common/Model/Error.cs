namespace Services.Common.Abstractions.Model;

public sealed record Error(string System, string Code, string Description)
{
    public static readonly Error Empty = new(string.Empty, string.Empty, string.Empty);
}