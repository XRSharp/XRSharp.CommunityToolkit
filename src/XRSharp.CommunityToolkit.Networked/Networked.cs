using System.Linq;
using XRSharp.Components;

namespace XRSharp.CommunityToolkit.Networked;

public class Networked : Component<UIElement3D>
{
    protected override string ComponentName => "networked";

    /// <summary>
    /// A css selector to a template tag.
    /// </summary>
    public string TemplateSelector { get; set; } = "#defaultTemplate";

    /// <summary>
    /// Does not attach the template for the local user when set to <see langword="false"/>.
    /// This is useful when there is different behavior locally and remotely.
    /// Default is <see langword="true"/>.
    /// </summary>
    public bool AttachTemplateToLocal { get; set; } = true;

    /// <summary>
    /// On remote creator (not owner) disconnect, attempts to take ownership of persistent entities rather than delete them.
    /// Default is <see langword="false"/>.
    /// </summary>
    public bool Persistent { get; set; } = false;

    protected override void Init()
    {
        Initialized = true;

        // workaround to set component only after the networked-scene is initialized
        var networkedScene = Root3D.Current.Components.OfType<NetworkedScene>().FirstOrDefault();
        if (networkedScene != null)
        {
            if (networkedScene.IsInitialized)
            {
                Update();
            }
            else
            {
                networkedScene.Initialized += (_, __) => Update();
            }
        }
    }

    protected override string GetProperties() => $"'template: {TemplateSelector}; attachTemplateToLocal: {AttachTemplateToLocal.ToLowerString()}; persistent: {Persistent.ToLowerString()};'";
}
