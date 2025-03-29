namespace Jgrass.Twig;

public interface ITwigTreeAncestorChecker<in TView>
    where TView : class
{
    bool IsAncestor(TView? child, TView? ancestor);
}