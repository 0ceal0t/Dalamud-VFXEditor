using Dalamud.Interface.ImGuiNotification;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;

namespace VfxEditor {
    public class Dalamud {
        [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static IFramework Framework { get; private set; } = null!;
        [PluginService] public static ICondition Condition { get; private set; } = null!;
        [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] public static IObjectTable Objects { get; private set; } = null!;
        [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService] public static IDataManager DataManager { get; private set; } = null!;
        [PluginService] public static ITargetManager TargetManager { get; private set; } = null!;
        [PluginService] public static IKeyState KeyState { get; private set; } = null!;
        [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;
        [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;
        [PluginService] public static IGameInteropProvider Hooks { get; private set; } = null!;
        [PluginService] public static INotificationManager Notification { get; private set; } = null!;

        public static void Error( Exception e, string message ) => PluginLog.Error( e, message );

        public static void Error( string message ) => PluginLog.Error( message );

        public static void Log( string messages ) => PluginLog.Info( messages );

#nullable enable
        public static void OkNotification( string content, string? title = "VFXEditor" ) => Notification.AddNotification( new() {
            Content = content,
            Title = title,
            Type = NotificationType.Success,
        } );

        public static void InfoNotification( string content, string? title = "VFXEditor" ) => Notification.AddNotification( new() {
            Content = content,
            Title = title,
            Type = NotificationType.Info,
        } );

        public static void ErrorNotification( string content, string? title = "VFXEditor" ) => Notification.AddNotification( new() {
            Content = content,
            Title = title,
            Type = NotificationType.Error,
        } );

        public static void WarningNotification( string content, string? title = "VFXEditor" ) => Notification.AddNotification( new() {
            Content = content,
            Title = title,
            Type = NotificationType.Warning,
        } );
#nullable disable

        public static bool GameFileExists( string path ) {
            try {
                return DataManager.FileExists( path );
            }
            catch( Exception ) { return false; }
        }
    }
}
