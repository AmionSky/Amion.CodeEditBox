using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox.Helpers
{
    static class RangeExtension
    {
        public static int Size(this CoreTextRange range)
        {
            return range.EndCaretPosition - range.StartCaretPosition;
        }
    }
}
