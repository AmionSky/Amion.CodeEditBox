using Amion.CodeEditBox.Document.Events;
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
        public string Text { get => _text; }

        public TextActions Actions { get; }

        // The TextEditContext lets us communicate with the input system.
        CoreTextEditContext _editContext;

        // We will use a plain text string to represent the
        // content of the custom text edit control.
        string _text = string.Empty;

        public TextDocument(CoreTextEditContext editContext)
        {
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

        public void ReplaceText(CoreTextRange modifiedRange, string text)
        {
            // Modify the internal text store.
            _text = _text.Substring(0, modifiedRange.StartCaretPosition) + 
                text + _text.Substring(modifiedRange.EndCaretPosition);

            // Move the caret to the end of the replacement text.
            Selection.SetPosition(modifiedRange.StartCaretPosition + text.Length);

            // Let the CoreTextEditContext know what changed.
            _editContext.NotifyTextChanged(modifiedRange, text.Length, Selection.Range);

            // Raise text changed event.
            OnTextChanged(new TextChangedEventArgs(modifiedRange, text.Length, Selection.Range));
        }

        public int Length()
        {
            return _text.Length;
        }

        public void SetText(string newText)
        {
            var range = new CoreTextRange
            {
                StartCaretPosition = 0,
                EndCaretPosition = _text.Length
            };

            _text = newText;

            Selection.SetPosition(0);
            OnTextChanged(new TextChangedEventArgs(range, _text.Length, Selection.Range));
        }

        #region EditContext

        // Return the specified range of text. Note that the system may ask for more text
        // than exists in the text buffer.
        private void EditContext_TextRequested(CoreTextEditContext sender, CoreTextTextRequestedEventArgs args)
        {
            CoreTextTextRequest request = args.Request;
            request.Text = _text.Substring(
                request.Range.StartCaretPosition,
                Math.Min(request.Range.EndCaretPosition, _text.Length) - request.Range.StartCaretPosition);
        }

        private void EditContext_SelectionRequested(CoreTextEditContext sender, CoreTextSelectionRequestedEventArgs args)
        {
            CoreTextSelectionRequest request = args.Request;
            request.Selection = Selection.Range;
        }

        private void EditContext_TextUpdating(CoreTextEditContext sender, CoreTextTextUpdatingEventArgs args)
        {
            CoreTextRange range = args.Range;
            string newText = args.Text;
            CoreTextRange newSelection = args.NewSelection;

            // Modify the internal text store.
            _text = _text.Substring(0, range.StartCaretPosition) +
                newText + _text.Substring(Math.Min(_text.Length, range.EndCaretPosition));

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
            CoreTextRange selection = Selection.Range;

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(selection));
            _editContext.NotifySelectionChanged(selection);
        }

        #endregion
    }
}
