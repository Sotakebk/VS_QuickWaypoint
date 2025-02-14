using System;
using System.Globalization;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace QuickWaypoint;

internal class QuickWaypointHotKeyHandler
{
    private const string HotKeyCode = "quickwaypoint_create";
    private readonly ICoreClientAPI _api;

    public QuickWaypointHotKeyHandler(ICoreClientAPI api)
    {
        _api = api;

        _api.Input.RegisterHotKey(HotKeyCode, "Quickly Create a Waypoint", GlKeys.I, HotkeyType.GUIOrOtherControls, altPressed: false, ctrlPressed: false, shiftPressed: false);
        _api.Input.SetHotKeyHandler(HotKeyCode, OnKeyPress);
    }

    internal bool OnKeyPress(KeyCombination keyCombination)
    {
        try
        {
            return InnerOnKeyPress();
        }
        catch (Exception e)
        {
            _api.Logger.Error(e);
            return false;
        }
    }

    internal bool InnerOnKeyPress()
    {
        if (IsMapDisallowed())
        {
            _api.Logger.Log(EnumLogType.Debug, "Attempted to make a waypoint, but map is disabled on this server.");
            _api.ShowChatMessage("QuickWaypoint: map is not allowed.");
            return false;
        }

        var currentEntity = _api.World.Player.CurrentEntitySelection;
        if (currentEntity != null)
        {
            var command = CreateWaypointCommandForEntity(currentEntity);
            _api.SendChatMessage(command, GlobalConstants.AllChatGroups);
            return true;
        }

        var currentBlock = _api.World.Player.CurrentBlockSelection;
        if (currentBlock != null)
        {
            var command = CreateWaypointCommandForBlock(currentBlock);
            _api.SendChatMessage(command, GlobalConstants.AllChatGroups);
            return true;
        }

        _api.Logger.Log(EnumLogType.Debug, "Attempted to make a waypoint, but nothing was selected.");
        _api.ShowChatMessage("QuickWaypoint: nothing selected. Try looking at an entity or a block.");
        return false;
    }

    internal static string CreateWaypointCommandForEntity(EntitySelection selection)
    {
        var x = selection.Position.X.ToString(CultureInfo.InvariantCulture);
        var y = selection.Position.Y.ToString(CultureInfo.InvariantCulture);
        var z = selection.Position.Z.ToString(CultureInfo.InvariantCulture);
        var pinned = false;
        var (name, icon, color) = EntityToWaypoint.GetNameIconAndColor(selection);

        return $"/waypoint addati {icon} ={x} ={y} ={z} {pinned} #{color.ToArgbHexString()} \"{name}\"";
    }

    internal string CreateWaypointCommandForBlock(BlockSelection selection)
    {
        var name = selection.Block.GetPlacedBlockName(_api.World, selection.Position);
        var x = selection.Position.X.ToString(CultureInfo.InvariantCulture);
        var y = selection.Position.Y.ToString(CultureInfo.InvariantCulture);
        var z = selection.Position.Z.ToString(CultureInfo.InvariantCulture);
        var pinned = false;
        var (icon, color) = BlockToWaypoint.GetIconAndColor(_api, selection);

        return $"/waypoint addati {icon} ={x} ={y} ={z} {pinned} #{color.ToArgbHexString()} \"{name}\"";
    }

    private bool IsMapDisallowed()
    {
        if (!_api.World.Config.GetBool("allowMap", true))
        {
            return true;
        }

        return false;
    }
}
