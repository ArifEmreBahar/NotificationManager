using Michsky.UI.Reach;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AEB.Systems.Notification
{
    /// <summary>
    /// Represents the UI elements of a notification panel.
    /// </summary>
    [Serializable]
    public class NotificationPanel
    {
        /// <summary>
        /// The modal window manager handling the notification.
        /// </summary>
        public ModalWindowManager Window;

        /// <summary>
        /// The text element for the notification title.
        /// </summary>
        public TMP_Text Title;

        /// <summary>
        /// The text element for the notification message.
        /// </summary>
        public TMP_Text Message;

        /// <summary>
        /// The image representing the notification badge.
        /// </summary>
        public Image Badge;
    }

    /// <summary>
    /// Manages the notification UI, handling incoming notifications and their lifecycle.
    /// </summary>
    public class NotificationUI : MonoBehaviour
    {
        #region Constants


        /// <summary>
        /// Animation clip key for opening the modal window.
        /// </summary>
        const string CLIP_IN_KEY = "ModalWindow_In";

        /// <summary>
        /// Animation clip key for closing the modal window.
        /// </summary>
        const string CLIP_OUT_KEY = "ModalWindow_Out";

        /// <summary>
        /// Safety tolerance duration to ensure smooth transitions.
        /// </summary>
        const float SAFETY_TOLERANCE = 0.1f;

        #endregion

        #region Fields

        /// <summary>
        /// The panel displaying the notification details.
        /// </summary>
        [SerializeField] 
        NotificationPanel _panel;

        /// <summary>
        /// Maximum duration for the open state animation.
        /// </summary>
        float _maxOpenStateLength = 0.35f;

        /// <summary>
        /// Duration for the close state animation.
        /// </summary>
        float _closeStateLength = 0.15f;

        #endregion

        #region Unity

        void Awake()
        {
            var animator = _panel.Window?.GetComponent<Animator>();
            if (animator != null)
            {
                float openLength = ReachUIInternalTools.GetAnimatorClipLength(animator, CLIP_IN_KEY);
                if (openLength > _maxOpenStateLength)
                {
                    _maxOpenStateLength = openLength;
                }
            }
        }

        void OnEnable()
        {
            NotificationManager.OnNotification += HandleOnNotification;
            NotificationManager.OnNotificationQueueEmpty += HandleOnNotificationQueueEmpty;
        }
        void OnDisable()
        {
            NotificationManager.OnNotification -= HandleOnNotification;
            NotificationManager.OnNotificationQueueEmpty -= HandleOnNotificationQueueEmpty;
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Handles incoming notifications and updates the UI panel.
        /// </summary>
        /// <param name="notification">The notification to display.</param>
        async void HandleOnNotification(Notification notification)
        {
            if (_panel == null || _panel.Window == null) return;

            // Close the panel if it is already open.
            if (_panel.Window.isOn)
            {
                _panel.Window.CloseWindow();
                await Task.Delay(TimeSpan.FromSeconds(_closeStateLength + SAFETY_TOLERANCE));
            }

            // Update the panel with new notification details.
            _panel.Title.text = notification.Title;
            _panel.Message.text = notification.Message;
            _panel.Badge.color = ImportanceColor.GetColor(notification.Importance);
            // _panel.Badge.sprite = notification.Icon; // TODO: Implement icon support if needed.

            // Open the panel.
            _panel.Window.OpenWindow();
            await Task.Delay(TimeSpan.FromSeconds(_maxOpenStateLength + SAFETY_TOLERANCE));
        }

        /// <summary>
        /// Handles the event when the notification queue is empty.
        /// </summary>
        async void HandleOnNotificationQueueEmpty()
        {
            if (_panel.Window.isOn)
            {
                _panel.Window.CloseWindow();
                await Task.Delay(TimeSpan.FromSeconds(_closeStateLength + SAFETY_TOLERANCE));
            }
        }

        #endregion
    }
}
