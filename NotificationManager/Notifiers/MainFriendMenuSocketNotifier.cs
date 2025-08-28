using AEB.Menu;
using AEB.PlayFab.Base;
using UnityEngine;

namespace AEB.Systems.Notification
{
    /// <summary>
    /// Listens to events from MainFriendMenuSocketNotifier and sends notifications via NotificationManager.
    /// </summary>
    public class MainFriendMenuSocketNotifier : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Reference to the MainInvitationSocket instance.
        /// </summary>
        [SerializeField] MainFriendMenuSocket _invitationSocket;

        #endregion

        #region Unity

        void OnEnable()
        {
            if (_invitationSocket != null)
            {
                _invitationSocket.OnNoRoomInvitationSent += HandleNoRoomInvitationSent;
                _invitationSocket.OnJoinInvitorRoomFailed += HandleJoinInvitorRoomFailed;
                _invitationSocket.OnNoInviterRoom += HandleNoInviterRoom;
            }
            else
            {
                Debug.LogError("[MainInvitationSocketNotifier] MainInvitationSocket reference is missing.");
            }
        }

        void OnDisable()
        {
            if (_invitationSocket != null)
            {
                _invitationSocket.OnNoRoomInvitationSent -= HandleNoRoomInvitationSent;
                _invitationSocket.OnJoinInvitorRoomFailed -= HandleJoinInvitorRoomFailed;
                _invitationSocket.OnNoInviterRoom -= HandleNoInviterRoom;
            }
        }

        #endregion

        #region Event

        /// <summary>
        /// Handles the event triggered when an invitation is sent without a room.
        /// </summary>
        /// <param name="invitation">The invitation data.</param>
        void HandleNoRoomInvitationSent(Account invitation)
        {
            NotificationManager.AddNotification(Notifications.Invitation_NoRoomInvitationSent);
        }

        /// <summary>
        /// Handles the event triggered when joining an inviter's room fails.
        /// </summary>
        /// <param name="invitation">The invitation data.</param>
        void HandleJoinInvitorRoomFailed(Account invitation)
        {
            NotificationManager.AddNotification(Notifications.Invitation_JoinInvitorRoomFailed);
        }

        /// <summary>
        /// Handles the event triggered when there is no room available for the inviter.
        /// </summary>
        /// <param name="invitation">The invitation data.</param>
        void HandleNoInviterRoom(Account invitation)
        {
            NotificationManager.AddNotification(Notifications.Invitation_NoInviterRoom);
        }

        #endregion
    }
}
