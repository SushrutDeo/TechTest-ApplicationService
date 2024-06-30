using AutoFixture;

using Moq;

using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using Services.Validations;

namespace Services.Tests
{
    public abstract class ApplicationProcessorTests
    {
        protected Mock<IAgeRangeValidator> _ageRangeValidatorMock = new();
        protected Mock<IMinimumPaymentValidator> _minimumPaymentValidatorMock = new();
        protected Mock<IKycService> _kycServiceMock = new();
        protected Mock<IBus> _busMock = new();
        protected Fixture _fixture = new();

        protected Application CreateApplication()
        {
            User user = new()
            {
                Addresses = _fixture.Create<Address[]>(),
                BankAccounts = _fixture.Create<BankAccount[]>(),
                Forename = "John",
                Surname = "Doe",
                Id = Guid.NewGuid(),
                IsVerified = true,
                Nino = "Nino",
                DateOfBirth = new DateOnly(2005, 1, 1)
            };

            var application = _fixture.Build<Application>().With(x => x.Applicant, user).Create();

            return application;
        }
    }
}
