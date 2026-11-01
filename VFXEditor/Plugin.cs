using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using VfxEditor.Data;
using VfxEditor.DirectX;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.AtchFormat;
using VfxEditor.Formats.AvfxFormat;
using VfxEditor.Formats.EidFormat;
using VfxEditor.Formats.KdbFormat;
using VfxEditor.Formats.MdlFormat;
using VfxEditor.Formats.MtrlFormat;
using VfxEditor.Formats.PapFormat;
using VfxEditor.Formats.PbdFormat;
using VfxEditor.Formats.PhybFormat;
using VfxEditor.Formats.ScdFormat;
using VfxEditor.Formats.ShcdFormat;
using VfxEditor.Formats.ShpkFormat;
using VfxEditor.Formats.SklbFormat;
using VfxEditor.Formats.SkpFormat;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Formats.TmbFormat;
using VfxEditor.Formats.UldFormat;
using VfxEditor.Interop;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Library;
using VfxEditor.Spawn;
using VfxEditor.Tracker;
using VfxEditor.Ui.Export;
using VfxEditor.Ui.Import;
using VfxEditor.Ui.Tools;

namespace VfxEditor {
    public unsafe partial class Plugin : IDalamudPlugin {
        public static ResourceLoader ResourceLoader { get; private set; }
        public static DirectXManager DirectXManager { get; private set; }
        public static Configuration Configuration { get; private set; }
        public static TrackerManager TrackerManager { get; private set; }
        public static ToolsDialog ToolsDialog { get; private set; }
        public static TexToolsDialog TexToolsExportDialog { get; private set; }
        public static LibraryManager LibraryManager { get; private set; }

        public static PenumbraIpc PenumbraIpc { get; private set; }
        public static PenumbraRequiredWindow PenumbraRequiredWindow { get; private set; }
        public static PenumbraDialog PenumbraExportDialog { get; private set; }
        public static PenumbraImportDialog PenumbraImportDialog { get; private set; }

        public static WindowSystem WindowSystem { get; private set; }

        public static List<IFileManagerGroup> Groups => [
            TextureManager,
            AvfxManager,
            TmbManager,
            SklbManager,
            ScdManager,
            EidManager,
            UldManager,
            PhybManager,
            PapManager,
            AtchManager,
            SkpManager,
            ShpkManager,
            ShcdManager,
            MtrlManager,
            MdlManager,
            KdbManager,
            PbdManager,
        ];

        public static AvfxManagerGroup AvfxManager { get; private set; }
        public static TextureManager TextureManager { get; private set; }
        public static TmbManagerGroup TmbManager { get; private set; }
        public static PapManagerGroup PapManager { get; private set; }
        public static ScdManagerGroup ScdManager { get; private set; }
        public static EidManagerGroup EidManager { get; private set; }
        public static UldManagerGroup UldManager { get; private set; }
        public static PhybManagerGroup PhybManager { get; private set; }
        public static SklbManagerGroup SklbManager { get; private set; }
        public static AtchManagerGroup AtchManager { get; private set; }
        public static SkpManagerGroup SkpManager { get; private set; }
        public static ShpkManagerGroup ShpkManager { get; private set; }
        public static ShcdManagerGroup ShcdManager { get; private set; }
        public static MtrlManagerGroup MtrlManager { get; private set; }
        public static MdlManagerGroup MdlManager { get; private set; }
        public static KdbManagerGroup KdbManager { get; private set; }
        public static PbdManagerGroup PbdManager { get; private set; }

        public static string RootLocation { get; private set; }
#if BETA
        private const string CommandName = "/vfxbeta";
#else
        private const string CommandName = "/vfxedit";
#endif

        private static bool ClearKeyState = false;
        public static bool IsImguiSafe { get; set; } = false;

        private static ConfigurationWindow ConfigWindow;

        public Plugin( IDalamudPluginInterface pluginInterface ) {
            pluginInterface.Create<Dalamud>();

            Dalamud.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) { HelpMessage = "toggle ui" } );

            RootLocation = Dalamud.PluginInterface.AssemblyLocation.DirectoryName;
            OtterTex.NativeDll.Initialize( pluginInterface.AssemblyLocation.DirectoryName );

            WindowSystem = new();

            Configuration = Dalamud.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Setup();
            ConfigWindow = new( Configuration );

            TextureManager.LoadLibrary();
            TextureManager = new();
            TmbManager = new();
            AvfxManager = new();
            PapManager = new();
            ScdManager = new();
            EidManager = new();
            UldManager = new();
            PhybManager = new();
            SklbManager = new();
            AtchManager = new();
            SkpManager = new();
            ShpkManager = new();
            ShcdManager = new();
            MtrlManager = new();
            MdlManager = new();
            KdbManager = new();
            PbdManager = new();
            AddDefaultDocuments();

            ToolsDialog = new();
            PenumbraIpc = new();
            PenumbraExportDialog = new();
            TexToolsExportDialog = new();
            ResourceLoader = new();
            DirectXManager = new();
            TrackerManager = new();
            LibraryManager = new();
            PenumbraImportDialog = new();
            PenumbraRequiredWindow = new();

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
            PenumbraTempMod.Tick();
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
            PenumbraTempMod.Dispose();
        }
    }
}