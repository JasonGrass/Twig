using System.Runtime.CompilerServices;
using Jgrass.Twig.Exceptions;

namespace Jgrass.Twig;

public class TwigProvider<TView> : IViewTwigProvider
    where TView : class
{
    private readonly ConditionalWeakTable<object, object> _store = new();

    private readonly TwigTree<TView> _tree;

    public TwigProvider(TwigTree<TView> tree)
    {
        _tree = tree;
    }

    public void Provide<T>(object owner, T data)
        where T : class
    {
        if (!_store.TryGetValue(owner, out object? value))
        {
            _store.Add(owner, data);
            return;
        }

        var storeType = value.GetType();
        var dataType = data.GetType();

        if (value == data && storeType != dataType)
        {
            throw new ViewTwigException(
                "For the same owner, the same data cannot be set with different types."
            );
        }

        if (storeType == dataType)
        {
            throw new ViewTwigException("For the same owner, duplicate data types cannot be set.");
        }

        _store.Add(owner, data);
    }

    public T? Get<T>(object self)
        where T : class
    {
        if (_store.TryGetValue(self, out object? value) && value is T t)
        {
            return t;
        }

        var ancestors = _tree.GetAllAncestors(self);
        foreach (var ancestor in ancestors)
        {
            if (
                ancestor.View != null
                && _store.TryGetValue(ancestor.View, out var ancestorValue1)
                && ancestorValue1 is T ancestorT1
            )
            {
                return ancestorT1;
            }

            if (
                ancestor.ViewModel != null
                && _store.TryGetValue(ancestor.ViewModel, out var ancestorValue2)
                && ancestorValue2 is T ancestorT2
            )
            {
                return ancestorT2;
            }
        }

        return null;
    }
}
