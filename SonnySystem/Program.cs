namespace SonnySystem;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            try 
            {
                // Initialize Database
                Data.DatabaseService.Initialize();
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro fatal ao iniciar o sistema: {ex.Message}\n\nDetalhes: {ex.StackTrace}", "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }    
}