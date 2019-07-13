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
    class TextDocument
    {
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public TextSelection Selection { get; }
        public TextBuffer TextBuffer { get; }
        public TextActions Actions { get; }

        // The TextEditContext lets us communicate with the input system.
        CoreTextEditContext _editContext;

        public TextDocument(CoreTextEditContext editContext)
        {
            TextBuffer = new TextBuffer();
            _editContext = editContext;

            // The system raises this event to request a specific range of text.
            _editContext.TextRequested += EditContext_TextRequested;
            // The system raises this event to request the current selection.
            _editContext.SelectionRequested += EditContext_SelectionRequested;
            // The system raises this event to update text in the edit control.
            _editContext.TextUpdating += EditContext_TextUpdating;
            // The system raises this event to change the selection in the edit control.
            _editContext.SelectionUpdating += EditContext_SelectionUpdating;
            // The system raises this event when it wants the edit control
            // to apply formatting on a range of text.
            _editContext.FormatUpdating += EditContext_FormatUpdating;

            Actions = new TextActions(this);
            Selection = new TextSelection(this, OnSelectionChanged);

        }

        /// <summary>
        /// Replaces some portion of the text in the text buffer
        /// </summary>
        /// <param name="modifiedRange">The range of text to modify in character indexes.</param>
        /// <param name="text">The text to insert.</param>
        public void ReplaceText(CoreTextRange modifiedRange, string text)
        {
            // Modify the internal text store.
            TextBuffer.InsertByCharIndex(text, modifiedRange.StartCaretPosition, modifiedRange.EndCaretPosition);

            // Change range to point at the end of the inserted text
            CoreTextRange newSelection = modifiedRange;
            newSelection.StartCaretPosition = newSelection.StartCaretPosition + text.Length;
            newSelection.EndCaretPosition = newSelection.StartCaretPosition;

            // Move the caret to the end of the replacement text.
            Selection.SetRange(newSelection);
            
            // Let the CoreTextEditContext know what changed.
            _editContext.NotifyTextChanged(modifiedRange, text.Length, newSelection);

            // Raise text changed event.
            OnTextChanged(new TextChangedEventArgs(modifiedRange, text.Length, newSelection));
        }

        public void ReplaceText(SelectionRange modifiedRange, string text)
        {
            ReplaceText(modifiedRange.ToCoreText(TextBuffer), text);
        }

        public void SetText(string newText)
        {
            // Always use character indexes when sending events out
            var range = new CoreTextRange
            {
                StartCaretPosition = 0,
                EndCaretPosition = TextBuffer.CharLength
            };

            // Set the new text and reset the selection to 0
            TextBuffer.SetText(newText);
            Selection.Reset();

            // Selection has been reset so "newSelection" can be an empty CoreTextRange.
            OnTextChanged(new TextChangedEventArgs(range, newText.Length, new CoreTextRange()));
        }

        #region EditContext

        // Return the specified range of text. Note that the system may ask for more text
        // than exists in the text buffer.
        private void EditContext_TextRequested(CoreTextEditContext sender, CoreTextTextRequestedEventArgs args)
        {
            CoreTextTextRequest request = args.Request;
            CoreTextRange range = request.Range;
            request.Text = TextBuffer.Text.Substring(
                range.StartCaretPosition,
                Math.Min(range.EndCaretPosition, TextBuffer.CharLength) - range.StartCaretPosition);
        }

        private void EditContext_SelectionRequested(CoreTextEditContext sender, CoreTextSelectionRequestedEventArgs args)
        {
            CoreTextSelectionRequest request = args.Request;
            request.Selection = TextBuffer.SelectionRangeToTextRange(Selection.Range);
        }

        private void EditContext_TextUpdating(CoreTextEditContext sender, CoreTextTextUpdatingEventArgs args)
        {
            CoreTextRange range = args.Range;
            CoreTextRange newSelection = args.NewSelection;
            string newText = args.Text;

            // Modify the internal text store.
            TextBuffer.InsertByCharIndex(newText, range.StartCaretPosition, Math.Min(TextBuffer.CharLength, range.EndCaretPosition));

            //! You can set the proper font or direction for the updated text based on the language by checking
            //! args.InputLanguage.  We will not do that in this sample.

            //? Modify the current selection.
            //? newSelection.EndCaretPosition = newSelection.StartCaretPosition;

            // Update the selection of the edit context.
            Selection.SetRange(newSelection);
        }

        private void EditContext_SelectionUpdating(CoreTextEditContext sender, CoreTextSelectionUpdatingEventArgs args)
        {
            // Set the new selection to the value specified by the system.
            CoreTextRange range = args.Selection;

            // Update the selection of the edit context.
            Selection.SetRange(range);
        }

        private void EditContext_FormatUpdating(CoreTextEditContext sender, CoreTextFormatUpdatingEventArgs args)
        {
            // We do not support text formatting
        }

        #endregion

        #region Events

        private void OnTextChanged(TextChangedEventArgs args)
        {
            TextChanged?.Invoke(this, args);
        }

        private void OnSelectionChanged()
        {
            CoreTextRange selection = TextBuffer.SelectionRangeToTextRange(Selection.Range);

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(selection));
            _editContext.NotifySelectionChanged(selection);
        }

        #endregion
    }
}
