using Amethyst.Network.Structures;
using Amethyst.Server.Systems.Chat.Base;
using Amethyst.Systems.Chat.Base.Models;

namespace Amethyst.Systems.Chat;

public sealed class PrimitiveRenderer : IChatMessageRenderer
{
    public string Name => "PrimitiveRenderer";

    public void Render(MessageRenderContext ctx)
    {
        bool isRoot = ctx.Player.User?.Permissions.HasPermission("root") == Users.Base.Permissions.PermissionAccess.HasPermission;

        ctx.Prefix.TryAdd("primitive_pfx", isRoot
            ? "[c/ff5959:root]@"
            : "@");

        ctx.Name.TryAdd("primitive_name", ctx.Player.User?.Name ?? "Unknown");

        ctx.Text.TryAdd("primitive_text", ctx.Message.ModifiedText ?? ctx.Message.Text);
        ctx.Text.TryAdd("primitive_text", ctx.Message.ModifiedText ?? ctx.Message.Text);

        if (isRoot)
        {
            ctx.Color = "bf6d6d";
        }
    }
}
