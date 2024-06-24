namespace Services.AdministratorOne.Abstractions.Model;

public class CreateInvestorResponse
{
    public string Reference { get; set; }
    
    public string InvestorId { get; set; }
    
    public string AccountId { get; set; }
    
    public string PaymentId { get; set; }
}