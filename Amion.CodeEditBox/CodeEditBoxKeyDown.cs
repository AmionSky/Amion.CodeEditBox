using Amion.CodeEditBox.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox
{
    public sealed partial class CodeEditBox
    {
        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Do not process keyboard input if the custom edit control does not have focus.
            if (!_internalFocus)
            {
                return;
            }

            //! For the purpose of this sample, we will support only the left and right
            //! arrow keys and the backspace key. A more complete text edit control
            //! would also handle keys like Home, End, and Delete, as well as
            //! hotkeys like Ctrl+V to paste.
            //!
            //! Note that this sample does not properly handle surrogate pairs
            //! nor does it handle grapheme clusters.

            switch (args.VirtualKey)
            {
                case VirtualKey.Back:
                    Key_Backspace();
                    break;
                case VirtualKey.Delete:
                    Key_Delete();
                    break;
                case VirtualKey.Left:
                    Key_LeftArrow();
                    break;
                case VirtualKey.Right:
                    Key_RightArrow();
                    break;
            }
        }

        private void Key_Backspace()
        {
            // If there is a selection, then delete the selection.
            if (!_textDocument.Selection.Range.IsEmpty())
            {
                _textDocument.Actions.DeleteSelected();
            }
            else
            {
                _textDocument.Actions.DeletePreviousChar();
            }
        }

        private void Key_Delete()
        {
            // If there is a selection, then delete the selection.
            if (!_textDocument.Selection.Range.IsEmpty())
            {
                _textDocument.Actions.DeleteSelected();
            }
            else
            {
                _textDocument.Actions.DeleteNextChar();
            }
        }

        private void Key_LeftArrow()
        {
            // If the shift key is down, then adjust the size of the selection.
            if (_coreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
            {
                _textDocument.Actions.AdjustSelectionLeft();
            }
            else
            {
                // The shift key is not down.
                _textDocument.Actions.StepLeft();
            }
        }

        private void Key_RightArrow()
        {
            // If the shift key is down, then adjust the size of the selection.
            if (_coreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
            {
                _textDocument.Actions.AdjustSelectionRight();
            }
            else
            {
                // The shift key is not down.
                _textDocument.Actions.StepRight();
            }
        }
    }
}
