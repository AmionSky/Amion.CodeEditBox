using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace Amion.CodeEditBox
{
    public sealed partial class CodeEditBox
    {
        private void SetInternalFocus()
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

        private void RemoveInternalFocus()
        {
            if (_internalFocus)
            {
                //Notify the system that this edit context is no longer in focus
                _editContext.NotifyFocusLeave();

                RemoveInternalFocusWorker();
            }
        }

        private void RemoveInternalFocusWorker()
        {
            //Update the internal notion of focus
            _internalFocus = false;

            //x Ask the software keyboard to dismiss.
            //x _inputPane.TryHide();

            // Update our UI.
            UpdateTextUI();
            UpdateFocusUI();
        }

        private void EditContext_FocusRemoved(CoreTextEditContext sender, object args)
        {
            RemoveInternalFocusWorker();
        }
    }
}
