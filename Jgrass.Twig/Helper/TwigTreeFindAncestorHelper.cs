namespace Jgrass.Twig.Helper;

internal static class TwigTreeFindAncestorHelper<TView>
    where TView : class
{
    public static IList<TwigTreeNode<TView>>? FindAncestors(TwigTreeNode<TView> node, object obj)
    {
        if (node.IsMatch(obj))
        {
            return GetAllAncestors(node);
        }

        if (!node.Children.Any())
        {
            return null;
        }

        foreach (var child in node.Children)
        {
            var result = FindAncestors(child, obj);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private static IList<TwigTreeNode<TView>> GetAllAncestors(TwigTreeNode<TView> node)
    {
        var result = new List<TwigTreeNode<TView>>();

        var n = node;

        result.Add(n); // include self
        while (n.Parent != null)
        {
            result.Add(n.Parent);
            n = n.Parent;
        }

        return result;
    }
}
