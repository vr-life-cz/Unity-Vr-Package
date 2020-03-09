namespace Vrlife.Core.Vr
{
    public interface IDebugInfoProvider
    {
        string GetDebugInfo();
        string Label { get; }
    }
}