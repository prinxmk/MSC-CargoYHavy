namespace MSCCargoYHavy;
internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        using var mutex = new Mutex(true, @"Local\MSC-CargoYHavy-SingleInstance", out bool createdNew);
        if (!createdNew)
        {
            MessageBox.Show("MSC CargoYHavy is already running.", AppConfig.AppName,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        Application.Run(new MainForm());
    }
}
