using System.Windows;
using System.Windows.Controls;

namespace FlashCard.ExtensionMethod
{
    public static class ViewBoxExtensions
    {
        public static double GetScaleFactor(this Viewbox viewbox)
        {
            if (viewbox.Child == null ||
                (viewbox.Child is FrameworkElement) == false)
            {
                return double.NaN;
            }
            FrameworkElement child = viewbox.Child as FrameworkElement;
            return viewbox.ActualHeight / child.ActualHeight;
        }
    }
}
