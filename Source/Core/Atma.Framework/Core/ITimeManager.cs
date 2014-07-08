
namespace Atma.Core
{
    interface ITimeManager
    {
        void setTime(long ms);
        long delta { get; }
        long time { get; }
    }
}
