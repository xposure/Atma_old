
namespace Atma.Json
{
    public abstract class JsonUnknown : JsonValue
    {
        public override JsonTypes Type { get { return JsonTypes.Unknown; } }

    }
}
