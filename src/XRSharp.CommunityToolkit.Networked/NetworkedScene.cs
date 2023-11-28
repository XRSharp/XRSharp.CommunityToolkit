using System.Collections.Generic;
using XRSharp.Components;
using XRSharp.Controls;
using XRSharp.Primitives;

namespace XRSharp.CommunityToolkit.Networked;

/// <summary>
/// Build multi-user experience in XR#.
/// </summary>
public class NetworkedScene : RootComponent
{
    private const string AvatarTemplateId = "avatarTemplate";
    private const string ResourcesPath = "ms-appx:///XRSharp.CommunityToolkit.Networked/Resources";

    // todo: load different files for different adapters
    public override IEnumerable<ResourceFile> Files => new ResourceFile[]
    {
        new() { Type = ResourceType.Script, Source =  $"{ResourcesPath}/socket.io.slim.js" },
        new() { Type = ResourceType.Script, Source =  $"{ResourcesPath}/easyrtc.js" },
        new() { Type = ResourceType.Script, Source =  $"{ResourcesPath}/networked-aframe.min.js" },
    };

    /// <summary>
    /// Choose where the WebSocket / signaling server is located.
    /// </summary>
    public string ServerURL { get; set; } = "/";

    /// <summary>
    /// Unique app name. Spaces are not allowed.
    /// </summary>
    public string AppName { get; set; } = "default";

    /// <summary>
    /// Unique room name. Can be multiple per app. Spaces are not allowed.
    /// There can be multiple rooms per app and clients can only connect to clients in the same app & room.
    /// </summary>
    public string Room { get; set; } = "default";

    /// <summary>
    /// Connect to the server as soon as the webpage loads. Default is <see langword="true"/>.
    /// If set to <see langword="false"/> then call <see cref="Connect"/> manually.
    /// </summary>
    public bool ConnectOnLoad { get; set; } = true;

    /// <summary>
    /// The network service that you wish to use, see <see cref="CommunityToolkit.Networked.Adapter"/>.
    /// </summary>
    public Adapter Adapter { get; set; } = Adapter.wseasyrtc;

    /// <summary>
    /// Turn on/off microphone audio streaming for your app. Only works if the chosen <see cref="Adapter"/> supports it.
    /// </summary>
    public bool Audio { get; set; } = false;

    /// <summary>
    /// Turn on/off video streaming for your app. Only works if the chosen <see cref="Adapter"/> supports it.
    /// </summary>
    public bool Video { get; set; } = false;

    /// <summary>
    /// Turn on/off Networked-Aframe debug logs.
    /// </summary>
    public bool Debug { get; set; } = false;

    public bool IsConnected => Interop.ExecuteJavaScriptGetResult<bool>("NAF.connection.isConnected();");

    /// <summary>
    /// When a user connects, other clients will display it as this avatar template.
    /// </summary>
    public FrameworkElement3D Avatar { get; set; } = new Box { SizeX = 0.5, SizeY = 0.5 };

    // todo: add onConnect, events

    protected override void Init()
    {
        Interop.ExecuteJavaScriptVoid(
            $"{Root.JsElement}.setAttribute('networked-scene', 'serverURL: {ServerURL}; app: {AppName}; room: {Room}; connectOnLoad: {ConnectOnLoad.ToLowerString()}; adapter: {Adapter}; audio: {Audio.ToLowerString()}; video: {Video.ToLowerString()}; debug: {Debug.ToLowerString()};');");

        if (Avatar != null)
        {
            Interop.ExecuteJavaScriptVoid($@"
const template = document.createElement('template');
template.setAttribute('id', '{AvatarTemplateId}');
{Root.JsElement}.appendChild(template);
");

            if (Root.Content is Panel3D panel)
            {
                Avatar.Loaded += (_, __) =>
                {
                    Interop.ExecuteJavaScriptVoid($@"
const template = document.getElementById('{AvatarTemplateId}');
const el = {Avatar.JsElement}.cloneNode(true);
template.content.appendChild(el);
");
                    panel.Children.Remove(Avatar);
                    SetAvatarToCamera();
                };

                panel.Children.Add(Avatar);
            }
            else
            {
                // todo:
                Interop.ExecuteJavaScriptVoid($@"
const template = document.getElementById('{AvatarTemplateId}');
const el = document.createElement('a-box');
template.content.appendChild(el);
");

                SetAvatarToCamera();
            }
        }
    }

    private void SetAvatarToCamera()
    {
        Interop.ExecuteJavaScriptVoid($"{Root.JsElement}.camera.el.setAttribute('networked', 'attachTemplateToLocal: false; template: #{AvatarTemplateId};');");
    }

    protected override void Remove()
    {
        Interop.ExecuteJavaScriptVoid($@"
{Root.JsElement}.camera.el.removeAttribute('networked');
{Root.JsElement}.removeAttribute('networked-scene');
document.getElementById('{AvatarTemplateId}')?.remove();
");
    }

    /// <summary>
    /// Manually connect to the server if <see cref="ConnectOnLoad"/> is <see langword="false"/>.
    /// </summary>
    public void Connect()
    {
        Interop.ExecuteJavaScriptVoid($"{Root.JsElement}.emit('connect');");
    }
}
