namespace Vrlife.Core.Vr
{
    public interface IGrabService
    {
        void Grab(Grabber possessor);

        void Release(Grabber possessor);
    }
}