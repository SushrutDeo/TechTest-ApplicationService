using Services.Common.Abstractions.Model;

namespace Services.Common.Abstractions.Abstractions;

public interface IKycService
{
    Task<Result<KycReport>> GetKycReportAsync(User user);
}