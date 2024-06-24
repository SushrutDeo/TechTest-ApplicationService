namespace Services.Common.Abstractions.Model;

public class Application
{
    public Guid Id { get; init; }

    public User Applicant { get; init; }

    public ProductCode ProductCode { get; init; }

    public Payment Payment { get; init; }
}
