using System.Collections.Generic;
using XRSharp.Components;
using XRSharp.Controls;
using XRSharp.Primitives;

namespace XRSharp.CommunityToolkit.Croquet;

// https://aframe.wiki/en/#!pages/multiuser.md
/// <summary>
/// Multi-user features for XR#.
/// To use it, add the instance of this class to the <see cref="Root3D.Initialize(RootComponent[])"/>
/// and.
/// https://github.com/NikolaySuslov/aframe-croquet-component
/// </summary>
public class Croquet : RootComponent
{
    private const string AvatarId = "avatarTemplate";
    private const string ResourcesPath = "ms-appx:///XRSharp.CommunityToolkit.Croquet/Resources";

    public override IEnumerable<ResourceFile> Files => new ResourceFile[]
    {
        new() { Type = ResourceType.Script, Source =  $"{ResourcesPath}/croquet.min.js" },
        new() { Type = ResourceType.Script, Source =  $"{ResourcesPath}/aframe-croquet-component.js" },
    };

    /// <summary>
    /// Default: 'demo', that is, randomly generated.
    /// </summary>
    public string SessionName { get; set; } = "demo";

    /// <summary>
    /// Default: 'demo', that is, randomly generated.
    /// </summary>
    public string Password { get; set; } = "demo";

    /// <summary>
    /// Default: 'myApiKey'. Croquet free API key could be get from https://croquet.io/keys
    /// </summary>
    public string ApiKey { get; set; } = "myApiKey";

    /// <summary>
    /// User avatars will spawn near to and facing this point.
    /// </summary>
    public Point3D SpawnPoint { get; set; } = new Point3D();

    /// <summary>
    /// When a user connects, other clients will display it as this avatar template.
    /// </summary>
    public FrameworkElement3D Avatar { get; set; } = new Box { SizeX = 0.5, SizeY = 0.5 };

    protected override void Init()
    {
        Interop.ExecuteJavaScriptVoid(
            $"{Root.JsElement}.setAttribute('croquet', 'sessionName: {SessionName}; password: {Password}; apiKey: {ApiKey}; spawnPoint: {SpawnPoint.X.ToInvariantString()} {SpawnPoint.Y.ToInvariantString()} {SpawnPoint.Z.ToInvariantString()};');");

        Interop.ExecuteJavaScriptVoid($@"
const template = document.createElement('template');
template.setAttribute('id', '{AvatarId}');
{Root.JsElement}.appendChild(template);
");

        if (Root.Content is Panel3D panel && Avatar != null)
        {
            Avatar.Loaded += (_, __) =>
            {
                Interop.ExecuteJavaScriptVoid($@"
const template = document.getElementById('{AvatarId}');
const el = {Avatar.JsElement}.cloneNode(true);
template.content.appendChild(el);
");
                panel.Children.Remove(Avatar);
            };

            panel.Children.Add(Avatar);
        }
        else
        {
            // todo:
            Interop.ExecuteJavaScriptVoid($@"
const template = document.getElementById('{AvatarId}');
const el = document.createElement('a-box');
template.content.appendChild(el);
");
        }
    }

    protected override void Remove()
    {
        Interop.ExecuteJavaScriptVoid($@"
{Root.JsElement}.removeAttribute('croquet');
document.getElementById('{AvatarId}').remove();
");
    }
}

