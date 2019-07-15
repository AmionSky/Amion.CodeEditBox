using Amion.CodeEditBox.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amion.CodeEditBox.Document
{
    class TextActions
    {
        TextDocument _document;

        public TextActions(TextDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// Deletes the currently selected text from the document.
        /// </summary>
        public void DeleteSelected()
        {
            var range = _document.Selection.Range;

            // Set the text in the selection to nothing.
            _document.ReplaceText(range, "");
        }

        /// <summary>
        /// Deletes the previous (left) char from the caret.
        /// Selection size is expected to be zero
        /// </summary>
        public void DeletePreviousChar()
        {
            var range = _document.Selection.Range;

            // Delete the character to the left of the caret, if one exists,
            // by creating a range that encloses the character to the left
            // of the caret, and setting the contents of that range to nothing.
            int start = _document.TextBuffer.GetIndexWithElementOffset(range.StartCaretPosition, -1);
            range.StartCaretPosition = start;
            _document.ReplaceText(range, "");
        }

        /// <summary>
        /// Move the caret to the left one char
        /// </summary>
        public void StepLeft()
        {
            var range = _document.Selection.Range;

            // If there was a selection, then snap the caret at the left edge of the selection.
            // TODO: RTL languages?
            if (!range.IsEmpty())
            {
                _document.Selection.SetPosition(range.StartCaretPosition);
            }
            else
            {
                // There was no selection. Move the caret left one code unit if possible.
                int position = _document.TextBuffer.GetIndexWithElementOffset(range.StartCaretPosition, -1);
                _document.Selection.SetPosition(position);
            }
        }

        /// <summary>
        /// Move the caret to the right one char
        /// </summary>
        public void StepRight()
        {
            var range = _document.Selection.Range;

            // If there was a selection, then snap the caret at the right edge of the selection.
            // TODO: RTL languages?
            if (!range.IsEmpty())
            {
                _document.Selection.SetPosition(range.EndCaretPosition);
            }
            else
            {
                // There was no selection. Move the caret right one code unit if possible.
                int position = _document.TextBuffer.GetIndexWithElementOffset(range.StartCaretPosition, +1);
                _document.Selection.SetPosition(position);
            }
        }

        public void AdjustSelectionLeft()
        {
            _document.Selection.AdjustSelectionEndpoint(-1);
        }

        public void AdjustSelectionRight()
        {
            _document.Selection.AdjustSelectionEndpoint(+1);
        }
    }
}
