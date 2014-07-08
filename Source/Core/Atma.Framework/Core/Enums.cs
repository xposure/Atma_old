
namespace Atma.Core
{
    /// <summary>
    ///     The level of detail in which the log will go into.
    /// </summary>
    public enum LoggingLevel
    {
        Low = 1,
        Normal,
        Verbose
    }

    /// <summary>
    ///     The importance of a logged message.
    /// </summary>
    public enum LogMessageLevel
    {
        Trivial = 1,
        Normal,
        Critical
    }

    //public enum BlendFunction
    //{
    //    Add = 0,
    //    Subtract = 1,
    //    ReverseSubtract = 2,
    //    Max = 3,
    //    Min = 4,
    //}

    //public enum Blend
    //{
    //    One = 0,
    //    Zero = 1,
    //    SourceColor = 2,
    //    InverseSourceColor = 3,
    //    SourceAlpha = 4,
    //    InverseSourceAlpha = 5,
    //    DestinationColor = 6,
    //    InverseDestinationColor = 7,
    //    DestinationAlpha = 8,
    //    InverseDestinationAlpha = 9,
    //    BlendFactor = 10,
    //    InverseBlendFactor = 11,
    //    SourceAlphaSaturation = 12,
    //}

    //[Flags]
    //public enum ColorWriteChannels
    //{
    //    None = 0,
    //    Red = 1,
    //    Green = 2,
    //    Blue = 4,
    //    Alpha = 8,
    //    All = 15,
    //}

    //public enum TextureAddressMode
    //{
    //    Wrap = 0,
    //    Clamp = 1,
    //    Mirror = 2,
    //}

    //public enum TextureFilter
    //{
    //    Linear = 0,
    //    Point = 1,
    //    Anisotropic = 2,
    //    LinearMipPoint = 3,
    //    PointMipLinear = 4,
    //    MinLinearMagPointMipLinear = 5,
    //    MinLinearMagPointMipPoint = 6,
    //    MinPointMagLinearMipLinear = 7,
    //    MinPointMagLinearMipPoint = 8,
    //}

    //public enum CullMode
    //{
    //    None = 0,
    //    CullClockwiseFace = 1,
    //    CullCounterClockwiseFace = 2,
    //}

    //public enum FillMode
    //{
    //    Solid = 0,
    //    WireFrame = 1,
    //}

    //public enum StencilOperation
    //{
    //    Keep = 0,
    //    Zero = 1,
    //    Replace = 2,
    //    Increment = 3,
    //    Decrement = 4,
    //    IncrementSaturation = 5,
    //    DecrementSaturation = 6,
    //    Invert = 7,
    //}

    //public enum CompareFunction
    //{
    //    Always = 0,
    //    Never = 1,
    //    Less = 2,
    //    LessEqual = 3,
    //    Equal = 4,
    //    GreaterEqual = 5,
    //    Greater = 6,
    //    NotEqual = 7,
    //}

}
