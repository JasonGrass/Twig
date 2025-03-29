using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jgrass.Twig.Wpf;

namespace Twig.Wpf.Example.Views;

public partial class FirstSubView : UserControl
{
    public FirstSubView()
    {
        InitializeComponent();

        TwigTreeUtils.Inject(this);
    }
}
