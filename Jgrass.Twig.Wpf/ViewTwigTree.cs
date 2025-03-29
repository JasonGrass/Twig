using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Jgrass.Twig.Wpf;

public class ViewTwigTree : TwigTree<FrameworkElement>
{
    public ViewTwigTree()
        : base(new ViewTwigTreeAncestorChecker()) { }
}
