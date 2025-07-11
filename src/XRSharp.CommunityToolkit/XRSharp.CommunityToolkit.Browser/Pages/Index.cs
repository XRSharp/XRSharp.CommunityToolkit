using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using OpenSilver.WebAssembly;
using System.Threading.Tasks;

namespace XRSharp.CommunityToolkit.Browser.Pages;

[Route("/")]
public class Index : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
    }

    protected override async Task OnInitializedAsync()
    {
        await Runner.RunApplicationAsync(async () =>
        {
            await Root3D.Initialize();
            return new CommunityToolkit.App();
        });
    }
}
