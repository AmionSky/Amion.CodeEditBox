using Amion.CodeEditBox.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox.Document
{
    struct SelectionRange : IEquatable<SelectionRange>
    {
        public int StartPosition;
        public int EndPosition;

        public SelectionRange(int position)
        {
            StartPosition = position;
            EndPosition = position;
        }

        public bool IsEmpty()
        {
            return StartPosition == EndPosition;
        }

        public int Size()
        {
            return EndPosition - StartPosition;
        }

        public CoreTextRange ToCoreText(TextBuffer textBuffer)
        {
            return textBuffer.SelectionRangeToTextRange(this);
        }

        public bool Equals(SelectionRange other)
        {
            return StartPosition == other.StartPosition && EndPosition == other.EndPosition;
        }
    }
}
