using Services.AdministratorOne.Abstractions;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using Services.Utilities;
using Services.Validations;

namespace Services.Applications
{
    public class ProductOneProcessor : IApplicationProcessor
    {
        // I was thinking of create factory but then I didn't find it much useful as ultimately it depends on
        // product code and the requirement says "In the future, the company might use AdministratorTwo for
        // ProductOne".
        private readonly IAdministrationService _administrationOneService;
        private readonly IAgeRangeValidator _ageRangeValidator;
        private readonly IMinimumPaymentValidator _minimumPaymentValidator;
        private readonly IKycService _kycService;
        private readonly IBus _bus;

        public ProductOneProcessor(IAdministrationService administrationService, IAgeRangeValidator ageRangeValidator,
            IMinimumPaymentValidator minimumPaymentValidator, IKycService kycService, IBus bus)
        {
            _administrationOneService = administrationService;
            _ageRangeValidator = ageRangeValidator;
            _minimumPaymentValidator = minimumPaymentValidator;
            _kycService = kycService;
            _bus = bus;
        }

        // I am not sure if I am allowed to change return type, but ideally this should return an object
        // informing error(s)/success and create investor response.
        public async Task Process(Application application)
        {
            if (application == null)
            {
                return;
            }

            int age = AgeCalculator.CalculateAge(application.Applicant.DateOfBirth);

            if (!_ageRangeValidator.IsRangeInValidRange(age, 18, 39))
            {
                return;
            }

            if (!_minimumPaymentValidator.IsMinimumPaymentReceived(application.Payment.Amount.Amount, 0.99m))
            {
                return;
            }

            var kycResult = await _kycService.GetKycReportAsync(application.Applicant);

            if (!kycResult.IsSuccess)
            {
                return;
            }

            var response = _administrationOneService.CreateInvestor(new AdministratorOne.Abstractions.Model.CreateInvestorRequest
            {
                AccountNumber = application.Applicant.BankAccounts[0].AccountNumber,
                Addressline1 = application.Applicant.Addresses[0].Addressline1,
                Addressline2 = application.Applicant.Addresses[0].Addressline2,
                Addressline3 = application.Applicant.Addresses[0].Addressline3,
                DateOfBirth = application.Applicant.DateOfBirth.ToString(),
                FirstName = application.Applicant.Forename,
                LastName = application.Applicant.Surname,
                InitialPayment = (int) application.Payment.Amount.Amount,
                Product = application.ProductCode.ToString(),
                PostCode = application.Applicant.Addresses[0].PostCode,
                SortCode = application.Applicant.BankAccounts[0].SortCode,
                Nino = application.Applicant.Nino
            });

            if (response != null && !string.IsNullOrWhiteSpace(response.InvestorId))
            {
                var domainEvent = new InvestorCreated(application.Applicant.Id, response.InvestorId);

                await _bus.PublishAsync(domainEvent);
            }
            else
            {
                return;
            }
        }
    }
}
