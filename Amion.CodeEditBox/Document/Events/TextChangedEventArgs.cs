using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox.Document.Events
{
    class TextChangedEventArgs
    {
        public CoreTextRange ModifiedRange { get; }
        public int NewLength { get; }
        public CoreTextRange NewSelection { get; }

        public TextChangedEventArgs(CoreTextRange modifiedRange, int newLength, CoreTextRange newSelection)
        {
            ModifiedRange = modifiedRange;
            NewLength = newLength;
            NewSelection = newSelection;
        }
    }
}
