using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

public class Library
{
    private IBackgroundTaskRegistration _registration;

    public void Save(string value)
    {
        ApplicationData.Current.LocalSettings.Values["value"] = value;
    }

    public bool Init()
    {
        if (BackgroundTaskRegistration.AllTasks.Count > 0)
        {
            _registration = BackgroundTaskRegistration.AllTasks.Values.First();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> Toggle()
    {
        if (BackgroundTaskRegistration.AllTasks.Count > 0)
        {
            _registration = BackgroundTaskRegistration.AllTasks.Values.First();
            _registration.Unregister(true);
            _registration = null;
            return false;
        }
        else
        {
            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder
                {
                    Name = typeof(Agent.Background.Trigger).FullName
                };
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.TimeZoneChange, false));
                builder.TaskEntryPoint = builder.Name;
                builder.Register();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}