using Amion.CodeEditBox.Document.Events;
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
        /// Selection range in Text Element indexes.
        /// If the _range starts and ends at the same point,
        /// then it represents the location of the caret (insertion point).
        /// </summary>
        public SelectionRange Range { get => _range; }
        private SelectionRange _range;

        // If there is a nonempty selection, then _extendingLeft is true if the user
        // is using shift+arrow to adjust the starting point of the selection,
        // or false if the user is adjusting the ending point of the selection.
        private bool _extendingLeft = false;

        // Should be called when the selection changes
        private Action _selectionChanged;
        
        private readonly TextDocument _textDocument;

        public TextSelection(TextDocument textDocument, Action selectionChanged)
        {
            _textDocument = textDocument;
            _selectionChanged = selectionChanged;
        }

        /// <summary>
        /// Move the caret to the beginning
        /// </summary>
        public void Reset()
        {
            _range.StartPosition = 0;
            _range.EndPosition = 0;

            _selectionChanged();
        }

        public void SetRange(SelectionRange range, bool extendingLeft = false)
        {
            _range = ClampRange(range);
            _extendingLeft = extendingLeft;

            _selectionChanged();
        }

        public void SetRange(CoreTextRange range, bool extendingLeft = false)
        {
            SetRange(_textDocument.TextBuffer.TextRangeToSelectionRange(range), extendingLeft);
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
                _range.StartPosition = ClampPosition(_range.StartPosition + direction);
            }
            else
            {
                _range.EndPosition = ClampPosition(_range.EndPosition + direction);
            }

            _selectionChanged();
        }

        private SelectionRange ClampRange(SelectionRange range)
        {
            return new SelectionRange()
            {
                StartPosition = ClampPosition(range.StartPosition),
                EndPosition = ClampPosition(range.EndPosition)
            };
        }

        private int ClampPosition(int position)
        {
            return Math.Clamp(position, 0, _textDocument.TextBuffer.ElementLength);
        }
    }
}
