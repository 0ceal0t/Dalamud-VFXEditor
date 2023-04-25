using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiFileDialog;
using ImGuiNET;
using ImPlotNET;
using System.Collections.Generic;
using VfxEditor.Animation;
using VfxEditor.AvfxFormat;
using VfxEditor.Data;
using VfxEditor.DirectX;
using VfxEditor.EidFormat;
using VfxEditor.FileManager;
using VfxEditor.Interop;
using VfxEditor.PapFormat;
using VfxEditor.Penumbra;
using VfxEditor.ScdFormat;
using VfxEditor.TexTools;
using VfxEditor.TextureFormat;
using VfxEditor.TmbFormat;
using VfxEditor.Tracker;
using VfxEditor.Ui;
using VfxEditor.UldFormat;
using DalamudCommandManager = Dalamud.Game.Command.CommandManager;

namespace VfxEditor {
    public partial class Plugin : IDalamudPlugin {
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

        public static ResourceLoader ResourceLoader { get; private set; }
        public static AnimationManager AnimationManager { get; private set; }
        public static ActorAnimationManager ActorAnimationManager { get; private set; }
        public static DirectXManager DirectXManager { get; private set; }
        public static Configuration Configuration { get; private set; }
        public static VfxTracker VfxTracker { get; private set; }
        public static ToolsDialog ToolsDialog { get; private set; }
        public static TexToolsDialog TexToolsDialog { get; private set; }
        public static PenumbraDialog PenumbraDialog { get; private set; }

        public static List<IFileManager> Managers => new( new IFileManager[]{
            TextureManager,
            AvfxManager,
            TmbManager,
            PapManager,
            ScdManager,
            EidManager,
            UldManager
        } );

        public static AvfxManager AvfxManager { get; private set; }
        public static TextureManager TextureManager { get; private set; }
        public static TmbManager TmbManager { get; private set; }
        public static PapManager PapManager { get; private set; }
        public static ScdManager ScdManager { get; private set; }
        public static EidManager EidManager { get; private set; }
        public static UldManager UldManager { get; private set; }

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
                KeyState keyState
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

            CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) { HelpMessage = "toggle ui" } );

            RootLocation = PluginInterface.AssemblyLocation.DirectoryName;

            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlot.SetCurrentContext( ImPlot.CreateContext() );

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Setup();

            TextureManager.Setup();
            TextureManager = new TextureManager();
            TmbManager = new TmbManager();
            AvfxManager = new AvfxManager();
            PapManager = new PapManager();
            ScdManager = new ScdManager();
            EidManager = new EidManager();
            UldManager = new UldManager();

            ToolsDialog = new ToolsDialog();
            PenumbraDialog = new PenumbraDialog();
            TexToolsDialog = new TexToolsDialog();
            ResourceLoader = new ResourceLoader();
            AnimationManager = new AnimationManager();
            ActorAnimationManager = new ActorAnimationManager();
            DirectXManager = new DirectXManager();
            VfxTracker = new VfxTracker();

            FileDialogManager.Initialize( PluginInterface );

            ResourceLoader.Init();
            ResourceLoader.Enable();

            Framework.Update += FrameworkOnUpdate;
            PluginInterface.UiBuilder.Draw += Draw;
            PluginInterface.UiBuilder.Draw += FileDialogManager.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUi;
        }

        public static void CheckClearKeyState() {
            if( ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) && Configuration.BlockGameInputsWhenFocused ) ClearKeyState = true;
        }

        private void FrameworkOnUpdate( Framework framework ) {
            KeybindConfiguration.UpdateState();
            if( ClearKeyState ) KeyState.ClearAll();
            ClearKeyState = false;
        }

        private void DrawConfigUi() => AvfxManager.Show();

        private void OnCommand( string command, string rawArgs ) {
            switch( rawArgs.ToLowerInvariant() ) {
                case "tmb":
                    TmbManager.Toggle();
                    break;
                case "pap":
                    PapManager.Toggle();
                    break;
                case "scd":
                    ScdManager.Toggle();
                    break;
                case "eid":
                    EidManager.Toggle();
                    break;
                case "uld":
                    UldManager.Toggle();
                    break;
                default:
                    AvfxManager.Toggle();
                    break;
            }
        }

        public void Dispose() {
            Framework.Update -= FrameworkOnUpdate;
            PluginInterface.UiBuilder.Draw -= FileDialogManager.Draw;
            PluginInterface.UiBuilder.Draw -= Draw;
            PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUi;

            ImPlot.DestroyContext();

            CommandManager.RemoveHandler( CommandName );

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

            AnimationManager?.Dispose();
            AnimationManager = null;

            ActorAnimationManager?.Dispose();
            ActorAnimationManager = null;

            DirectXManager?.Dispose();
            DirectXManager = null;

            RemoveSpawn();

            FileDialogManager.Dispose();
        }
    }
}