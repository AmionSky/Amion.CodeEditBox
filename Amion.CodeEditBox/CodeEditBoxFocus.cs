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
                UpdateUI();

                // Notify the CoreTextEditContext that the edit context has focus,
                // so it should start processing text input.
                _editContext.NotifyFocusEnter();
            }
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

            // Update our UI.
            UpdateUI();
        }

        private void EditContext_FocusRemoved(CoreTextEditContext sender, object args)
        {
            RemoveInternalFocusWorker();
        }
    }
}
