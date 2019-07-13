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

            // Make the control focusable
            IsTabStop = true;

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

        private void UpdateTextUI()
        {
            // The raw materials we have are a string and information about where
            // the caret/selection is. We can render the control any way we like.
            var selection = _textDocument.Selection.Range;

            BeforeSelectionText.Text = _textDocument.TextBuffer.SubstringByTextElements(0, selection.StartPosition);
            if (!_textDocument.Selection.Range.IsEmpty())
            {
                // There is a selection. Draw that.
                CaretText.Visibility = Visibility.Collapsed;
                SelectionText.Text = _textDocument.TextBuffer.SubstringByTextElements(
                    selection.StartPosition, 
                    selection.EndPosition - selection.StartPosition);
            }
            else
            {
                // There is no selection. Remove it.
                SelectionText.Text = "";

                // Draw the caret if we have focus.
                CaretText.Visibility = _internalFocus ? Visibility.Visible : Visibility.Collapsed;
            }

            AfterSelectionText.Text = _textDocument.TextBuffer.SubstringByTextElements(selection.EndPosition);

            // Update statistics for demonstration purposes.
            FullText.Text = _textDocument.TextBuffer.Text;
            SelectionStartIndexText.Text = selection.StartPosition.ToString();
            SelectionEndIndexText.Text = selection.EndPosition.ToString();
        }

        private void UpdateFocusUI()
        {
            InterFocus.Text = _internalFocus.ToString();
            BorderPanel.BorderBrush = _internalFocus ? new SolidColorBrush(Windows.UI.Colors.Green) : null;
        }

        #region Events

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
                Focus(FocusState.Programmatic);

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
