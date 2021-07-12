using System;
using System.IO;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using ImPlotNET;
using ImGuizmoNET;
using VFXEditor.Data;
using VFXEditor.Data.DirectX;
using VFXEditor.External;
using VFXEditor.Data.Vfx;
using System.Reflection;
using VFXEditor.Data.Texture;
using VFXSelect;
#if DEBUG
using HarmonyLib;
#endif

namespace VFXEditor
{
    public partial class Plugin : IDalamudPlugin {
        public string Name => "VFXEditor";
        private const string CommandName = "/vfxedit";
        private bool PluginReady => PluginInterface.Framework.Gui.GetBaseUIObject() != IntPtr.Zero;

        public DalamudPluginInterface PluginInterface;
        public Configuration Configuration;
        public ResourceLoader ResourceLoader;
        public DocumentManager DocManager;
        public DirectXManager DXManager;
        public VfxTracker Tracker;
        public TextureManager TexManager;
        public SheetManager Sheets;

        public static string TemplateLocation;

        public string WriteLocation => Configuration?.WriteLocation;

        public string AssemblyLocation { get; set; } = Assembly.GetExecutingAssembly().Location;

# if DEBUG
        public static string Patch_AssemblyLocation;
# endif

        private IntPtr ImPlotContext;

        public void Initialize( DalamudPluginInterface pluginInterface ) {
            PluginInterface = pluginInterface;
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize( PluginInterface );
            Directory.CreateDirectory( WriteLocation ); // create if it doesn't already exist
            PluginLog.Log( "Write location: " + WriteLocation );

#if DEBUG
            Patch_AssemblyLocation = AssemblyLocation;
            ApplyPatches();
#endif

            ResourceLoader = new ResourceLoader( this );
            PluginInterface.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) {
                HelpMessage = "toggle ui"
            } );

            TemplateLocation = Path.GetDirectoryName( AssemblyLocation );

            // ==== IMGUI ====
            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlotContext = ImPlot.CreateContext();
            ImPlot.SetCurrentContext( ImPlotContext );
            ImGuizmo.SetImGuiContext( ImGui.GetCurrentContext() );

            Tracker = new VfxTracker( this );
            TexManager = new TextureManager( this );
            TexManager.OneTimeSetup();
            Sheets = new SheetManager( PluginInterface, Path.Combine( TemplateLocation, "Files", "npc.csv" ) );
            DocManager = new DocumentManager( this );
            DXManager = new DirectXManager( this );

            InitUI();

            ResourceLoader.Init();
            ResourceLoader.Enable();

            PluginInterface.UiBuilder.OnBuildUi += Draw;
        }

        public void Draw() {
            if( !PluginReady ) return;
            DrawUI();
            Tracker.Draw();
        }

        public void Dispose() {
            PluginInterface.UiBuilder.OnBuildUi -= Draw;
            ResourceLoader?.Dispose();

            ImPlot.DestroyContext();

            PluginInterface.CommandManager.RemoveHandler( CommandName );
            PluginInterface?.Dispose();
            SpawnVfx?.Remove();
            DXManager?.Dispose();
            DocManager?.Dispose();
            TexManager?.Dispose();
        }

        private void OnCommand( string command, string rawArgs ) {
            Visible = !Visible;
        }

#if DEBUG
        private void ApplyPatches() {
            var harmony = new Harmony( "com.vfxeditor.resourceloader" );

            var targetType = typeof( Plugin ).Assembly.GetType();

            var locationTarget = AccessTools.PropertyGetter( targetType, nameof( Assembly.Location ) );
            var locationPatch = AccessTools.Method( typeof( Plugin ), nameof( Plugin.AssemblyLocationPatch ) );
            harmony.Patch( locationTarget, postfix: new( locationPatch ) );

#pragma warning disable SYSLIB0012 // Type or member is obsolete
            var codebaseTarget = AccessTools.PropertyGetter( targetType, nameof( Assembly.CodeBase ) );
            var codebasePatch = AccessTools.Method( typeof( Plugin ), nameof( Plugin.AssemblyCodeBasePatch ) );
            harmony.Patch( codebaseTarget, postfix: new( codebasePatch ) );
#pragma warning restore SYSLIB0012 // Type or member is obsolete
        }

        private static void AssemblyLocationPatch( Assembly __instance, ref string __result ) {
            __result = Patch_AssemblyLocation;
        }

        private static void AssemblyCodeBasePatch( Assembly __instance, ref string __result ) {
            __result = Patch_AssemblyLocation;
        }
#endif
    }
}