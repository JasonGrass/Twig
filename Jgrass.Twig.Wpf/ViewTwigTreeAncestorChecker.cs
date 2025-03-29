using System.Windows;

namespace Jgrass.Twig.Wpf;

public class ViewTwigTreeAncestorChecker : ITwigTreeAncestorChecker<FrameworkElement>
{
    public bool IsAncestor(FrameworkElement? child, FrameworkElement? ancestor)
    {
        if (ancestor == null || child == null)
        {
            return false;
        }

        if (child == ancestor)
        {
            return false;
        }

        DependencyObject? current = child;
        while (current != null)
        {
            if (current == ancestor)
            {
                return true;
            }
            current = LogicalTreeHelper.GetParent(current);
        }
        return false;
    }
}
