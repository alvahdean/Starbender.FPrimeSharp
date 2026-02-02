using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Starbender.FPrimeSharp.Bridge.Samples;

public interface ISampleAppService : IApplicationService
{
    Task<SampleDto> GetAsync();

    Task<SampleDto> GetAuthorizedAsync();
}
