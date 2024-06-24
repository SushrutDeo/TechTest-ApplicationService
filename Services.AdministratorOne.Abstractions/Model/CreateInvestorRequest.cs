namespace Services.AdministratorOne.Abstractions.Model;

public class CreateInvestorRequest
{
    public string Reference { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string DateOfBirth { get; set; }
    
    public string Nino { get; set; }
    
    public string Addressline1 { get; set; }
    
    public string Addressline2 { get; set; }
    
    public string Addressline3 { get; set; }
    
    public string Addressline4 { get; set; }
    
    public string PostCode { get; set; }
    
    public string Email { get; set; }
    
    public string MobileNumber { get; set; }
    
    public string Product { get; set; }
    
    public string SortCode { get; set; }
    
    public string AccountNumber { get; set; }
    
    public int InitialPayment { get; set; }
}