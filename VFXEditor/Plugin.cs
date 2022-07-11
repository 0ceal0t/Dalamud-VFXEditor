using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiFileDialog;
using ImGuiNET;
using ImPlotNET;
using System.IO;
using System.Reflection;
using VFXEditor.AVFX;
using VFXEditor.Data;
using VFXEditor.Dialogs;
using VFXEditor.DirectX;
using VFXEditor.Interop;
using VFXEditor.PAP;
using VFXEditor.Penumbra;
using VFXEditor.TexTools;
using VFXEditor.Texture;
using VFXEditor.TMB;
using VFXEditor.Tracker;
using VFXSelect;

namespace VFXEditor {
    public partial class Plugin : IDalamudPlugin {
        public static DalamudPluginInterface PluginInterface { get; private set; }
        public static ClientState ClientState { get; private set; }
        public static Framework Framework { get; private set; }
        public static Condition Condition { get; private set; }
        public static CommandManager CommandManager { get; private set; }
        public static ObjectTable Objects { get; private set; }
        public static SigScanner SigScanner { get; private set; }
        public static DataManager DataManager { get; private set; }
        public static TargetManager TargetManager { get; private set; }

        public static ResourceLoader ResourceLoader { get; private set; }
        public static DirectXManager DirectXManager { get; private set; }
        public static AVFXManager AvfxManager { get; private set; }
        public static TextureManager TextureManager { get; private set; }
        public static TMBManager TmbManager { get; private set; }
        public static PAPManager PapManager { get; private set; }
        public static Configuration Configuration { get; private set; }
        public static VfxTracker VfxTracker { get; private set; }
        public static ToolsDialog ToolsDialog { get; private set; }
        public static TexToolsDialog TexToolsDialog { get; private set; }
        public static PenumbraDialog PenumbraDialog { get; private set; }

        public string Name => "VFXEditor";
        public static string RootLocation { get; private set; }
        private const string CommandName = "/vfxedit";

        public Plugin(
                DalamudPluginInterface pluginInterface,
                ClientState clientState,
                CommandManager commandManager,
                Framework framework,
                Condition condition,
                ObjectTable objects,
                SigScanner sigScanner,
                DataManager dataManager,
                TargetManager targetManager
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

            CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) { HelpMessage = "toggle ui" } );

            RootLocation = PluginInterface.AssemblyLocation.DirectoryName;

            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlot.SetCurrentContext( ImPlot.CreateContext() );

            SheetManager.Initialize(
                Path.Combine( RootLocation, "Files", "npc.csv" ),
                Path.Combine( RootLocation, "Files", "monster_vfx.json" ),
                Path.Combine( RootLocation, "Files", "vfx_misc.txt" ),
                DataManager,
                PluginInterface
            );

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Setup();

            TextureManager.Setup();
            TextureManager = new TextureManager();

            TMBManager.Setup();
            TmbManager = new TMBManager();

            AVFXManager.Setup();
            AvfxManager = new AVFXManager();

            PAPManager.Setup();
            PapManager = new PAPManager();

            ToolsDialog = new ToolsDialog();
            PenumbraDialog = new PenumbraDialog();
            TexToolsDialog = new TexToolsDialog();
            ResourceLoader = new ResourceLoader();
            DirectXManager = new DirectXManager();
            VfxTracker = new VfxTracker();

            FileDialogManager.Initialize( PluginInterface );
            CopyManager.Initialize();

            ResourceLoader.Init();
            ResourceLoader.Enable();

            PluginInterface.UiBuilder.Draw += Draw;
            PluginInterface.UiBuilder.Draw += FileDialogManager.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose() {
            PluginInterface.UiBuilder.Draw -= FileDialogManager.Draw;
            PluginInterface.UiBuilder.Draw -= Draw;
            PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            ImPlot.DestroyContext();

            CommandManager.RemoveHandler( CommandName );

            ResourceLoader.Dispose();
            ResourceLoader = null;

            TmbManager.Dispose();
            TmbManager = null;

            PapManager.Dispose();
            PapManager = null;

            AvfxManager.Dispose();
            AvfxManager = null;

            TextureManager.BreakDown();
            TextureManager.Dispose();
            TextureManager = null;

            DirectXManager.Dispose();
            DirectXManager = null;

            RemoveSpawnVfx();

            FileDialogManager.Dispose();
            CopyManager.Dispose();
        }

        private void DrawConfigUI() {
            AvfxManager.Show();
        }

        private void OnCommand( string command, string rawArgs ) {
            AvfxManager.Toggle();
        }
    }
}