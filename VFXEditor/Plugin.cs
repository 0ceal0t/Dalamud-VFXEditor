using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiFileDialog;
using ImGuiNET;
using ImPlotNET;
using OtterGui;
using System;
using System.Collections.Generic;
using VfxEditor.AvfxFormat;
using VfxEditor.Data;
using VfxEditor.DirectX;
using VfxEditor.EidFormat;
using VfxEditor.FileManager;
using VfxEditor.Interop;
using VfxEditor.Library;
using VfxEditor.PapFormat;
using VfxEditor.PhybFormat;
using VfxEditor.ScdFormat;
using VfxEditor.SklbFormat;
using VfxEditor.Spawn;
using VfxEditor.TextureFormat;
using VfxEditor.TmbFormat;
using VfxEditor.Tracker;
using VfxEditor.Ui.Export;
using VfxEditor.Ui.Tools;
using VfxEditor.UldFormat;
using DalamudCommandManager = Dalamud.Game.Command.CommandManager;

namespace VfxEditor {
    public unsafe partial class Plugin : IDalamudPlugin {
        public static DalamudPluginInterface PluginInterface { get; private set; }
        public static ClientState ClientState { get; private set; }
        public static Framework Framework { get; private set; }
        public static Condition Condition { get; private set; }
        public static DalamudCommandManager CommandManager { get; private set; }
        public static ObjectTable Objects { get; private set; }
        public static SigScanner SigScanner { get; private set; }
        public static DataManager DataManager { get; private set; }
        public static TargetManager TargetManager { get; private set; }
        public static KeyState KeyState { get; private set; }
        public static ITextureProvider TextureProvider { get; private set; }

        public static bool InGpose => PluginInterface.UiBuilder.GposeActive;
        public static GameObject GposeTarget => Objects.CreateObjectReference( new IntPtr( TargetSystem.Instance()->GPoseTarget ) );
        public static GameObject PlayerObject => InGpose ? GposeTarget : ClientState?.LocalPlayer;
        public static GameObject TargetObject => InGpose ? GposeTarget : TargetManager?.Target;

        public static ResourceLoader ResourceLoader { get; private set; }
        public static DirectXManager DirectXManager { get; private set; }
        public static Configuration Configuration { get; private set; }
        public static TrackerManager Tracker { get; private set; }
        public static ToolsDialog ToolsDialog { get; private set; }
        public static TexToolsDialog TexToolsDialog { get; private set; }
        public static LibraryManager LibraryManager { get; private set; }

        public static PenumbraIpc PenumbraIpc { get; private set; }
        public static PenumbraDialog PenumbraDialog { get; private set; }

        public static List<IFileManager> Managers => new( new IFileManager[]{
            TextureManager,
            AvfxManager,
            TmbManager,
            PapManager,
            ScdManager,
            EidManager,
            UldManager,
            PhybManager,
            SklbManager,
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

        public string Name => "VFXEditor";
        public static string RootLocation { get; private set; }
        private const string CommandName = "/vfxedit";

        private static bool ClearKeyState = false;

        public Plugin(
                DalamudPluginInterface pluginInterface,
                ClientState clientState,
                DalamudCommandManager commandManager,
                Framework framework,
                Condition condition,
                ObjectTable objects,
                SigScanner sigScanner,
                DataManager dataManager,
                TargetManager targetManager,
                KeyState keyState,
                ITextureProvider textureProvider
         ) {
            PluginInterface = pluginInterface;
            ClientState = clientState;
            Condition = condition;
            CommandManager = commandManager;
            Objects = objects;
            SigScanner = sigScanner;
            DataManager = dataManager;
            Framework = framework;
            TargetManager = targetManager;
            KeyState = keyState;
            TextureProvider = textureProvider;

            CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) { HelpMessage = "toggle ui" } );

            RootLocation = PluginInterface.AssemblyLocation.DirectoryName;

            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlot.SetCurrentContext( ImPlot.CreateContext() );

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Setup();

            TextureManager.Setup();
            TextureManager = new();
            TmbManager = new();
            AvfxManager = new();
            PapManager = new();
            ScdManager = new();
            EidManager = new();
            UldManager = new();
            PhybManager = new();
            SklbManager = new();

            ToolsDialog = new ToolsDialog();
            PenumbraIpc = new PenumbraIpc();
            PenumbraDialog = new PenumbraDialog();
            TexToolsDialog = new TexToolsDialog();
            ResourceLoader = new ResourceLoader();
            DirectXManager = new DirectXManager();
            Tracker = new TrackerManager();
            LibraryManager = new LibraryManager();

            FileDialogManager.Initialize( PluginInterface );

            Framework.Update += FrameworkOnUpdate;
            PluginInterface.UiBuilder.Draw += Draw;
            PluginInterface.UiBuilder.Draw += FileDialogManager.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUi;
        }

        public static void CheckClearKeyState() {
            if( ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) && Configuration.BlockGameInputsWhenFocused ) ClearKeyState = true;
        }

        private void FrameworkOnUpdate( Framework framework ) {
            VfxSpawn.Tick();
            KeybindConfiguration.UpdateState();
            if( ClearKeyState ) KeyState.ClearAll();
            ClearKeyState = false;
        }

        private void DrawConfigUi() => AvfxManager.Show();

        private void OnCommand( string command, string rawArgs ) {
            if( Managers.FindFirst( x => rawArgs.ToLower().Equals( x.GetExportName().ToLower() ), out var manager ) ) manager.Show();
        }

        public void Dispose() {
            Framework.Update -= FrameworkOnUpdate;
            PluginInterface.UiBuilder.Draw -= FileDialogManager.Draw;
            PluginInterface.UiBuilder.Draw -= Draw;
            PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUi;

            ImPlot.DestroyContext();

            CommandManager.RemoveHandler( CommandName );
            PenumbraIpc?.Dispose();

            ResourceLoader?.Dispose();
            ResourceLoader = null;

            CopyManager.DisposeAll();
            VfxEditor.CommandManager.DisposeAll();

            TextureManager.BreakDown();
            Managers.ForEach( x => x?.Dispose() );
            TextureManager = null;
            AvfxManager = null;
            TmbManager = null;
            PapManager = null;
            ScdManager = null;
            EidManager = null;
            UldManager = null;
            PhybManager = null;
            SklbManager = null;

            DirectXManager?.Dispose();
            DirectXManager = null;

            Modals.Clear();

            VfxSpawn.Remove();
            TmbSpawn.Dispose();
            FileDialogManager.Dispose();
        }
    }
}