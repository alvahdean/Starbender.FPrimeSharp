using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Starbender.FPrimeSharp.Gds.Extensions;
using Starbender.FPrimeSharp.Gds.Topology;
using System.Threading.Tasks;

namespace Starbender.FPrimeSharp.Gds.Blazor.Pages.Gds;

public partial class Index : GdsComponentBase
{
    [Inject] IConfiguration Configuration { get; set; } = null!;

    private TopologyDictionary? _topolopgy;

    protected override Task OnInitializedAsync()
    {
        _topolopgy = Configuration.GetTopology();

        return base.OnInitializedAsync();
    }
}
