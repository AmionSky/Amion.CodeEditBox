using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amion.CodeEditBox.Document
{
    class TextActions
    {
        TextDocument _textDocument;

        public TextActions(TextDocument textDocument)
        {
            _textDocument = textDocument;
        }

        /// <summary>
        /// Deletes the currently selected text from the document.
        /// </summary>
        public void DeleteSelected()
        {
            // Set the text in the selection to nothing.
            _textDocument.ReplaceText(_textDocument.Selection.Range, "");
        }

        /// <summary>
        /// Deletes the previous (left) char from the caret.
        /// Selection size is expected to be zero
        /// </summary>
        public void DeletePreviousChar()
        {
            var range = _textDocument.Selection.Range;

            // Delete the character to the left of the caret, if one exists,
            // by creating a range that encloses the character to the left
            // of the caret, and setting the contents of that range to nothing.
            range.StartCaretPosition = Math.Max(0, range.StartCaretPosition - 1);
            _textDocument.ReplaceText(range, "");
        }

        /// <summary>
        /// Move the caret to the left one char
        /// </summary>
        public void StepLeft()
        {
            var range = _textDocument.Selection.Range;

            // If there was a selection, then snap the caret at the left edge of the selection.
            // TODO: RTL languages?
            if (_textDocument.Selection.HasSelection())
            {
                _textDocument.Selection.SetPosition(range.StartCaretPosition);
            }
            else
            {
                // There was no selection. Move the caret left one code unit if possible.
                _textDocument.Selection.SetPosition(range.StartCaretPosition - 1);
            }
        }

        /// <summary>
        /// Move the caret to the right one char
        /// </summary>
        public void StepRight()
        {
            var range = _textDocument.Selection.Range;

            // If there was a selection, then snap the caret at the right edge of the selection.
            // TODO: RTL languages?
            if (_textDocument.Selection.HasSelection())
            {
                _textDocument.Selection.SetPosition(range.EndCaretPosition);
            }
            else
            {
                // There was no selection. Move the caret right one code unit if possible.
                _textDocument.Selection.SetPosition(range.StartCaretPosition + 1);
            }
        }

        public void AdjustSelectionLeft()
        {
            _textDocument.Selection.AdjustSelectionEndpoint(-1);
        }

        public void AdjustSelectionRight()
        {
            _textDocument.Selection.AdjustSelectionEndpoint(+1);
        }
    }
}
