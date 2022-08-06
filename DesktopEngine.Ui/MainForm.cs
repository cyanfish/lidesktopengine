using Eto.Forms;
using Eto.Drawing;

namespace DesktopEngine.Ui
{
    public partial class MainForm : Form
    {
        private const int STATUS_UPDATE_INTERVAL = 1000;

        private readonly Button _startButton = new Button
        {
            Text = UiResources.Start
        };
        private readonly Button _setupButton = new Button
        {
            Text = UiResources.Setup
        };
        private readonly Label _statusLabel = new Label();
        private readonly Timer _statusUpdateTimer;

        public MainForm()
        {
            Title = UiResources.TitleText;
            MinimumSize = new Size(400, 100);

            var setupCommand = new Command();
            setupCommand.Executed += (_, _) => new SetupForm().Show();
            _setupButton.Command = setupCommand;

            var startCommand = new Command();
            startCommand.Executed += (_, _) => { };
            _startButton.Command = startCommand;

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
                            _setupButton,
                            _startButton
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

            UpdateStatus();
            _statusUpdateTimer = new Timer(_ => UpdateStatus(), null, 0, STATUS_UPDATE_INTERVAL);
            UserConfig.Saved += (_, _) => UpdateStatus();
        }

        private void UpdateStatus()
        {
            var config = UserConfig.Load();
            if (!config.IsValid)
            {
                _statusLabel.Text = "";
                _startButton.Enabled = false;
                return;
            }
            _startButton.Enabled = true;

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