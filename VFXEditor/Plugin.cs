using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using VfxEditor.Data;
using VfxEditor.Data.Command;
using VfxEditor.DirectX;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats;
using VfxEditor.Formats.AvfxFormat;
using VfxEditor.Formats.MtrlFormat;
using VfxEditor.Formats.SklbFormat;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Interop;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Library;
using VfxEditor.Spawn;
using VfxEditor.Tracker;
using VfxEditor.Ui.Export;
using VfxEditor.Ui.Import;
using VfxEditor.Ui.Tools;
using VfxEditor.Utils;

namespace VfxEditor {
    public unsafe partial class Plugin : IDalamudPlugin {
        public static ResourceLoader ResourceLoader { get; private set; }
        public static DirectXManager DirectXManager { get; private set; }
        public static Configuration Configuration { get; private set; }
        public static TrackerManager TrackerManager { get; private set; }
        public static ToolsDialog ToolsDialog { get; private set; }
        public static PenumbraRequiredWindow PenumbraRequiredWindow { get; private set; }
        public static TexToolsDialog TexToolsExportDialog { get; private set; }
        public static LibraryManager LibraryManager { get; private set; }

        public static PenumbraIpc PenumbraIpc { get; private set; }
        public static PenumbraDialog PenumbraExportDialog { get; private set; }
        public static PenumbraImportDialog PenumbraImportDialog { get; private set; }

        public static WindowSystem WindowSystem { get; private set; }

        // Every editor, in the order they are shown in the menu bar. Formats are defined in Formats/FileFormats.cs
        public static List<IFileManagerGroup> Groups { get; private set; } = [];

        // Editors which other code needs to access directly
        public static AvfxManagerGroup AvfxManager { get; private set; }
        public static TextureManager TextureManager { get; private set; }
        public static IFileManagerGroup ScdManager { get; private set; }
        public static SklbManagerGroup SklbManager { get; private set; }
        public static MtrlManagerGroup MtrlManager { get; private set; }
        public static IFileManagerGroup ShpkManager { get; private set; }

        public static string RootLocation { get; private set; }
        private const string CommandName = "/vfxedit";

        private static bool ClearKeyState = false;
        public static bool IsImguiSafe { get; set; } = false;

        private static ConfigurationWindow ConfigWindow;

        public Plugin( IDalamudPluginInterface pluginInterface ) {
            pluginInterface.Create<Dalamud>();

            Dalamud.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) { HelpMessage = "toggle ui" } );

            RootLocation = Dalamud.PluginInterface.AssemblyLocation.DirectoryName;
            OtterTex.NativeDll.Initialize( pluginInterface.AssemblyLocation.DirectoryName );

            WindowSystem = new();

            // Loads this plugin's settings, or on first run imports them from VFXEditor / VFXEditor-Beta. Never fails
            Configuration = ConfigurationMigration.Load();
            ConfigWindow = new( Configuration );

            TextureManager.LoadLibrary();
            Groups = [
                AvfxManager = new(),
                TextureManager = new(),
                FileFormats.Tmb.CreateGroup(),
                FileFormats.Pap.CreateGroup(),
                ScdManager = FileFormats.Scd.CreateGroup(),
                FileFormats.Uld.CreateGroup(),
                SklbManager = new(),
                FileFormats.Skp.CreateGroup(),
                FileFormats.Phyb.CreateGroup(),
                FileFormats.Eid.CreateGroup(),
                FileFormats.Atch.CreateGroup(),
                FileFormats.Kdb.CreateGroup(),
                FileFormats.Pbd.CreateGroup(),
                FileFormats.Mdl.CreateGroup(),
                MtrlManager = new(),
                ShpkManager = FileFormats.Shpk.CreateGroup(),
                FileFormats.Shcd.CreateGroup(),
            ];
            AddDefaultDocuments();

            ToolsDialog = new();
            PenumbraIpc = new();
            PenumbraRequiredWindow = new( WindowSystem );
            PenumbraExportDialog = new();
            TexToolsExportDialog = new();
            ResourceLoader = new();
            DirectXManager = new();
            TrackerManager = new();
            LibraryManager = new();
            PenumbraImportDialog = new();

            Dalamud.Framework.Update += FrameworkOnUpdate;
            Dalamud.PluginInterface.UiBuilder.Draw += Draw;
            Dalamud.PluginInterface.UiBuilder.Draw += FileBrowserManager.Draw;
            Dalamud.PluginInterface.UiBuilder.OpenConfigUi += OpenConfigUi;
            Dalamud.PluginInterface.UiBuilder.OpenMainUi += OpenConfigUi;
        }

        public static void CheckClearKeyState() {
            if( ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) && Configuration.BlockGameInputsWhenFocused ) ClearKeyState = true;
        }

        private void FrameworkOnUpdate( IFramework framework ) {
            VfxSpawn.Tick();
            PenumbraPreview.Tick();
            KeybindConfiguration.UpdateState();
            if( ClearKeyState ) Dalamud.KeyState.ClearAll();
            ClearKeyState = false;
        }

        private void OpenConfigUi() => AvfxManager.Show();

        private void OnCommand( string command, string rawArgs ) {
            if( string.IsNullOrEmpty( rawArgs ) ) {
                AvfxManager?.Show();
                return;
            }
            if( Groups.FindFirst( x => rawArgs.ToLower().Equals( x.GetId().ToLower() ), out var manager ) ) manager.Show();
        }

        public void Dispose() {
            Dalamud.Framework.Update -= FrameworkOnUpdate;
            Dalamud.PluginInterface.UiBuilder.Draw -= FileBrowserManager.Draw;
            Dalamud.PluginInterface.UiBuilder.Draw -= Draw;
            Dalamud.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigUi;
            Dalamud.PluginInterface.UiBuilder.OpenMainUi -= OpenConfigUi;

            Dalamud.CommandManager.RemoveHandler( CommandName );
            PenumbraPreview.Dispose();
            PenumbraIpc?.Dispose();

            ResourceLoader?.Dispose();
            ResourceLoader = null;

            TextureManager.FreeLibrary();
            Groups.ForEach( x => x?.Reset( true ) );
            DirectXManager?.Dispose();

            WindowSystem.RemoveAllWindows();
            Modals.Clear();

            VfxSpawn.Dispose();
            TmbSpawn.Dispose();
            FileBrowserManager.Dispose();
            ExportDialog.Reset();
        }
    }
}