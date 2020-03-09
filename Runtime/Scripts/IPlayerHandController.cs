using Vrlife.Core.Mvc;

namespace Vrlife.Core.Vr
{
    public interface IPlayerHandController : IController<IPlayerHandView>
    {
        void Update();
    }
}