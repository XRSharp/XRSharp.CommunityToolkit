using System.Collections.Generic;
using System.Windows;
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
        new() { Type = ResourceType.Script, Source =  $"{ResourcesPath}/networked-hand-tracking.js" },
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
    /// There can be multiple rooms per app and clients can only connect to clients in the same app and room.
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

    /// <summary>
    /// Whether user hands should be visible to others.
    /// Currently only dots model is supported.
    /// Default is <see langword="false"/>.
    /// </summary>
    public bool HandTrackingEnabled { get; set; }

    public bool IsConnected => IsInitialized && Interop.ExecuteJavaScriptGetResult<bool>("NAF.connection.isConnected();");

    /// <summary>
    /// When a user connects, other clients will display it as this avatar template.
    /// </summary>
    public FrameworkElement3D Avatar { get; set; } = new Box { SizeX = 0.3, SizeY = 0.5, SizeZ = 0.3, Color = RandomColor.Generate() };

    #region IsMicrophoneEnabled
    public bool IsMicrophoneEnabled
    {
        get => (bool)GetValue(IsMicrophoneEnabledProperty);
        set => SetValue(IsMicrophoneEnabledProperty, value);
    }

    public static readonly DependencyProperty IsMicrophoneEnabledProperty =
        DependencyProperty.Register(nameof(IsMicrophoneEnabled), typeof(bool), typeof(NetworkedScene), new PropertyMetadata(true, OnIsMicrophoneEnabledChanged));

    private static void OnIsMicrophoneEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var component = (NetworkedScene)d;

        if (component.Audio && component.IsInitialized)
        {
            component.UpdateMicState();
        }
    }

    private void UpdateMicState()
    {
        Interop.ExecuteJavaScriptVoid($@"
if (NAF.connection.isConnected()) {{
  NAF.connection.adapter.enableMicrophone({IsMicrophoneEnabled.ToLowerString()});
}}
else {{
  document.body.addEventListener('clientConnected', function (evt) {{
    NAF.connection.adapter.enableMicrophone({IsMicrophoneEnabled.ToLowerString()})
  }});
}}");
    }
    #endregion

    // todo: add onConnect, events

    protected override void Init()
    {
        Interop.ExecuteJavaScriptVoid(
            $"{Root.JsElement}.setAttribute('networked-scene', 'serverURL: {ServerURL}; app: {AppName}; room: {Room}; connectOnLoad: {ConnectOnLoad.ToLowerString()}; adapter: {Adapter}; audio: {Audio.ToLowerString()}; video: {Video.ToLowerString()}; debug: {Debug.ToLowerString()};');");

        if (Avatar != null)
        {
            Interop.ExecuteJavaScriptVoid($@"
var assets = {Root.JsElement}.querySelector('a-assets');
const template = document.createElement('template');
template.setAttribute('id', '{AvatarTemplateId}');
assets.appendChild(template);

const defaultTemplate = document.createElement('template');
defaultTemplate.setAttribute('id', 'defaultTemplate');
defaultTemplate.content.appendChild(document.createElement('a-entity'));
assets.appendChild(defaultTemplate);
");

            if (Root.Content is Panel3D panel)
            {
                Avatar.Loaded += (_, __) =>
                {
                    Interop.ExecuteJavaScriptVoid($@"
const template = document.getElementById('{AvatarTemplateId}');
const el = {Avatar.JsElement}.cloneNode(true);
if ({Audio.ToLowerString()})
  el.setAttribute('networked-audio-source', '');
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

        if (HandTrackingEnabled)
        {
            Interop.ExecuteJavaScriptVoid(@$"
var assets = {Root.JsElement}.querySelector('a-assets');
const handJointTemplate = document.createElement('template');
handJointTemplate.setAttribute('id', 'hand-joint-template');

const jointSphere = document.createElement('a-sphere');
jointSphere.setAttribute('radius', 0.01);

handJointTemplate.content.appendChild(jointSphere);
assets.appendChild(handJointTemplate);

const handElements = document.querySelectorAll('[hand-tracking-controls]');

for (var i = 0; i < handElements.length; i++) {{
  var hand = handElements[i];
  hand.setAttribute('hand-tracking-controls', 'modelStyle: dots');
  hand.setAttribute('networked-hand-tracking', '');
}}
");
        }

        if (Audio)
        {
            UpdateMicState();
        }
    }

    private void SetAvatarToCamera()
    {
        Interop.ExecuteJavaScriptVoid($@"
var camera = {Root.JsElement}.camera.el;
camera.parentEl.setAttribute('networked', 'template: #defaultTemplate');
camera.setAttribute('networked', 'attachTemplateToLocal: false; template: #{AvatarTemplateId};');
");
    }

    protected override void Remove()
    {
        Interop.ExecuteJavaScriptVoid($@"
var scene = {Root.JsElement}
scene.camera.el.removeAttribute('networked');
scene.removeAttribute('networked-scene');
scene.getElementById('{AvatarTemplateId}')?.remove();
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
