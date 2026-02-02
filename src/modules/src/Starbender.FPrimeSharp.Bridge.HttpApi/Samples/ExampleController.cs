using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Starbender.FPrimeSharp.Bridge.Samples;

[Area(BridgeRemoteServiceConsts.ModuleName)]
[RemoteService(Name = BridgeRemoteServiceConsts.RemoteServiceName)]
[Route("api/bridge/example")]
public class ExampleController : BridgeController, ISampleAppService
{
    private readonly ISampleAppService _sampleAppService;

    public ExampleController(ISampleAppService sampleAppService)
    {
        _sampleAppService = sampleAppService;
    }

    [HttpGet]
    public async Task<SampleDto> GetAsync()
    {
        return await _sampleAppService.GetAsync();
    }

    [HttpGet]
    [Route("authorized")]
    [Authorize]
    public async Task<SampleDto> GetAuthorizedAsync()
    {
        return await _sampleAppService.GetAsync();
    }
}
