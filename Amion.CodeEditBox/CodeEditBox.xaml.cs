using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        public CodeEditBox()
        {
            InitializeComponent();

            // The CoreTextEditContext processes text input, but other keys are
            // the apps's responsibility.
            _coreWindow = CoreWindow.GetForCurrentThread();
            _coreWindow.KeyDown += CoreWindow_KeyDown;
            _coreWindow.PointerPressed += CoreWindow_PointerPressed;

            // Create a CoreTextEditContext for our custom edit control.
            CoreTextServicesManager manager = CoreTextServicesManager.GetForCurrentView();
            _editContext = manager.CreateEditContext();

            //x Get the Input Pane so we can programmatically hide and show it.
            //x _inputPane = InputPane.GetForCurrentView();

            // Automatic hide and show the Input Pane. Note that on Desktop, you will need to
            // implement the UIA text pattern to get expected automatic behavior.
            _editContext.InputPaneDisplayPolicy = CoreTextInputPaneDisplayPolicy.Automatic;

            // Set the input scope to inform software keyboard layout and text behavior.
            _editContext.InputScope = CoreTextInputScope.Default;


            // The system raises this event to request a specific range of text.
            _editContext.TextRequested += EditContext_TextRequested;

            // The system raises this event to request the current selection.
            _editContext.SelectionRequested += EditContext_SelectionRequested;

            // The system raises this event when it wants the edit control to remove focus.
            _editContext.FocusRemoved += EditContext_FocusRemoved;

            // The system raises this event to update text in the edit control.
            _editContext.TextUpdating += EditContext_TextUpdating;

            // The system raises this event to change the selection in the edit control.
            _editContext.SelectionUpdating += EditContext_SelectionUpdating;

            // The system raises this event when it wants the edit control
            // to apply formatting on a range of text.
            _editContext.FormatUpdating += EditContext_FormatUpdating;

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

            // Set our initial UI.
            UpdateTextUI();
            UpdateFocusUI();
        }

        private void UpdateTextUI()
        {
            throw new NotImplementedException();
        }

        private void UpdateFocusUI()
        {
            throw new NotImplementedException();
        }

        #region Events

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_TextRequested(CoreTextEditContext sender, CoreTextTextRequestedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_SelectionRequested(CoreTextEditContext sender, CoreTextSelectionRequestedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_FocusRemoved(CoreTextEditContext sender, object args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_TextUpdating(CoreTextEditContext sender, CoreTextTextUpdatingEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_SelectionUpdating(CoreTextEditContext sender, CoreTextSelectionUpdatingEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_FormatUpdating(CoreTextEditContext sender, CoreTextFormatUpdatingEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_LayoutRequested(CoreTextEditContext sender, CoreTextLayoutRequestedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_CompositionStarted(CoreTextEditContext sender, CoreTextCompositionStartedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void EditContext_CompositionCompleted(CoreTextEditContext sender, CoreTextCompositionCompletedEventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
