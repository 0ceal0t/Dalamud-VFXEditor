using System;
using System.IO;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using ImPlotNET;
using ImGuizmoNET;
using VFXEditor.Data;
using VFXEditor.DirectX;
using VFXEditor.External;
using VFXEditor.Data.Vfx;
using System.Reflection;
using VFXEditor.Data.Texture;
using VFXSelect;
using ImGuiFileDialog;

namespace VFXEditor
{
    public partial class Plugin : IDalamudPlugin {
        public string Name => "VFXEditor";
        private const string CommandName = "/vfxedit";
        private bool PluginReady => PluginInterface.Framework.Gui.GetBaseUIObject() != IntPtr.Zero;

        public DalamudPluginInterface PluginInterface;
        public ResourceLoader ResourceLoader;
        public DocumentManager DocManager;
        public VfxTracker Tracker;
        public TextureManager TexManager;
        public SheetManager Sheets;
        public static FileDialogManager DialogManager;

        public static string TemplateLocation;

        public string AssemblyLocation { get; set; } = Assembly.GetExecutingAssembly().Location;

        private IntPtr ImPlotContext;

        public void Initialize( DalamudPluginInterface pluginInterface ) {
            PluginInterface = pluginInterface;
            Configuration.Initialize( PluginInterface );
            Directory.CreateDirectory( Configuration.Config.WriteLocation );
            PluginLog.Log( "Write location: " + Configuration.Config.WriteLocation );

            ResourceLoader = new ResourceLoader( this );
            PluginInterface.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) {
                HelpMessage = "toggle ui"
            } );

            TemplateLocation = Path.GetDirectoryName( AssemblyLocation );

            DialogManager = new FileDialogManager();

            // ==== IMGUI ====
            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlotContext = ImPlot.CreateContext();
            ImPlot.SetCurrentContext( ImPlotContext );

            Tracker = new VfxTracker( this );
            TextureManager.Initialize( this );
            Sheets = new SheetManager( PluginInterface, Path.Combine( TemplateLocation, "Files", "npc.csv" ) );
            DocManager = new DocumentManager( this );
            DirectXManager.Initialize( this );

            InitUI();

            ResourceLoader.Init();
            ResourceLoader.Enable();

            PluginInterface.UiBuilder.OnBuildUi += Draw;
            PluginInterface.UiBuilder.OnBuildUi += DialogManager.Draw;
        }

        public void Draw() {
            if( !PluginReady ) return;

            DrawUI();
            Tracker.Draw();
        }

        public void Dispose() {
            PluginInterface.UiBuilder.OnBuildUi -= DialogManager.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= Draw;
            ResourceLoader?.Dispose();

            DialogManager.Reset();

            ImPlot.DestroyContext();

            PluginInterface.CommandManager.RemoveHandler( CommandName );
            PluginInterface?.Dispose();
            SpawnVfx?.Remove();
            DirectXManager.Dispose();
            DocManager?.Dispose();
            TextureManager.Dispose();
        }

        private void OnCommand( string command, string rawArgs ) {
            Visible = !Visible;
        }
    }
}