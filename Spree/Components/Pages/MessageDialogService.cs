using Spree.Components.Pages.OtherPages;

namespace Spree.Components.Pages
{
    public class MessageDialogService
    {
        public MessageDialog? messageDialog { get; set; }
        public bool ShowBusyButton { get; set; }
        public bool ShowSaveButton { get; set; } = true;
        public Action? Action { get; set; }

        public async void SetMessageDialog()
        {
            if (messageDialog != null)
            {
                await messageDialog.ShowMessage();
                ShowBusyButton = false;
                ShowSaveButton = !ShowBusyButton;
                Action?.Invoke();
            }            
        }
    }
}
