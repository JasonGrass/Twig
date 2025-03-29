using System.Diagnostics;

namespace Jgrass.Twig;

[DebuggerDisplay("[TwigTreeNode] {View?.GetType().Name + ' ' + ViewModel?.GetType().Name}")]
public class TwigTreeNode<TView>
    where TView : class
{
    private readonly WeakReference<TView> _viewRef;
    private readonly WeakReference<object> _viewModelRef;

    public TView? View
    {
        get
        {
            if (_viewRef.TryGetTarget(out var view))
            {
                return view;
            }
            return null;
        }
    }

    public object? ViewModel
    {
        get
        {
            if (_viewModelRef.TryGetTarget(out var viewModel))
            {
                return viewModel;
            }
            return null;
        }
    }

    public TwigTreeNode<TView>? Parent { get; set; }

    public HashSet<TwigTreeNode<TView>> Children { get; } = [];

    public bool IsAlive => _viewRef.TryGetTarget(out _) && _viewModelRef.TryGetTarget(out _);

    public bool IsMatch(object obj)
    {
        if (obj == null!)
        {
            return false;
        }

        if (_viewRef.TryGetTarget(out var v) && v == obj)
        {
            return true;
        }

        if (_viewModelRef.TryGetTarget(out var vm) && vm == obj)
        {
            return true;
        }

        return false;
    }

    public bool IsSame(TwigTreeNode<TView> other)
    {
        if (other == null!)
        {
            return false;
        }

        if (this == other)
        {
            return true;
        }

        if (
            _viewRef.TryGetTarget(out var v)
            && other._viewRef.TryGetTarget(out var otherV)
            && v == otherV
        )
        {
            return true;
        }
        return false;
    }

    public TwigTreeNode(TView view, object viewModel)
    {
        _viewRef = new WeakReference<TView>(view);
        _viewModelRef = new WeakReference<object>(viewModel);
    }
}
