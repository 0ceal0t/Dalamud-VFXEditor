using System;
using System.IO;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using ImPlotNET;

using VFXEditor.Data;
using VFXEditor.Tmb;
using VFXEditor.DirectX;
using VFXEditor.Texture;
using VFXEditor.Tracker;
using VFXEditor.Document;

using VFXEditor.Structs.Vfx;
using VFXSelect;

using System.Reflection;
using ImGuiFileDialog;

using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;

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

            Configuration.Initialize();

            ResourceLoader = new ResourceLoader();
            CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) {
                HelpMessage = "toggle ui"
            } );

            TemplateLocation = Path.GetDirectoryName( AssemblyLocation );
            
            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlot.SetCurrentContext( ImPlot.CreateContext() );

            AvfxHelper.Initialize();
            SheetManager.Initialize(
                Path.Combine( TemplateLocation, "Files", "npc.csv" ),
                Path.Combine( TemplateLocation, "Files", "monster_vfx.json" ),
                DataManager,
                PluginInterface
            );

            TmbManager.Initialize( this );
            TextureManager.Initialize();
            DirectXManager.Initialize();
            DocumentManager.Initialize();
            FileDialogManager.Initialize( PluginInterface );
            CopyManager.Initialize();
            VfxTracker.Initialize();

            InitUI();

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
            PluginInterface?.Dispose();

            ResourceLoader.Dispose();
            ResourceLoader = null;

            SpawnVfx?.Remove();
            SpawnVfx = null;

            Configuration.Dispose();
            FileDialogManager.Dispose();
            DirectXManager.Dispose();
            DocumentManager.Dispose();
            TmbManager.Dispose();
            TextureManager.Dispose();
            AvfxHelper.Dispose();
            CopyManager.Dispose();
        }

        private void DrawConfigUI() {
            Visible = true;
        }

        private void OnCommand( string command, string rawArgs ) {
            Visible = !Visible;
        }

        public static bool SpawnExists() {
            return SpawnVfx != null;
        }

        public static void RemoveSpawnVfx() {
            SpawnVfx?.Remove();
            SpawnVfx = null;
        }

        public static void SpawnOnGround( string path ) {
            SpawnVfx = new StaticVfx( path, ClientState.LocalPlayer.Position );
        }

        public static void SpawnOnSelf( string path ) {
            SpawnVfx = new ActorVfx( ClientState.LocalPlayer, ClientState.LocalPlayer, path );
        }

        public static void SpawnOnTarget( string path ) {
            var t = TargetManager.Target;
            if( t != null ) {
                SpawnVfx = new ActorVfx( t, t, path );
            }
        }

        public static void ClearSpawnVfx() {
            SpawnVfx = null;
        }
    }
}