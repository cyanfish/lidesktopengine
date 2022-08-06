using Eto.Forms;

namespace DesktopEngine.Ui;

public static class EtoHelpers
{
    public static void ShowSingletonForm<T>() where T : Form, new()
    {
        var alreadyOpenForm = (T)Application.Instance.Windows.FirstOrDefault(x => x is T);
        if (alreadyOpenForm != null)
        {
            alreadyOpenForm.BringToFront();
        }
        else
        {
            new T().Show();
        }
    }
}