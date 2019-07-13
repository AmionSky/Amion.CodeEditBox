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

        public CoreTextRange Selection { get => _selection; }
        public string Text { get => _text; }
        public bool ExtendingLeft { get => _extendingLeft; set => _extendingLeft = value; }

        // The TextEditContext lets us communicate with the input system.
        CoreTextEditContext _editContext;

        // We will use a plain text string to represent the
        // content of the custom text edit control.
        string _text = string.Empty;

        // If the _selection starts and ends at the same point,
        // then it represents the location of the caret (insertion point).
        CoreTextRange _selection;

        // If there is a nonempty selection, then _extendingLeft is true if the user
        // is using shift+arrow to adjust the starting point of the selection,
        // or false if the user is adjusting the ending point of the selection.
        bool _extendingLeft = false;

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
        }

        public void ReplaceText(CoreTextRange modifiedRange, string text)
        {
            // Modify the internal text store.
            _text = _text.Substring(0, modifiedRange.StartCaretPosition) + 
                text + _text.Substring(modifiedRange.EndCaretPosition);

            // Move the caret to the end of the replacement text.
            _selection.StartCaretPosition = modifiedRange.StartCaretPosition + text.Length;
            _selection.EndCaretPosition = _selection.StartCaretPosition;

            // Update the selection of the edit context. There is no need to notify the system
            // of the selection change because we are going to call NotifyTextChanged soon.
            SetSelection(_selection);

            // Let the CoreTextEditContext know what changed.
            _editContext.NotifyTextChanged(modifiedRange, text.Length, _selection);

            // Raise text changed event.
            OnTextChanged(new TextChangedEventArgs(modifiedRange, text.Length, _selection));
        }

        public bool HasSelection()
        {
            return _selection.StartCaretPosition != _selection.EndCaretPosition;
        }

        // Change the selection without notifying CoreTextEditContext of the new selection.
        public void SetSelection(CoreTextRange selection)
        {
            // Modify the internal selection.
            _selection = selection;

            // Raise selection changed event.
            OnSelectionChanged(new SelectionChangedEventArgs(selection));
        }

        // Change the selection and notify CoreTextEditContext of the new selection.
        public void SetSelectionAndNotify(CoreTextRange selection)
        {
            SetSelection(selection);
            _editContext.NotifySelectionChanged(_selection);
        }

        // Adjust the active endpoint of the selection in the specified direction.
        public void AdjustSelectionEndpoint(int direction)
        {
            CoreTextRange range = _selection;
            if (_extendingLeft)
            {
                range.StartCaretPosition = Math.Max(0, range.StartCaretPosition + direction);
            }
            else
            {
                range.EndCaretPosition = Math.Min(_text.Length, range.EndCaretPosition + direction);
            }

            SetSelectionAndNotify(range);
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
            request.Selection = _selection;
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

            // Update the selection of the edit context. There is no need to notify the system
            // because the system itself changed the selection.
            SetSelection(newSelection);
        }

        private void EditContext_SelectionUpdating(CoreTextEditContext sender, CoreTextSelectionUpdatingEventArgs args)
        {
            // Set the new selection to the value specified by the system.
            CoreTextRange range = args.Selection;

            // Update the selection of the edit context. There is no need to notify the system
            // because the system itself changed the selection.
            SetSelection(range);
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

        private void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            SelectionChanged?.Invoke(this, args);
        }

        #endregion
    }
}
