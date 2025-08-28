using System.Linq;
using UnityEngine;

namespace AEB.Systems.Notification
{
    /// <summary>
    /// Manages a collection of predefined notifications.
    /// </summary>
    [CreateAssetMenu(fileName = "Notification", menuName = "AEB/NotificationLibrary", order = 0)]
    public class NotificationLibrary : ScriptableObject
    {
        [SerializeField]
        Notification[] _notifications;

        /// <summary>
        /// Retrieves a predefined notification by title.
        /// </summary>
        /// <param name="title">The title of the notification to retrieve.</param>
        /// <returns>The notification with the matching title, or null if not found.</returns>
        public Notification GetNotificationByTitle(string title)
        {
            foreach (var notification in _notifications)
            {
                if (notification.Title == title)
                {
                    return notification;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves all notifications.
        /// </summary>
        /// <returns>An array of all predefined notifications.</returns>
        public Notification[] GetAllNotifications()
        {
            return _notifications;
        }

        /// <summary>
        /// Validates the list to ensure no duplicate notifications are present.
        /// </summary>
        void OnValidate()
        {
            if (_notifications == null || _notifications.Length == 0)
                return;

            var uniqueNotifications = _notifications
                .GroupBy(notification => notification.Title)
                .Select(group => group.First())
                .ToArray();

            if (uniqueNotifications.Length != _notifications.Length)
            {
                Debug.LogWarning("Duplicate notifications detected and removed.");
                _notifications = uniqueNotifications;
            }
        }
    }
}