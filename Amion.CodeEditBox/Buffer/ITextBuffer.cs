using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amion.CodeEditBox.Buffer
{
    interface ITextBuffer
    {
        int CharLength { get; }

        string Text { get; }

        int GetIndexWithElementOffset(int currentIndex, int offset);
    }
}
