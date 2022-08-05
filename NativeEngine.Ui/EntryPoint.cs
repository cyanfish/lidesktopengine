using Eto;
using Eto.Forms;

namespace NativeEngine.Ui;

public class EntryPoint
{
    public static void Main()
    {
        new Application(Platform.Detect).Run(new MainForm());
    }
}