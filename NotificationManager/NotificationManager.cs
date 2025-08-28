using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AEB.PlayFab.Content;
using AEB.Utilities;
using UnityEngine;

namespace AEB.Systems.Notification
{
    /// <summary>
    /// A wrapper class used for deserializing a list of notifications from JSON data.
    /// </summary>
    [System.Serializable]
    public class NotificationListWrapper
    {
        /// <summary>
        /// An array of notifications contained within the wrapper.
        /// </summary>
        public Notification[] notifications;
    }

    /// <summary>
    /// Manages the notification system, including queueing and broadcasting notifications to listeners.
    /// </summary>
    public static class NotificationManager
    {
        #region Fields

        /// <summary>
        /// List of notification listeners.
        /// </summary>
        static readonly List<NotificationUI> notificationListeners = new();

        /// <summary>
        /// Queue to store and sort notifications based on their importance.
        /// </summary>
        static readonly SortedQueue<Notification> queue = new((a, b) => b.Importance.CompareTo(a.Importance));

        /// <summary>
        /// Indicates whether the notification processing loop is running.
        /// </summary>
        static bool isProcessing;

        /// <summary>
        /// Lock object for thread synchronization.
        /// </summary>
        static readonly object lockObj = new();

        /// <summary>
        /// Cancellation token source for notification processing.
        /// </summary>
        static CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Indicates whether the notifications have been loaded into the queue.
        /// </summary>
        static bool load;

        /// <summary>
        /// Stores notifications by their ID.
        /// </summary>
        static Dictionary<int, Notification> library;

        #endregion

        #region Properties

        /// <summary>
        /// The currently active notification.
        /// </summary>
        public static Notification Current { get; set; }

        /// <summary>
        /// The remaining duration of the current notification in milliseconds.
        /// </summary>
        public static float RemainingDuration { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when a notification is processed.
        /// </summary>
        public static event Action<Notification> OnNotification;

        /// <summary>
        /// Event triggered when the notification queue is empty.
        /// </summary>
        public static event Action OnNotificationQueueEmpty;

        #endregion

        #region Public

        /// <summary>
        /// Adds a notification to the queue.
        /// </summary>
        /// <param name="notification">The notification to add.</param>
        public async static void AddNotification(Notifications notification)
        {
            if (!load)
            {
                var result = await GetNotifications();

                library = result.ToDictionary(
                    notification => notification.ID,
                    notification => notification
                );

                load = true;
            }

            var notif = library[(int)notification];
            
            if (notif == null)
                Debug.LogWarning("Notification cannot be null.");

            lock (lockObj)
            {
                if (Current != null &&
                    notif.Importance > NotificationImportance.High &&
                    notif.Importance > Current.Importance)
                {
                    queue.Enqueue(Current);
                    KillCurrent();
                }

                queue.Enqueue(notif);
            }

            StartProcessing();
        }

        /// <summary>
        /// Terminates the current notification.
        /// </summary>
        public static void KillCurrent()
        {
            lock (lockObj)
            {
                Current = null;
            }
        }

        /// <summary>
        /// Removes a specific notification from the queue.
        /// </summary>
        /// <param name="notification">The notification to remove.</param>
        public static void KillAny(Notification notification)
        {
            if (notification == null) return;

            lock (lockObj)
            {
                queue.Kill(notification);
            }
        }

        /// <summary>
        /// Clears all notifications from the queue.
        /// </summary>
        public static void ClearNotifications()
        {
            lock (lockObj)
            {
                queue.Clear();
                Current = null;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Starts the notification processing loop.
        /// </summary>
        static void StartProcessing()
        {
            if (isProcessing) return;

            cancellationTokenSource = new CancellationTokenSource();
            _ = ProcessQueue(cancellationTokenSource.Token);
        }

        /// <summary>
        /// Continuously processes the queue, dispatching notifications to listeners until the queue is empty or processing is canceled.
        /// </summary>
        /// <param name="token">A cancellation token that can be used to cancel the processing.</param>
        static async Task ProcessQueue(CancellationToken token)
        {
            isProcessing = true;

            while (queue.Count() > 0 && !token.IsCancellationRequested)
            {
                int listeners = OnNotification?.GetInvocationList().Length ?? 0;
                if (listeners == 0) continue;

                Notification notification = null;

                lock (lockObj)
                {
                    if (queue.Count() > 0)
                        notification = queue.Pop();
                }

                Current = notification;
                OnNotification?.Invoke(notification);

                TimeSpan duration = TimeSpan.FromSeconds(notification.Duration);
                DateTime startTime = DateTime.UtcNow;

                while (!token.IsCancellationRequested)
                {
                    TimeSpan elapsed = DateTime.UtcNow - startTime;

                    if (elapsed >= duration || Current == null)
                        break;

                    RemainingDuration = (float)(duration - elapsed).TotalMilliseconds;
                    await Task.Delay(1000, token);
                } 
            }

            Current = null;
            isProcessing = false;
            OnNotificationQueueEmpty?.Invoke();
        }

        /// <summary>
        /// Asynchronously retrieves a list of notifications from persistent storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation and contains the list of notifications.</returns>
        static async Task<Notification[]> GetNotifications()
        {
            var tcs = new TaskCompletionSource<Notification[]>();

            await TitleData.GetTitleData(TitleData.Keys.System.Notification.NOTIFICATIONS, (key, val) =>
            {
                if (!string.IsNullOrEmpty(val))
                {
                    var wrapper = JsonUtility.FromJson<NotificationListWrapper>(val);
                    if (wrapper != null && wrapper.notifications != null)
                        tcs.SetResult(wrapper.notifications);
                    else
                        tcs.SetResult(new Notification[] { });
                }
                else
                    tcs.SetResult(new Notification[] { });
            });

            return await tcs.Task;
        }

        #endregion
    }
}
