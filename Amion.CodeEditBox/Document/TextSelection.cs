using Amion.CodeEditBox.Buffer;
using Amion.CodeEditBox.Document.Events;
using Amion.CodeEditBox.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox.Document
{
    class TextSelection
    {
        /// <summary>
        /// If the _range starts and ends at the same point,
        /// then it represents the location of the caret (insertion point).
        /// </summary>
        public CoreTextRange Range { get => _range; }
        private CoreTextRange _range;

        // If there is a nonempty selection, then _extendingLeft is true if the user
        // is using shift+arrow to adjust the starting point of the selection,
        // or false if the user is adjusting the ending point of the selection.
        private bool _extendingLeft = false;

        // Should be called when the selection changes
        private Action _selectionChanged;
        
        private readonly ITextBuffer _buffer;

        public TextSelection(ITextBuffer buffer, Action selectionChanged)
        {
            _buffer = buffer;
            _selectionChanged = selectionChanged;
        }

        /// <summary>
        /// Move the caret to the beginning
        /// </summary>
        public void Reset()
        {
            _range.StartCaretPosition = 0;
            _range.EndCaretPosition = 0;

            _selectionChanged();
        }

        public void SetPosition(int position)
        {
            position = ClampPosition(position);
            _range.StartCaretPosition = position;
            _range.EndCaretPosition = position;

            _selectionChanged();
        }

        public void SetRange(CoreTextRange range, bool extendingLeft = false)
        {
            _range = ClampRange(range);
            _extendingLeft = extendingLeft;

            _selectionChanged();
        }

        /// <summary>
        /// Adjust the active endpoint of the selection in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to adjust the selection</param>
        public void AdjustSelectionEndpoint(int direction)
        {
            // If this is the start of a selection, then remember which edge we are adjusting.
            if (_range.IsEmpty())
            {
                _extendingLeft = direction < 0;
            }

            // Move the active edge
            if (_extendingLeft)
            {
                int index = _buffer.GetIndexWithElementOffset(_range.StartCaretPosition, direction);
                _range.StartCaretPosition = index;
            }
            else
            {
                int index = _buffer.GetIndexWithElementOffset(_range.EndCaretPosition, direction);
                _range.EndCaretPosition = index;
            }

            _selectionChanged();
        }

        private int ClampPosition(int position)
        {
            return Math.Clamp(position, 0, _buffer.CharLength);
        }

        private CoreTextRange ClampRange(CoreTextRange range)
        {
            return new CoreTextRange()
            {
                StartCaretPosition = ClampPosition(range.StartCaretPosition),
                EndCaretPosition = ClampPosition(range.EndCaretPosition)
            };
        }
    }
}
