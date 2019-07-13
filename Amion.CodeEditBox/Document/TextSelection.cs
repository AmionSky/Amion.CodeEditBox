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
        public CoreTextRange Range { get => _range; }

        // If the _range starts and ends at the same point,
        // then it represents the location of the caret (insertion point).
        private CoreTextRange _range;

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

        public bool HasSelection()
        {
            return _range.StartCaretPosition != _range.EndCaretPosition;
        }

        /// <summary>
        /// Move the caret to the specified position
        /// </summary>
        /// <param name="position">Position to move the caret</param>
        public void SetPosition(int position)
        {
            _range.StartCaretPosition = ClampPosition(position);
            _range.EndCaretPosition = ClampPosition(position);

            _selectionChanged();
        }

        public void SetRange(CoreTextRange range, bool extendingLeft = false)
        {
            _range = range;
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
            if (!HasSelection())
            {
                _extendingLeft = direction < 0;
            }

            // Move the active edge
            if (_extendingLeft)
            {
                _range.StartCaretPosition = ClampPosition(_range.StartCaretPosition + direction);
            }
            else
            {
                _range.EndCaretPosition = ClampPosition(_range.EndCaretPosition + direction);
            }

            _selectionChanged();
        }

        private int ClampPosition(int position)
        {
            return Math.Clamp(position, 0, _textDocument.Length());
        }
    }
}
