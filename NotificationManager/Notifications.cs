namespace AEB.Systems.Notification
{
    /// <summary>
    /// Enum representing all possible notification IDs in the system.
    /// These IDs must match the corresponding IDs in the PlayFab TitleData or other persistent storage.
    /// </summary>
    public enum Notifications : int
    {
        Invitation_NoRoomInvitationSent = 1001,
        Invitation_JoinInvitorRoomFailed = 1002,
        Invitation_NoInviterRoom = 1003,
        Demo_FinishedDemo = 1004,
        Demo_FinishedDemoWithF2P = 1005,
        Rule_HigherLevel = 1006,
        Rule_HigherLevel_Connection = 1007

        //////Example
        //// InGame Notifications
        //InGame_LevelUp = 3001,
        //InGame_ItemCollected = 3002,

        //// Platform Notifications
        //Platform_UpdateAvailable = 4001,
        //Platform_AchievementUnlocked = 4002
    }
}