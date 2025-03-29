using System.Windows;
using Jgrass.Twig.Wpf;

namespace Twig.Wpf.Example.Views;

public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        InitializeComponent();
        TwigTreeUtils.Inject(this);

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await Task.Delay(1000);
        var tree = TwigTreeUtils.ViewTwigTree.PrintTree();
        MessageBox.Show(tree);
    }
}
