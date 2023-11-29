using System.Windows.Controls;
using XRSharp.CommunityToolkit.Networked;

namespace XRSharp.CommunityToolkit.Examples;
public partial class Networked : Page
{
    public Networked()
    {
        InitializeComponent();

        box.Color = RandomColor.Generate();
    }
}
