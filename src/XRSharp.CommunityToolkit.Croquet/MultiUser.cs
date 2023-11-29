using System.Windows;
using XRSharp.Components;

namespace XRSharp.CommunityToolkit.Croquet;

public class MultiUser : Component<UIElement3D>
{
    protected override string ComponentName => "multiuser";

    #region Enabled
    public static bool GetEnabled(Root3D obj) => (bool)obj.GetValue(EnabledProperty);
    public static void SetEnabled(Root3D obj, bool value) => obj.SetValue(EnabledProperty, value);

    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(MultiUser), new PropertyMetadata(false));
    #endregion

    #region Shared
    public static bool GetShared(DependencyObject obj) => (bool)obj.GetValue(SharedProperty);
    public static void SetShared(DependencyObject obj, bool value) => obj.SetValue(SharedProperty, value);

    public static readonly DependencyProperty SharedProperty =
        DependencyProperty.RegisterAttached("Shared", typeof(bool), typeof(MultiUser), new PropertyMetadata(false, Update));
    #endregion

    #region Animate
    /// <summary>
    /// Whether the element will be animated by its Croquet Model. Default: <see langword="false"/>.
    /// </summary>
    public bool Animate
    {
        get => (bool)GetValue(AnimateProperty);
        set => SetValue(AnimateProperty, value);
    }

    public static readonly DependencyProperty AnimateProperty =
        DependencyProperty.Register(nameof(Animate), typeof(bool), typeof(MultiUser), new PropertyMetadata(false, Update));
    #endregion

    private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var component = (MultiUser)d;
        component.Update();
    }

    protected override string GetProperties() => $"{{ anim: {Animate.ToLowerString()} }}";
}
