namespace Client_TaskManager
{
    /*Написать сетевое приложение «Диспетчер задач», 
позволяющее: 
 отобразить 
список 
удаленного хоста; 
запущенных 
процессов 
 завершить на удаленном хосте процесс, выбранный 
из списка; 
 обновить список процессов; 
 создать новый процесс на удаленном хосте (путь к 
исполняемому файлу вводится в текстовое поле 
ввода).*/
    internal static class Program
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
            Application.Run(new FormClient());
        }
    }
}