using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Starbender.FPrimeSharp.Gds.Samples;

public interface ISampleAppService : IApplicationService
{
    Task<SampleDto> GetAsync();

    Task<SampleDto> GetAuthorizedAsync();
}
