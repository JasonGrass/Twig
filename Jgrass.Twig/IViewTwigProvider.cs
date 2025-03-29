using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jgrass.Twig;

public interface IViewTwigProvider
{
    public void Provide<T>(object owner, T data)
        where T : class;

    public T? Get<T>(object self)
        where T : class;
}
