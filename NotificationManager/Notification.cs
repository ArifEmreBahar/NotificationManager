using Sirenix.OdinInspector;
using UnityEngine;

namespace AEB.Systems.Notification
{
    /// <summary>
    /// Provides colors corresponding to different levels of notification importance.
    /// </summary>
    public struct ImportanceColor
    {
        /// <summary>
        /// Gets the color associated with a given notification importance level.
        /// </summary>
        /// <param name="importance">The importance level of the notification.</param>
        /// <returns>The color corresponding to the specified importance level.</returns>
        public static Color GetColor(NotificationImportance importance) =>
            importance switch
            {
                NotificationImportance.Low => Low,
                NotificationImportance.Normal => new Color(0,200,255),
                NotificationImportance.High => High,
                NotificationImportance.Critical => Critical,
                _ => Normal 
            };

        /// <summary>
        /// Color representing low importance notifications.
        /// </summary>
        public static Color Low => Color.white;

        /// <summary>
        /// Color representing normal importance notifications.
        /// </summary>
        public static Color Normal => new Color(0, 200, 255);

        /// <summary>
        /// Color representing high importance notifications.
        /// </summary>
        public static Color High => Color.yellow;

        /// <summary>
        /// Color representing critical importance notifications.
        /// </summary>
        public static Color Critical => Color.red;
    }

    /// <summary>
    /// Enum representing the importance level of a notification.
    /// </summary>
    public enum NotificationImportance : byte
    {
        /// <summary>
        /// Low importance level, for less critical notifications.
        /// </summary>
        Low = 0,

        /// <summary>
        /// Normal importance level, for standard notifications.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// High importance level, for significant notifications requiring attention.
        /// </summary>
        High = 2,

        /// <summary>
        /// Critical importance level, for urgent and high-priority notifications.
        /// </summary>
        Critical = 3
    }

    /// <summary>
    /// Represents a notification with properties such as title, message, icon, duration, and importance.
    /// </summary>
    [System.Serializable]
    [InlineEditor]
    [CreateAssetMenu(fileName = "Notification", menuName = "AEB/Notification", order = 0)]
    public class Notification
    {
        /// <summary>
        /// The unique identifier for the notification.
        /// </summary>
        public int ID;

        /// <summary>
        /// The title of the notification.
        /// </summary>
        [Title("Title")]
        [HideLabel]
        public string Title;

        /// <summary>
        /// The message content of the notification.
        /// </summary>
        [Title("Message")]
        [HideLabel]
        [TextArea]
        public string Message;

        /// <summary>
        /// The icon associated with the notification.
        /// </summary>
        public Sprite Icon;

        /// <summary>
        /// The duration (in seconds) for which the notification is displayed. Default is 3 seconds.
        /// </summary>
        [Range(3, 10)]
        public float Duration = 3f;

        /// <summary>
        /// The importance level of the notification.
        /// </summary>
        [EnumToggleButtons]
        public NotificationImportance Importance;

        /// <summary>
        /// Returns a string representation of the notification, including its title, message, duration, and importance.
        /// </summary>
        /// <returns>A string summarizing the notification's properties.</returns>
        public override string ToString()
        {
            return $"Notification: [Title: {Title}, Message: {Message}, Duration: {Duration} seconds, Importance: {Importance}]";
        }
    }
}