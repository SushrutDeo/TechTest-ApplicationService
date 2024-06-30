using AutoFixture;

using Moq;

using Services.AdministratorTwo.Abstractions;
using Services.Applications;
using Services.Common.Abstractions.Model;

namespace Services.Tests
{
    // In this class I am not adding the validation tests as they will be repeated. As mentioned in my
    // other comment, the validation functionality can be moved to the on place. I didn't move it as I
    // was not sure how much changes I can make to the return type of the "Process" method. Becasue, if we
    // move validation code, that method needs to return an object detailing which validation failed and 
    // that information needs to be conveyed to the caller of "Process" method. 
    [TestClass]
    public class ProductTwoProcessorTests : ApplicationProcessorTests
    {
        private readonly Mock<IAdministrationService> _administrationServiceMock = new();

        private readonly ProductTwoProcessor _processor;

        public ProductTwoProcessorTests()
        {
            _processor = new ProductTwoProcessor(_administrationServiceMock.Object,
                _ageRangeValidatorMock.Object,
                _minimumPaymentValidatorMock.Object,
                _kycServiceMock.Object,
                _busMock.Object);
        }

        [TestMethod]
        public async Task Process_When_Create_Investor_Fails()
        {
            // Arrange
            var application = CreateApplication();

            var investorResponse = new Result<Guid>(false, new Error("Error", "Error", "Error"), Guid.NewGuid());

            _ageRangeValidatorMock.Setup(x => x.IsRangeInValidRange(It.IsAny<int>(), 18, 50)).Returns(true);

            _minimumPaymentValidatorMock.Setup(x => x.IsMinimumPaymentReceived(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(true);

            _kycServiceMock.Setup(x => x.GetKycReportAsync(It.IsAny<User>()))
                .ReturnsAsync(new Result<KycReport>(true, new Error(string.Empty, string.Empty, string.Empty),
                new KycReport(It.IsAny<Guid>(), true)));

            _administrationServiceMock.Setup(x => x.CreateInvestorAsync(It.IsAny<User>()))
                .ReturnsAsync(investorResponse);

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Once);
            _administrationServiceMock.Verify(x => x.CreateAccountAsync(It.IsAny<Guid>(), It.IsAny<ProductCode>()), Times.Never);
            _administrationServiceMock.Verify(x => x.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>()), Times.Never);

            _busMock.Verify(x => x.PublishAsync(It.IsAny<InvestorCreated>()), Times.Never);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<AccountCreated>()), Times.Never);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<ApplicationCompleted>()), Times.Never);
        }

        [TestMethod]
        public async Task Process_When_Create_Account_Fails()
        {
            // Arrange
            var application = CreateApplication();

            var investorResponse = new Result<Guid>(true, new Error(string.Empty, string.Empty, string.Empty), Guid.NewGuid());

            var accountResponse = new Result<Guid>(false, new Error("Error", "Error", "Error"), Guid.NewGuid());

            _ageRangeValidatorMock.Setup(x => x.IsRangeInValidRange(It.IsAny<int>(), 18, 50)).Returns(true);

            _minimumPaymentValidatorMock.Setup(x => x.IsMinimumPaymentReceived(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(true);

            _kycServiceMock.Setup(x => x.GetKycReportAsync(It.IsAny<User>()))
                .ReturnsAsync(new Result<KycReport>(true, new Error(string.Empty, string.Empty, string.Empty),
                new KycReport(It.IsAny<Guid>(), true)));

            _administrationServiceMock.Setup(x => x.CreateInvestorAsync(It.IsAny<User>()))
                .ReturnsAsync(investorResponse);

            _administrationServiceMock.Setup(x => x.CreateAccountAsync(It.IsAny<Guid>(), It.IsAny<ProductCode>()))
                .ReturnsAsync(accountResponse);

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Once);
            _administrationServiceMock.Verify(x => x.CreateAccountAsync(It.IsAny<Guid>(), It.IsAny<ProductCode>()), Times.Once);
            _administrationServiceMock.Verify(x => x.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>()), Times.Never);

            _busMock.Verify(x => x.PublishAsync(It.IsAny<InvestorCreated>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<AccountCreated>()), Times.Never);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<ApplicationCompleted>()), Times.Never);
        }

        [TestMethod]
        public async Task Process_When_Create_Process_Payment_Fails()
        {
            // Arrange
            var application = CreateApplication();

            var investorResponse = new Result<Guid>(true, new Error(string.Empty, string.Empty, string.Empty), Guid.NewGuid());

            var accountResponse = new Result<Guid>(true, new Error(string.Empty, string.Empty, string.Empty), Guid.NewGuid());

            var paymentResponse = new Result<Guid>(false, new Error("Error", "Error", "Error"), Guid.NewGuid());

            _ageRangeValidatorMock.Setup(x => x.IsRangeInValidRange(It.IsAny<int>(), 18, 50)).Returns(true);

            _minimumPaymentValidatorMock.Setup(x => x.IsMinimumPaymentReceived(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(true);

            _kycServiceMock.Setup(x => x.GetKycReportAsync(It.IsAny<User>()))
                .ReturnsAsync(new Result<KycReport>(true, new Error(string.Empty, string.Empty, string.Empty),
                new KycReport(It.IsAny<Guid>(), true)));

            _administrationServiceMock.Setup(x => x.CreateInvestorAsync(It.IsAny<User>()))
                .ReturnsAsync(investorResponse);

            _administrationServiceMock.Setup(x => x.CreateAccountAsync(It.IsAny<Guid>(), It.IsAny<ProductCode>()))
                .ReturnsAsync(accountResponse);

            _administrationServiceMock.Setup(x => x.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>()))
                .ReturnsAsync(paymentResponse);

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Once);
            _administrationServiceMock.Verify(x => x.CreateAccountAsync(It.IsAny<Guid>(), It.IsAny<ProductCode>()), Times.Once);
            _administrationServiceMock.Verify(x => x.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>()), Times.Once);

            _busMock.Verify(x => x.PublishAsync(It.IsAny<InvestorCreated>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<AccountCreated>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<ApplicationCompleted>()), Times.Never);
        }

        [TestMethod]
        public async Task Process_When_There_Are_No_Errors()
        {
            // Arrange
            var application = CreateApplication();

            var investorResponse = new Result<Guid>(true, new Error(string.Empty, string.Empty, string.Empty), Guid.NewGuid());

            var accountResponse = new Result<Guid>(true, new Error(string.Empty, string.Empty, string.Empty), Guid.NewGuid());

            var paymentResponse = new Result<Guid>(true, new Error(string.Empty, string.Empty, string.Empty), Guid.NewGuid());

            _ageRangeValidatorMock.Setup(x => x.IsRangeInValidRange(It.IsAny<int>(), 18, 50)).Returns(true);

            _minimumPaymentValidatorMock.Setup(x => x.IsMinimumPaymentReceived(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(true);

            _kycServiceMock.Setup(x => x.GetKycReportAsync(It.IsAny<User>()))
                .ReturnsAsync(new Result<KycReport>(true, new Error(string.Empty, string.Empty, string.Empty),
                new KycReport(It.IsAny<Guid>(), true)));

            _administrationServiceMock.Setup(x => x.CreateInvestorAsync(It.IsAny<User>()))
                .ReturnsAsync(investorResponse);

            _administrationServiceMock.Setup(x => x.CreateAccountAsync(It.IsAny<Guid>(), It.IsAny<ProductCode>()))
                .ReturnsAsync(accountResponse);

            _administrationServiceMock.Setup(x => x.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>()))
                .ReturnsAsync(paymentResponse);

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestorAsync(It.IsAny<User>()), Times.Once);
            _administrationServiceMock.Verify(x => x.CreateAccountAsync(It.IsAny<Guid>(), It.IsAny<ProductCode>()), Times.Once);
            _administrationServiceMock.Verify(x => x.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>()), Times.Once);
            
            _busMock.Verify(x => x.PublishAsync(It.IsAny<InvestorCreated>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<AccountCreated>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<ApplicationCompleted>()), Times.Once);
        }
    }
}
