namespace XRSharp.CommunityToolkit.Networked;

/// <summary>
/// https://github.com/networked-aframe/networked-aframe#adapters
/// </summary>
public enum Adapter
{
    /// <summary>
    /// Uses the open-easyrtc library.
    /// </summary>
    wseasyrtc,

    /// <summary>
    /// Uses the open-easyrtc library.
    /// </summary>
    easyrtc,

    /// <summary>
    /// Uses the Janus WebRTC server and janus-plugin-sfu.
    /// </summary>
    janus,

    /// <summary>
    /// SocketIO implementation without external library.
    /// </summary>
    socketio,

    /// <summary>
    /// Native WebRTC implementation without external library.
    /// </summary>
    webrtc,

    /// <summary>
    /// Firebase for WebRTC signaling.
    /// </summary>
    Firebase,

    /// <summary>
    /// Implementation of uWebSockets.
    /// </summary>
    uWS,
}
