using AutoProcrun;
using System.ServiceProcess;

namespace AutoUpdateService
{
    public partial class Service1 : ServiceBase
    {
        private System.Threading.Thread myWorkingThread;
        private System.Timers.Timer myTimer = new System.Timers.Timer();

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            DoSmth(null, null);
            myWorkingThread = new System.Threading.Thread(PrepareTask);
            myWorkingThread.Start();
        }

        private void PrepareTask()
        {
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(DoSmth);
            myTimer.Interval = 20 * 1000; // Каждые 20 секунд работает сервис 
            myTimer.Start();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        void DoSmth(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Планируется парсить папку сервиса, чтобы автообновление работало для нескольких сервисов
            //Путь к настройкам сервиса, пока что писать вручную
            string path = @"D:\VSProjects\AutoProcrun\AutoProcrun\bin\Debug\JavaServices\settings\AJavaServiceTest_settings.xml";
            
            Service service = Service.LoadSettings(path);

            if (service.autoUpdate)
            {
                //В папке latest_version находится последняя версия сервиса и если у него Specification-Version не совпадает происходит замена файла и перезапуск сервиса
                if (service.GetVersion(service.latestVersionPath) != service.GetVersion(service.classpath))
                {
                    System.IO.File.Copy(service.latestVersionPath, service.classpath, true);
                    string command = "sc stop " + service.serviceID;
                    CmdCommand.ExecuteCommandSync(command);
                    command = "sc start " + service.serviceID;
                    CmdCommand.ExecuteCommandSync(command);
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}
