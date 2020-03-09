namespace Vrlife.Core.Vr
{
    public interface IPlayerInputUpdater
    {
        PlayerHandInputDevice LeftHandInputDevice { get; }
        PlayerHandInputDevice RightHandInputDevice { get; }
        void SendHapticFeedback(HumanBodyPart handType);
    }
    
    
}