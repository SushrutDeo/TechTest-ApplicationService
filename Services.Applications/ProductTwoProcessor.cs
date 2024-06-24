using Services.AdministratorTwo.Abstractions;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using Services.Utilities;
using Services.Validations;

namespace Services.Applications
{
    public class ProductTwoProcessor : IApplicationProcessor
    {
        // I was thinking of create factory but then I didn't find it much useful as ultimately it depends on
        // product code and the requirement says "In the future, the company might use AdministratorTwo for
        // ProductOne".
        private readonly IAdministrationService _administrationTwoService;
        private readonly IAgeRangeValidator _ageRangeValidator;
        private readonly IMinimumPaymentValidator _minimumPaymentValidator;
        private readonly IKycService _kycService;
        private readonly IBus _bus;

        public ProductTwoProcessor(IAdministrationService administrationService, IAgeRangeValidator ageRangeValidator,
            IMinimumPaymentValidator minimumPaymentValidator, IKycService kycService, IBus bus)
        {
            _administrationTwoService = administrationService;
            _ageRangeValidator = ageRangeValidator;
            _minimumPaymentValidator = minimumPaymentValidator;
            _kycService = kycService;
            _bus = bus;
        }

        // I am not sure if I am allowed to change return type, but ideally this should return an object
        // informing error(s)/success and create investor response.
        public async Task Process(Application application)
        {
            // This validation code is common for both the product. Can be moved to one place.
            if (application == null)
            {
                return;
            }

            int age = AgeCalculator.CalculateAge(application.Applicant.DateOfBirth);

            if (!_ageRangeValidator.IsRangeInValidRange(age, 18, 50))
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

            var createInvestorResponse = await _administrationTwoService.CreateInvestorAsync(application.Applicant);

            var investorCreated = new InvestorCreated(application.Applicant.Id, createInvestorResponse.Value.ToString());

            await _bus.PublishAsync(investorCreated);

            var createAccountResponse = _administrationTwoService.CreateAccountAsync(createInvestorResponse.Value, application.ProductCode);

            var accountCreated = new AccountCreated(createInvestorResponse.Value.ToString(),
                application.ProductCode, createAccountResponse.Result.Value.ToString());

            await _bus.PublishAsync(accountCreated);

            var paymentProcessResponse = await _administrationTwoService.ProcessPaymentAsync(createAccountResponse.Result.Value, application.Payment);

            var paymentProcessed = new ApplicationCompleted(paymentProcessResponse.Value);

            await _bus.PublishAsync(paymentProcessed);
        }
    }
}
