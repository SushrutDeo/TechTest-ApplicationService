using Services.AdministratorOne.Abstractions.Model;

namespace Services.AdministratorOne.Abstractions;

public interface IAdministrationService
{
    CreateInvestorResponse CreateInvestor(CreateInvestorRequest request);
}