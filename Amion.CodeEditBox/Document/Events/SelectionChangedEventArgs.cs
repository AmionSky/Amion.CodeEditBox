using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox.Document.Events
{
    class SelectionChangedEventArgs
    {
        public CoreTextRange Selection { get; }

        public SelectionChangedEventArgs(CoreTextRange selection)
        {
            Selection = selection;
        }
    }
}
