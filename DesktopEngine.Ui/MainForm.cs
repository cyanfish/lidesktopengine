using Eto.Forms;
using Eto.Drawing;

namespace DesktopEngine.Ui
{
    public partial class MainForm : Form
    {
        private const int STATUS_UPDATE_INTERVAL = 1000;

        private readonly Label _statusLabel = new Label();
        private readonly Timer _statusUpdateTimer;

        public MainForm()
        {
            Title = UiResources.TitleText;
            MinimumSize = new Size(400, 100);

            Content = new StackLayout
            {
                Padding = 10,
                Items =
                {
                    new Label
                    {
                        Text = UiResources.WelcomeMessage
                    },
                    new StackLayout
                    {
                        Padding = 10,
                        Spacing = 10,
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            new Button
                            {
                                Text = UiResources.Setup
                            },
                            new Button
                            {
                                Text = UiResources.Start,
                                Enabled = false
                            }
                        }
                    }
                }
            };

            var aboutCommand = new Command { MenuText = UiResources.About };
            aboutCommand.Executed += (_, _) => new AboutDialog().ShowDialog(this);

            Menu = new MenuBar
            {
                AboutItem = aboutCommand
            };

            Width = 400;

            _statusUpdateTimer = new Timer(UpdateStatus, null, 0, STATUS_UPDATE_INTERVAL);
        }

        private void UpdateStatus(object _)
        {
            var status = ExternalEngineApi.GetStatus();
            var statusText = status switch
            {
                ExternalEngineApi.NOT_STARTED => "Press Start to connect",
                ExternalEngineApi.DISCONNECTED => "Trying to connect...",
                ExternalEngineApi.CONNECTED_IDLE => "Connected, waiting for analysis request",
                ExternalEngineApi.CONNECTED_RUNNING => "Connected, analysis in progress",
                ExternalEngineApi.ENGINE_ERROR => "Error with engine setup",
                ExternalEngineApi.UNKNOWN_ERROR => "Unknown error",
                _ => "Unknown status"
            };
            _statusLabel.Text = statusText;
        }
    }
}