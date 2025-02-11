using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace QuickWaypoint;

public class QuickWaypointModSystem : ModSystem
{
    internal QuickWaypointHotKeyHandler _handler;

    public override void StartClientSide(ICoreClientAPI api)
    {
        _handler = new QuickWaypointHotKeyHandler(api);
        api.Logger.Log(EnumLogType.Event, "QuickWaypoint loaded.");
    }

    public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Client;
}
