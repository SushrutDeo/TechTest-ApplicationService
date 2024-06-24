using Services.Common.Abstractions.Model;

namespace Services.Common.Abstractions.Abstractions;

public interface IApplicationProcessor  
{
    Task Process(Application application);
}