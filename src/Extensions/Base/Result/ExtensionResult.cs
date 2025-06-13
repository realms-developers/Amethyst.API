namespace Amethyst.Extensions.Base.Result;

public enum ExtensionResult
{
    SuccessOperation,
    NotAllowed,

    // error in plugin side
    InternalError,

    // error in amethyst side
    ExternalError
}
