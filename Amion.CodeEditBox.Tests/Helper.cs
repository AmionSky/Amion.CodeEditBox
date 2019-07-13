using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Amion.CodeEditBox.Tests
{
    static class Helper
    {
        public static async Task ExecuteOnUIThread(DispatchedHandler action)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
        }
    }
}
