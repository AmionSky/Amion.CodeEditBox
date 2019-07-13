using Amion.CodeEditBox.Document;
using Amion.CodeEditBox.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Text.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Amion.CodeEditBox
{
    public sealed partial class CodeEditBox : UserControl
    {
        // The CoreWindow that contains our control.
        CoreWindow _coreWindow;

        // The TextEditContext lets us communicate with the input system.
        CoreTextEditContext _editContext;

        //x The input pane object indicates the visibility of the on screen keyboard.
        //x Apps can also ask the keyboard to show or hide.
        //x InputPane _inputPane;

        // _internalFocus keeps track of whether our control acts like it has focus.
        bool _internalFocus = false;

        // The Document where the text is stored and modifications handled
        TextDocument _textDocument;

        public CodeEditBox()
        {
            InitializeComponent();

            Unloaded += CodeEditBox_Unloaded;


            // The CoreTextEditContext processes text input, but other keys are
            // the apps's responsibility.
            _coreWindow = CoreWindow.GetForCurrentThread();
            _coreWindow.KeyDown += CoreWindow_KeyDown;
            _coreWindow.PointerPressed += CoreWindow_PointerPressed;

            // Create a CoreTextEditContext for our custom edit control.
            CoreTextServicesManager manager = CoreTextServicesManager.GetForCurrentView();
            _editContext = manager.CreateEditContext();

            // Create a TextDocument where we will store the text data
            _textDocument = new TextDocument(_editContext);
            _textDocument.TextChanged += TextDocument_TextChanged;
            _textDocument.SelectionChanged += TextDocument_SelectionChanged;

            //x Get the Input Pane so we can programmatically hide and show it.
            //x _inputPane = InputPane.GetForCurrentView();

            //! Automatic hide and show the Input Pane. Note that on Desktop, you will need to
            //! implement the UIA text pattern to get expected automatic behavior.
            _editContext.InputPaneDisplayPolicy = CoreTextInputPaneDisplayPolicy.Automatic;
            // Set the input scope to inform software keyboard layout and text behavior.
            _editContext.InputScope = CoreTextInputScope.Default;

            // The system raises this event when it wants the edit control to remove focus.
            _editContext.FocusRemoved += EditContext_FocusRemoved;
            // The system raises this event to request layout information.
            // This is used to help choose a position for the IME candidate window.
            _editContext.LayoutRequested += EditContext_LayoutRequested;
            // The system raises this event to notify the edit control
            // that the string composition has started.
            _editContext.CompositionStarted += EditContext_CompositionStarted;
            // The system raises this event to notify the edit control
            // that the string composition is finished.
            _editContext.CompositionCompleted += EditContext_CompositionCompleted;
            // The system raises this event when the NotifyFocusLeave operation has
            // completed.
            // _editContext.NotifyFocusLeaveCompleted += EditContext_NotifyFocusLeaveCompleted;

            // Focus state reporter
            // TODO: remove this
            IsTabStop = true;
            GotFocus += CodeEditBox_FocusChanged;
            LostFocus += CodeEditBox_FocusChanged;
            CodeEditBox_FocusChanged(this, null);


            // Set our initial UI.
            UpdateTextUI();
            UpdateFocusUI();
        }

        private void TextDocument_SelectionChanged(object sender, Document.Events.SelectionChangedEventArgs e)
        {
            //Update the UI to show the new selection.
            UpdateTextUI();
        }

        private void TextDocument_TextChanged(object sender, Document.Events.TextChangedEventArgs e)
        {
            // Nothing yet
        }

        private void CodeEditBox_Unloaded(object sender, RoutedEventArgs e)
        {
            _coreWindow.KeyDown -= CoreWindow_KeyDown;
            _coreWindow.PointerPressed -= CoreWindow_PointerPressed;
        }

        // TODO: remove this
        private void CodeEditBox_FocusChanged(object sender, RoutedEventArgs e)
        {
            OrigFocus.Text = FocusState.ToString();
        }

        #region Focus
        void SetInternalFocus()
        {
            if (!_internalFocus)
            {
                // Update internal notion of focus.
                _internalFocus = true;

                // Update the UI.
                UpdateTextUI();
                UpdateFocusUI();

                // Notify the CoreTextEditContext that the edit context has focus,
                // so it should start processing text input.
                _editContext.NotifyFocusEnter();
            }

            //x Ask the software keyboard to show.  The system will ultimately decide if it will show.
            //x For example, it will not show if there is a keyboard attached.
            //x _inputPane.TryShow();

        }

        void RemoveInternalFocus()
        {
            if (_internalFocus)
            {
                //Notify the system that this edit context is no longer in focus
                _editContext.NotifyFocusLeave();

                RemoveInternalFocusWorker();
            }
        }

        void RemoveInternalFocusWorker()
        {
            //Update the internal notion of focus
            _internalFocus = false;

            //x Ask the software keyboard to dismiss.
            //x _inputPane.TryHide();

            // Update our UI.
            UpdateTextUI();
            UpdateFocusUI();
        }
        #endregion

        private void UpdateTextUI()
        {
            // The raw materials we have are a string and information about where
            // the caret/selection is. We can render the control any way we like.
            var selection = _textDocument.Selection;

            BeforeSelectionText.Text = _textDocument.Text.Substring(0, selection.StartCaretPosition);
            if (_textDocument.HasSelection())
            {
                // There is a selection. Draw that.
                CaretText.Visibility = Visibility.Collapsed;
                SelectionText.Text = _textDocument.Text.Substring(
                    selection.StartCaretPosition, 
                    selection.EndCaretPosition - selection.StartCaretPosition);
            }
            else
            {
                // There is no selection. Remove it.
                SelectionText.Text = "";

                // Draw the caret if we have focus.
                CaretText.Visibility = _internalFocus ? Visibility.Visible : Visibility.Collapsed;
            }

            AfterSelectionText.Text = _textDocument.Text.Substring(selection.EndCaretPosition);

            // Update statistics for demonstration purposes.
            FullText.Text = _textDocument.Text;
            SelectionStartIndexText.Text = selection.StartCaretPosition.ToString();
            SelectionEndIndexText.Text = selection.EndCaretPosition.ToString();
        }

        private void UpdateFocusUI()
        {
            InterFocus.Text = _internalFocus.ToString();
            BorderPanel.BorderBrush = _internalFocus ? new SolidColorBrush(Windows.UI.Colors.Green) : null;
        }

        #region Events

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Do not process keyboard input if the custom edit control does not
            // have focus.
            if (!_internalFocus)
            {
                return;
            }

            // This holds the range we intend to operate on, or which we intend
            // to become the new selection. Start with the current selection.
            CoreTextRange range = _textDocument.Selection;

            //! For the purpose of this sample, we will support only the left and right
            //! arrow keys and the backspace key. A more complete text edit control
            //! would also handle keys like Home, End, and Delete, as well as
            //! hotkeys like Ctrl+V to paste.
            //!
            //! Note that this sample does not properly handle surrogate pairs
            //! nor does it handle grapheme clusters.

            switch (args.VirtualKey)
            {
                // Backspace
                case VirtualKey.Back:
                    // If there is a selection, then delete the selection.
                    if (_textDocument.HasSelection())
                    {
                        // Set the text in the selection to nothing.
                        _textDocument.ReplaceText(range, "");
                    }
                    else
                    {
                        // Delete the character to the left of the caret, if one exists,
                        // by creating a range that encloses the character to the left
                        // of the caret, and setting the contents of that range to nothing.
                        range.StartCaretPosition = Math.Max(0, range.StartCaretPosition - 1);
                        _textDocument.ReplaceText(range, "");
                    }
                    break;

                // Left arrow
                case VirtualKey.Left:
                    // If the shift key is down, then adjust the size of the selection.
                    if (_coreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                    {
                        // If this is the start of a selection, then remember which edge we are adjusting.
                        if (!_textDocument.HasSelection())
                        {
                            _textDocument.ExtendingLeft = true;
                        }

                        // Adjust the selection and notify CoreTextEditContext.
                        _textDocument.AdjustSelectionEndpoint(-1);
                    }
                    else
                    {
                        // The shift key is not down. If there was a selection, then snap the
                        // caret at the left edge of the selection.
                        if (_textDocument.HasSelection())
                        {
                            range.EndCaretPosition = range.StartCaretPosition;
                            _textDocument.SetSelectionAndNotify(range);
                        }
                        else
                        {
                            // There was no selection. Move the caret left one code unit if possible.
                            range.StartCaretPosition = Math.Max(0, range.StartCaretPosition - 1);
                            range.EndCaretPosition = range.StartCaretPosition;
                            _textDocument.SetSelectionAndNotify(range);
                        }
                    }
                    break;

                // Right arrow
                case VirtualKey.Right:
                    // If the shift key is down, then adjust the size of the selection.
                    if (_coreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                    {
                        // If this is the start of a selection, then remember which edge we are adjusting.
                        if (!_textDocument.HasSelection())
                        {
                            _textDocument.ExtendingLeft = false;
                        }

                        // Adjust the selection and notify CoreTextEditContext.
                        _textDocument.AdjustSelectionEndpoint(+1);
                    }
                    else
                    {
                        // The shift key is not down. If there was a selection, then snap the
                        // caret at the right edge of the selection.
                        if (_textDocument.HasSelection())
                        {
                            range.StartCaretPosition = range.EndCaretPosition;
                            _textDocument.SetSelectionAndNotify(range);
                        }
                        else
                        {
                            // There was no selection. Move the caret right one code unit if possible.
                            range.StartCaretPosition = Math.Min(_textDocument.Text.Length, range.StartCaretPosition + 1);
                            range.EndCaretPosition = range.StartCaretPosition;
                            _textDocument.SetSelectionAndNotify(range);
                        }
                    }
                    break;
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            // See whether the pointer is inside or outside the control.
            Rect contentRect = LayoutHelper.GetElementRect(BorderPanel);
            if (contentRect.Contains(args.CurrentPoint.Position))
            {
                // The user tapped inside the control. Give it focus.
                SetInternalFocus();

                // Tell XAML that this element has focus, so we don't have two
                // focus elements. That is the extent of our integration with XAML focus.
                bool s = Focus(FocusState.Programmatic);

                //! A more complete custom control would move the caret to the
                //! pointer position. It would also provide some way to select
                //! text via touch. We do neither in this sample.
            }
            else
            {
                // The user tapped outside the control. Remove focus.
                RemoveInternalFocus();
            }
        }

        

        private void EditContext_FocusRemoved(CoreTextEditContext sender, object args)
        {
            RemoveInternalFocusWorker();
        }

        private void EditContext_LayoutRequested(CoreTextEditContext sender, CoreTextLayoutRequestedEventArgs args)
        {
            CoreTextLayoutRequest request = args.Request;

            // Get the screen coordinates of the entire control and the selected text.
            // This information is used to position the IME candidate window.

            // First, get the coordinates of the edit control and the selection
            // relative to the Window.
            Rect contentRect = LayoutHelper.GetElementRect(ContentPanel);
            Rect selectionRect = LayoutHelper.GetElementRect(SelectionText);

            // Next, convert to screen coordinates in view pixels.
            Rect windowBounds = Window.Current.CoreWindow.Bounds;
            contentRect.X += windowBounds.X;
            contentRect.Y += windowBounds.Y;
            selectionRect.X += windowBounds.X;
            selectionRect.Y += windowBounds.Y;

            // Finally, scale up to raw pixels.
            double scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

            contentRect = LayoutHelper.ScaleRect(contentRect, scaleFactor);
            selectionRect = LayoutHelper.ScaleRect(selectionRect, scaleFactor);

            // This is the bounds of the selection.
            // Note: If you return bounds with 0 width and 0 height, candidates will not appear while typing.
            request.LayoutBounds.TextBounds = selectionRect;

            //This is the bounds of the whole control
            request.LayoutBounds.ControlBounds = contentRect;
        }

        // This indicates that an IME has started composition. If there is no handler for this event,
        // then composition will not start.
        private void EditContext_CompositionStarted(CoreTextEditContext sender, CoreTextCompositionStartedEventArgs args)
        {
            // TODO?
        }

        private void EditContext_CompositionCompleted(CoreTextEditContext sender, CoreTextCompositionCompletedEventArgs args)
        {
            // TODO?
        }

        #endregion
    }
}
