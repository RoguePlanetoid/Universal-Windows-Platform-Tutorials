using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Background
{
    public sealed class Trigger : Windows.ApplicationModel.Background.IBackgroundTask
    {
        public void Run(Windows.ApplicationModel.Background.IBackgroundTaskInstance taskInstance)
        {
            try
            {
                string value =
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("value") ?
                    (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["value"] :
                    string.Empty;
                Windows.Data.Xml.Dom.XmlDocument xml =
                    Windows.UI.Notifications.ToastNotificationManager
                    .GetTemplateContent(Windows.UI.Notifications.ToastTemplateType.ToastText02);
                xml.GetElementsByTagName("text")[0].InnerText = "Time Zone Changed";
                xml.GetElementsByTagName("text")[1].InnerText = value;
                Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().Show(
                    new Windows.UI.Notifications.ToastNotification(xml));
            }
            catch
            {

            }
        }
    }
}
