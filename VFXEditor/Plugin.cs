using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiFileDialog;
using ImGuiNET;
using ImPlotNET;
using OtterGui;
using System.Collections.Generic;
using VfxEditor.AvfxFormat;
using VfxEditor.Data;
using VfxEditor.DirectX;
using VfxEditor.EidFormat;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.AtchFormat;
using VfxEditor.Formats.ShpkFormat;
using VfxEditor.Formats.SkpFormat;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Interop;
using VfxEditor.Library;
using VfxEditor.PapFormat;
using VfxEditor.PhybFormat;
using VfxEditor.ScdFormat;
using VfxEditor.SklbFormat;
using VfxEditor.Spawn;
using VfxEditor.TmbFormat;
using VfxEditor.Tracker;
using VfxEditor.Ui.Export;
using VfxEditor.Ui.Tools;
using VfxEditor.UldFormat;

namespace VfxEditor {
    public unsafe partial class Plugin : IDalamudPlugin {
        public static ResourceLoader ResourceLoader { get; private set; }
        public static DirectXManager DirectXManager { get; private set; }
        public static Configuration Configuration { get; private set; }
        public static TrackerManager TrackerManager { get; private set; }
        public static ToolsDialog ToolsDialog { get; private set; }
        public static TexToolsDialog TexToolsDialog { get; private set; }
        public static LibraryManager LibraryManager { get; private set; }

        public static PenumbraIpc PenumbraIpc { get; private set; }
        public static PenumbraDialog PenumbraDialog { get; private set; }

        public static List<IFileManager> Managers => new( new IFileManager[]{
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
        } );

        public static AvfxManager AvfxManager { get; private set; }
        public static TextureManager TextureManager { get; private set; }
        public static TmbManager TmbManager { get; private set; }
        public static PapManager PapManager { get; private set; }
        public static ScdManager ScdManager { get; private set; }
        public static EidManager EidManager { get; private set; }
        public static UldManager UldManager { get; private set; }
        public static PhybManager PhybManager { get; private set; }
        public static SklbManager SklbManager { get; private set; }
        public static AtchManager AtchManager { get; private set; }
        public static SkpManager SkpManager { get; private set; }
        public static ShpkManager ShpkManager { get; private set; }

        public string Name => "VFXEditor";
        public static string RootLocation { get; private set; }
        private const string CommandName = "/vfxedit";

        private static bool ClearKeyState = false;

        public Plugin( DalamudPluginInterface pluginInterface ) {
            pluginInterface.Create<Dalamud>();

            Dalamud.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) { HelpMessage = "toggle ui" } );

            RootLocation = Dalamud.PluginInterface.AssemblyLocation.DirectoryName;

            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlot.SetCurrentContext( ImPlot.CreateContext() );

            Configuration = Dalamud.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Setup();

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

            ToolsDialog = new();
            PenumbraIpc = new();
            PenumbraDialog = new();
            TexToolsDialog = new();
            ResourceLoader = new();
            DirectXManager = new();
            TrackerManager = new();
            LibraryManager = new();

            FileDialogManager.Initialize( Dalamud.PluginInterface );

            Dalamud.Framework.Update += FrameworkOnUpdate;
            Dalamud.PluginInterface.UiBuilder.Draw += Draw;
            Dalamud.PluginInterface.UiBuilder.Draw += FileDialogManager.Draw;
            Dalamud.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUi;
        }

        public static void CheckClearKeyState() {
            if( ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) && Configuration.BlockGameInputsWhenFocused ) ClearKeyState = true;
        }

        private void FrameworkOnUpdate( Framework framework ) {
            VfxSpawn.Tick();
            KeybindConfiguration.UpdateState();
            if( ClearKeyState ) Dalamud.KeyState.ClearAll();
            ClearKeyState = false;
        }

        private void DrawConfigUi() => AvfxManager.Show();

        private void OnCommand( string command, string rawArgs ) {
            if( string.IsNullOrEmpty( rawArgs ) ) {
                AvfxManager?.Show();
                return;
            }
            if( Managers.FindFirst( x => rawArgs.ToLower().Equals( x.GetId().ToLower() ), out var manager ) ) manager.Show();
        }

        public void Dispose() {
            Dalamud.Framework.Update -= FrameworkOnUpdate;
            Dalamud.PluginInterface.UiBuilder.Draw -= FileDialogManager.Draw;
            Dalamud.PluginInterface.UiBuilder.Draw -= Draw;
            Dalamud.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUi;

            ImPlot.DestroyContext();

            Dalamud.CommandManager.RemoveHandler( CommandName );
            PenumbraIpc?.Dispose();

            ResourceLoader?.Dispose();
            ResourceLoader = null;

            CopyManager.DisposeAll();
            CommandManager.DisposeAll();

            Managers.ForEach( x => x?.Dispose() );
            DirectXManager?.Dispose();

            Modals.Clear();

            VfxSpawn.Dispose();
            TmbSpawn.Dispose();
            FileDialogManager.Dispose();
        }
    }
}