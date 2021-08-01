using System;
using System.IO;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using ImPlotNET;
using VFXEditor.Data;
using VFXEditor.DirectX;
using VFXEditor.Data.Vfx;
using System.Reflection;
using VFXEditor.Data.Texture;
using VFXSelect;
using ImGuiFileDialog;

namespace VFXEditor
{
    public partial class Plugin : IDalamudPlugin {
        public string Name => "VFXEditor";
        public string AssemblyLocation { get; set; } = Assembly.GetExecutingAssembly().Location;
        public DalamudPluginInterface PluginInterface;

        private const string CommandName = "/vfxedit";
        private bool PluginReady => PluginInterface.Framework.Gui.GetBaseUIObject() != IntPtr.Zero;

        public ResourceLoader ResourceLoader;
        public VfxTracker Tracker;
        public SheetManager Sheets;

        public static string TemplateLocation { get; private set; }

        public void Initialize( DalamudPluginInterface pluginInterface ) {
            PluginInterface = pluginInterface;

            Configuration.Initialize( PluginInterface );

            ResourceLoader = new ResourceLoader( this );
            PluginInterface.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) {
                HelpMessage = "toggle ui"
            } );

            TemplateLocation = Path.GetDirectoryName( AssemblyLocation );
            
            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlot.SetCurrentContext( ImPlot.CreateContext() );

            DataManager.Initialize( this );
            TextureManager.Initialize( this );
            DirectXManager.Initialize( this );
            DocumentManager.Initialize();
            FileDialogManager.Initialize();
            CopyManager.Initialize();

            Tracker = new VfxTracker( this );
            Sheets = new SheetManager( PluginInterface, Path.Combine( TemplateLocation, "Files", "npc.csv" ) );

            InitUI();

            ResourceLoader.Init();
#if !DEBUG
            ResourceLoader.Enable();
#endif

            PluginInterface.UiBuilder.OnBuildUi += Draw;
            PluginInterface.UiBuilder.OnBuildUi += FileDialogManager.Draw;
        }

        public void Dispose() {

            PluginInterface.UiBuilder.OnBuildUi -= FileDialogManager.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= Draw;

            ImPlot.DestroyContext();

            PluginInterface.CommandManager.RemoveHandler( CommandName );
            PluginInterface?.Dispose();

#if !DEBUG
            ResourceLoader.Dispose();
#endif
            ResourceLoader = null;

            SpawnVfx?.Remove();

            Configuration.Dispose();
            FileDialogManager.Dispose();
            DirectXManager.Dispose();
            DocumentManager.Dispose();
            TextureManager.Dispose();
            DataManager.Dispose();
            CopyManager.Dispose();
        }

        private void OnCommand( string command, string rawArgs ) {
            Visible = !Visible;
        }
    }
}