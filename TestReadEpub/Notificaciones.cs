using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Notifications.Wpf.Core;

namespace Gabriel.Cat.S.Extension
{
    public delegate bool NotificationsIsOnDelegate();
    public static class Notificaciones
    {
        public static TimeSpan DefaultNotificationDurationTime = TimeSpan.FromSeconds(30);
        public static string NameControlNotifications = default;
        static NotificationManager Manager = new NotificationManager();
        static SortedList<string, List<Guid>> DicControlNotifications = new SortedList<string, List<Guid>>();


        public static async Task<Guid> ShowMessage(string title, string content, NotificationType notificationType = NotificationType.Information, TimeSpan? duracion = default, string nameControl = default, Action<Guid> onClick = default, Action<Guid> onClose = default, CancellationToken token = default, NotificationsIsOnDelegate notificacionesOn = default)
        {
            Guid id;
            if (Equals(notificacionesOn, default) || notificacionesOn())
            {
                if (Equals(nameControl, default) && !Equals(NameControlNotifications, default))
                    nameControl = NameControlNotifications;

                id = await new NotificationContent() { Title = title, Message = content, Type = notificationType }.ShowMessage(duracion, nameControl, onClick, onClose, token);
                if (!Equals(nameControl, default) && !DicControlNotifications.ContainsKey(nameControl))
                    DicControlNotifications.Add(nameControl, new List<Guid>());
                if (DicControlNotifications.ContainsKey(nameControl))
                    DicControlNotifications[nameControl].Add(id);
            }
            else id = default;
            return id;
        }

        public static async Task<Guid> ShowMessage(this NotificationContent content, TimeSpan? duracion = default, string nameControl = default, Action<Guid> onClick = default, Action<Guid> onClose = default, CancellationToken token = default)
        {
            Guid id = Guid.NewGuid();

            if (Equals(duracion, default))
                duracion = DefaultNotificationDurationTime;

            if (Equals(nameControl, default) && !Equals(NameControlNotifications, default))
                nameControl = NameControlNotifications;

            await Manager.ShowAsync(id, content, nameControl, duracion, onClick, onClose, token);

            return id;
        }
        public static async Task CloseMessage(this Guid id)
        {
            await Manager.CloseAsync(id);
        }
        public static async Task CloseAllMessages()
        {
            await Manager.CloseAllAsync();
        }
        public static async Task CloseAllMessages(string nameControl)
        {
            if (DicControlNotifications.ContainsKey(nameControl))
            {
                await Task.WhenAll(DicControlNotifications[nameControl].Convert((id) => id.CloseMessage()));
                DicControlNotifications[nameControl].Clear();

            }
        }
    }
}
