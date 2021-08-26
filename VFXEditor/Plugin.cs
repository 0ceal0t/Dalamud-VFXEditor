using System;
using System.IO;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using ImPlotNET;

using VFXEditor.Data;
using VFXEditor.DirectX;
using VFXEditor.Data.Vfx;
using VFXEditor.Data.Texture;
using VFXEditor.Structs.Vfx;
using VFXSelect;

using System.Reflection;
using ImGuiFileDialog;

using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Data;

namespace VFXEditor
{
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

        public static BaseVfx SpawnVfx { get; private set; }
        public static ResourceLoader ResourceLoader { get; private set; }

        public static string TemplateLocation { get; private set; }

        public string Name => "VFXEditor";
        public string AssemblyLocation { get; set; } = Assembly.GetExecutingAssembly().Location;

        private const string CommandName = "/vfxedit";

        public VfxTracker Tracker;

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

            Configuration.Initialize( PluginInterface );

            ResourceLoader = new ResourceLoader( this );
            CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) {
                HelpMessage = "toggle ui"
            } );

            TemplateLocation = Path.GetDirectoryName( AssemblyLocation );
            
            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlot.SetCurrentContext( ImPlot.CreateContext() );

            DataHelper.Initialize();
            SheetManager.Initialize(
                Path.Combine( TemplateLocation, "Files", "npc.csv" ),
                Path.Combine( TemplateLocation, "Files", "monster_vfx.json" ),
                DataManager,
                PluginInterface
            );

            TextureManager.Initialize( this );
            DirectXManager.Initialize();
            DocumentManager.Initialize();
            FileDialogManager.Initialize( PluginInterface );
            CopyManager.Initialize();

            Tracker = new VfxTracker( this );

            InitUI();

            ResourceLoader.Init();
            ResourceLoader.Enable();

            PluginInterface.UiBuilder.Draw += Draw;
            PluginInterface.UiBuilder.Draw += FileDialogManager.Draw;
        }

        public void Dispose() {
            PluginInterface.UiBuilder.Draw -= FileDialogManager.Draw;
            PluginInterface.UiBuilder.Draw -= Draw;

            ImPlot.DestroyContext();

            CommandManager.RemoveHandler( CommandName );
            PluginInterface?.Dispose();

            ResourceLoader.Dispose();
            ResourceLoader = null;

            SpawnVfx?.Remove();
            SpawnVfx = null;

            Configuration.Dispose();
            FileDialogManager.Dispose();
            DirectXManager.Dispose();
            DocumentManager.Dispose();
            TextureManager.Dispose();
            DataHelper.Dispose();
            CopyManager.Dispose();
        }

        private void OnCommand( string command, string rawArgs ) {
            Visible = !Visible;
        }

        public void ClearSpawnVfx() {
            SpawnVfx = null;
        }
    }
}