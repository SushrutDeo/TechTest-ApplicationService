namespace Services.AdministratorOne.Abstractions.Model;

public class AdministratorException : Exception
{
    public AdministratorException(string code)
    {
        Code = code;
    }

    public AdministratorException(string code, string message) : base(message)
    {
        Code = code;
    }

    public AdministratorException(string code, string message, Exception inner) : base(message, inner)
    {
        Code = code;
    }
    
    public string Code { get; set; }
}