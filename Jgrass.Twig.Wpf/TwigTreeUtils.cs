using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Jgrass.Twig.Exceptions;

namespace Jgrass.Twig.Wpf;

public static class TwigTreeUtils
{
    public static readonly ViewTwigTree ViewTwigTree = new ViewTwigTree();

    public static readonly ViewTwigProvider ViewTwigProvider = new ViewTwigProvider(ViewTwigTree);

    public static void Inject(FrameworkElement view)
    {
        if (view == null!)
        {
            throw new ArgumentNullException(nameof(view));
        }

        if (view.IsLoaded)
        {
            var viewModel = view.DataContext;
            if (viewModel == null)
            {
                throw new ViewTwigException($"No view model if view. {view.GetType().Name}");
            }
            ViewTwigTree.Add(view, viewModel);
        }
        else
        {
            view.Loaded += ViewOnLoaded;
            void ViewOnLoaded(object sender, RoutedEventArgs e)
            {
                view.Loaded -= ViewOnLoaded;
                var viewModel = view.DataContext;
                if (viewModel == null)
                {
                    throw new ViewTwigException($"No view model if view. {view.GetType().Name}");
                }
                ViewTwigTree.Add(view, viewModel);
            }
        }
    }

    public static T? Get<T>(object self)
        where T : class
    {
        return ViewTwigProvider.Get<T>(self);
    }

    public static void Provide<T>(object owner, T data)
        where T : class
    {
        ViewTwigProvider.Provide(owner, data);
    }
}
