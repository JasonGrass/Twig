using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jgrass.Twig.Exceptions;

public class ViewTwigException : Exception
{
    public ViewTwigException(string? message)
        : base(message) { }

    public ViewTwigException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
