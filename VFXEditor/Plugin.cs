using System;
using System.Collections.Generic;
using System.IO;
using AVFXLib.Models;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using ImPlotNET;
using ImGuizmoNET;
using VFXEditor.Data;
using VFXEditor.Data.DirectX;
using VFXEditor.External;
using VFXEditor.UI;
using VFXEditor.Data.Vfx;
using System.Reflection;

namespace VFXEditor
{
    public class Plugin : IDalamudPlugin {
        public string Name => "VFXEditor";
        private const string CommandName = "/vfxedit";

        public DalamudPluginInterface PluginInterface;
        public Configuration Configuration;
        public ResourceLoader ResourceLoader;
        public TexTools TexToolsManager;
        public Penumbra PenumbraManager;
        public DocManager Doc;
        public DataManager Manager;
        public DirectXManager DXManager;
        public VfxTracker Tracker;

        public MainInterface MainUI;

        public static string TemplateLocation;

        public AVFXBase AVFX {
            get { return Doc.ActiveDoc.AVFX; }
            set { Doc.ActiveDoc.AVFX = value; }
        }
        public string ReplaceAVFXPath {
            get { return Doc.ActiveDoc.Replace.Path; }
        }
        public string SourceString {
            get { return Doc.ActiveDoc.Source.DisplayString; }
        }
        public string ReplaceString {
            get { return Doc.ActiveDoc.Replace.DisplayString; }
        }

        public string WriteLocation;
        public string PluginDebugTitleStr;
        public string AssemblyLocation { get; set; } = Assembly.GetExecutingAssembly().Location;

        // ====== IMGUI ======
        public IntPtr ImPlotContext;
        public IntPtr ImNodesContext;

        public void Initialize( DalamudPluginInterface pluginInterface ) {
            PluginInterface = pluginInterface;
            WriteLocation = Path.Combine( new[] {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "XIVLauncher",
                "pluginConfigs",
                Name,
            } );
            Directory.CreateDirectory( WriteLocation ); // create if it doesn't already exist
            PluginLog.Log( "Write location: " + WriteLocation );

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize( PluginInterface );
            ResourceLoader = new ResourceLoader( this );
            PluginInterface.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand ) {
                HelpMessage = "/vfxedit - toggle ui"
            } );

            PluginDebugTitleStr = $"{Name} - Debug Build";

            TemplateLocation = Path.GetDirectoryName( AssemblyLocation );

            // ==== IMGUI ====
            ImPlot.SetImGuiContext( ImGui.GetCurrentContext() );
            ImPlotContext = ImPlot.CreateContext();
            ImPlot.SetCurrentContext( ImPlotContext );
            ImGuizmo.SetImGuiContext( ImGui.GetCurrentContext() );

            Tracker = new VfxTracker( this );
            ResourceLoader.Init();
            ResourceLoader.Enable();
            Manager = new DataManager( this );
            Doc = new DocManager( this );
            MainUI = new MainInterface( this );
            TexToolsManager = new TexTools( this );
            PenumbraManager = new Penumbra( this );
            DXManager = new DirectXManager( this );

            PluginInterface.UiBuilder.OnBuildUi += DXManager.Draw;
            PluginInterface.UiBuilder.OnBuildUi += MainUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi += Tracker.Draw;
        }

        public DateTime LastSelect = DateTime.Now;
        public void SelectAVFX(VFXSelectResult selectResult ) {
            if( ( DateTime.Now - LastSelect ).TotalSeconds < 0.5 ) { // only allow new selects every 1/2 second
                return;
            }
            LastSelect = DateTime.Now;
            switch( selectResult.Type ) {
                case VFXSelectType.Local: // LOCAL
                    bool localResult = Manager.GetLocalFile( selectResult.Path, out var localAvfx );
                    if( localResult ) {
                        LoadAVFX( localAvfx );
                    }
                    else {
                        PluginLog.Log( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
                default: // EVERYTHING ELSE: GAME FILES
                    bool gameResult = Manager.GetGameFile( selectResult.Path, out var gameAvfx );
                    if( gameResult ) {
                        LoadAVFX( gameAvfx );
                    }
                    else {
                        PluginLog.Log( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
            }
            Doc.UpdateSource( selectResult );
        }

        public void ReplaceAVFX(VFXSelectResult replaceResult ) {
            Doc.UpdateReplace( replaceResult );
        }
        public void RemoveReplaceAVFX() {
            Doc.UpdateReplace( VFXSelectResult.None() );
        }
        public void RefreshDoc() {
            if( Doc.HasVFX() ) {
                MainUI.RefreshAVFX();
            }
            else {
                UnloadAVFX();
            }
        }
        public void LoadAVFX(AVFXBase avfx ) {
            if( avfx == null )
                return;
            AVFX = avfx;
            // ===============
            if( Configuration.VerifyOnLoad ) {
                var node = AVFX.toAVFX();
                bool verifyResult = Manager.LastImportNode.CheckEquals( node, out List<string> messages );
                MainUI.SetStatus( verifyResult );
                PluginLog.Log( $"[VERIFY RESULT]: {verifyResult}" );
                foreach( var m in messages ) {
                    PluginLog.Log( m );
                }
            }
            Manager.TexManager.Reset();
            MainUI.RefreshAVFX();
        }
        public void UnloadAVFX() {
            AVFX = null;
            MainUI.UnloadAVFX();
        }
        public void Dispose() {
            PluginInterface.UiBuilder.OnBuildUi -= DXManager.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= MainUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= Tracker.Draw;

            // ====== IMGUI =======
            ImPlot.DestroyContext();

            PluginInterface.CommandManager.RemoveHandler( CommandName );
            PluginInterface?.Dispose();
            MainUI?.Dispose();
            ResourceLoader?.Dispose();
            DXManager?.Dispose();
            Doc?.Dispose();
            Manager.TexManager.Dispose();
        }
        private void OnCommand( string command, string rawArgs ) {
            MainUI.Visible = !MainUI.Visible;
        }
    }
}