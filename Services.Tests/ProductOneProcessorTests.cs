using AutoFixture;

using Moq;

using Services.AdministratorOne.Abstractions;
using Services.AdministratorOne.Abstractions.Model;
using Services.Applications;
using Services.Common.Abstractions.Model;

namespace Services.Tests
{
    [TestClass]
    public class ProductOneProcessorTests : ApplicationProcessorTests
    {
        private readonly Mock<IAdministrationService> _administrationServiceMock = new();

        private readonly ProductOneProcessor _processor;

        public ProductOneProcessorTests()
        {
            _processor = new ProductOneProcessor(_administrationServiceMock.Object,
                _ageRangeValidatorMock.Object,
                _minimumPaymentValidatorMock.Object,
                _kycServiceMock.Object,
                _busMock.Object);
        }

        [TestMethod]
        public async Task Process_When_Application_Is_Null()
        {
            // Act
            await _processor.Process(null);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Never);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>()), Times.Never);
        }

        [TestMethod]
        public async Task Process_When_Age_Is_Not_Valid()
        {
            // Arrange
            var application = CreateApplication();

            _ageRangeValidatorMock.Setup(x => x.IsRangeInValidRange(It.IsAny<int>(), 18, 39)).Returns(false);

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Never);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>()), Times.Never);
        }

        [TestMethod]
        public async Task Process_When_Minimum_Payment_Not_Received()
        {
            // Arrange
            var application = CreateApplication();

            _minimumPaymentValidatorMock.Setup(x => x.IsMinimumPaymentReceived(It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(false);

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Never);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>()), Times.Never);
        }

        [TestMethod]
        public async Task Process_When_Kyc_Failed()
        {
            // Arrange
            var application = CreateApplication();

            _kycServiceMock.Setup(x => x.GetKycReportAsync(application.Applicant))
                .ReturnsAsync(new Result<KycReport>(false, new Error("Error", "Error", "Error"),
                new KycReport(It.IsAny<Guid>(), false)));

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Never);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>()), Times.Never);
        }

        [TestMethod]
        public async Task Process_When_There_Are_No_Errors_And_Investor_Created()
        {
            // Arrange
            var application = CreateApplication();

            var investorResponse = _fixture.Create<CreateInvestorResponse>();

            _ageRangeValidatorMock.Setup(x => x.IsRangeInValidRange(It.IsAny<int>(), 18, 39)).Returns(true);
            
            _minimumPaymentValidatorMock.Setup(x => x.IsMinimumPaymentReceived(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(true);
            
            _kycServiceMock.Setup(x => x.GetKycReportAsync(application.Applicant))
                .ReturnsAsync(new Result<KycReport>(true, new Error(string.Empty, string.Empty, string.Empty),
                new KycReport(It.IsAny<Guid>(), true)));

            _administrationServiceMock.Setup(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>())).Returns(investorResponse);

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>()), Times.Once);
        }

        [TestMethod]
        public async Task Process_When_There_Are_No_Errors_But_Investor_Not_Created()
        {
            // Arrange
            var application = CreateApplication();

            var investorResponse = new CreateInvestorResponse { InvestorId = string.Empty };

            _ageRangeValidatorMock.Setup(x => x.IsRangeInValidRange(It.IsAny<int>(), 18, 39)).Returns(true);

            _minimumPaymentValidatorMock.Setup(x => x.IsMinimumPaymentReceived(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns(true);

            _kycServiceMock.Setup(x => x.GetKycReportAsync(application.Applicant))
                .ReturnsAsync(new Result<KycReport>(true, new Error(string.Empty, string.Empty, string.Empty),
                new KycReport(It.IsAny<Guid>(), true)));

            _administrationServiceMock.Setup(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>())).Returns(investorResponse);

            // Act
            await _processor.Process(application);

            // Assert
            _administrationServiceMock.Verify(x => x.CreateInvestor(It.IsAny<CreateInvestorRequest>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>()), Times.Never);
        }
    }
}
