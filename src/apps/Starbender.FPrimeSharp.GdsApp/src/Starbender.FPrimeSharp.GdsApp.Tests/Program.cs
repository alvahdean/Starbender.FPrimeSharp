using Microsoft.AspNetCore.Builder;
using Starbender.FPrimeSharp.GdsApp;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Starbender.FPrimeSharp.GdsApp.csproj");
await builder.RunAbpModuleAsync<GdsAppTestModule>(applicationName: "Starbender.FPrimeSharp.GdsApp");
namespace Starbender.FPrimeSharp.GdsApp
{
    public partial class Program
    {
    }
}
