using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace VfxEditor {
    public class Dalamud {
        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static Framework Framework { get; private set; } = null!;
        [PluginService] public static Condition Condition { get; private set; } = null!;
        [PluginService] public static global::Dalamud.Game.Command.CommandManager CommandManager { get; private set; } = null!;
        [PluginService] public static IObjectTable Objects { get; private set; } = null!;
        [PluginService] public static SigScanner SigScanner { get; private set; } = null!;
        [PluginService] public static IDataManager DataManager { get; private set; } = null!;
        [PluginService] public static ITargetManager TargetManager { get; private set; } = null!;
        [PluginService] public static KeyState KeyState { get; private set; } = null!;
        [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;
    }
}
