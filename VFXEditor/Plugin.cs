using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using AVFXLib.Models;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using VFXEditor.UI;

namespace VFXEditor
{
    public class Plugin : IDalamudPlugin {
        public string Name => "VFXEditor";
        private const string CommandName = "/vfxedit";

        public DalamudPluginInterface PluginInterface { get; set; }
        public Configuration Configuration { get; set; }
        public ResourceLoader ResourceLoader { get; set; }
        public TexTools TexToolsManager { get; set; }
        public Penumbra PenumbraManager { get; set; }

        public MainInterface MainUI { get; set; }
        public VFXSelectDialog SelectUI { get; set; }
        public VFXSelectDialog PreviewUI { get; set; }
        public TexToolsDialog TexToolsUI { get; set; }
        public PenumbraDialog PenumbraUI { get; set; }
        //public UISavedDialog SavedLoadUI { get; set; }
        //public UISavedDialog SavedExportUI { get; set; }

        public AVFXBase AVFX = null;
        public DataManager Manager;

        public string TemplateLocation;
        public string WriteLocation;

        public string PluginDebugTitleStr { get; private set; }

        //https://git.sr.ht/~jkcclemens/NoSoliciting/tree/master/item/NoSoliciting/Plugin.cs#L53

        public void Initialize( DalamudPluginInterface pluginInterface ) {
            PluginInterface = pluginInterface;

            WriteLocation = Path.Combine( new[] {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "XIVLauncher",
                "pluginConfigs",
                Name,
            } );
            Directory.CreateDirectory( WriteLocation ); // create if it doesn't already exist

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize( PluginInterface, WriteLocation );
            ResourceLoader = new ResourceLoader( this );
            PluginInterface.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand )
            {
                HelpMessage = "/vfxedit - toggle ui"
            } );

#if !DEBUG
            TemplateLocation = Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
#else
            TemplateLocation = @"D:\FFXIV\TOOLS\Dalamud-VFXEditor\VFXEditor\bin\Debug\net472";
#endif
            ResourceLoader.Init();
            ResourceLoader.Enable();
            MainUI = new MainInterface( this );
            Manager = new DataManager( this );
            TexToolsManager = new TexTools( this );
            PenumbraManager = new Penumbra( this );

            SelectUI = new VFXSelectDialog( this, "Load File" );
            PreviewUI = new VFXSelectDialog( this, "Replace File" );
            SelectUI.OnSelect += SelectAVFX;
            PreviewUI.OnSelect += ReplaceAVFX;
            TexToolsUI = new TexToolsDialog( this );
            PenumbraUI = new PenumbraDialog( this );
            //SavedLoadUI = new UISavedDialog( this, "Load Saved", load:true);
            //SavedExportUI = new UISavedDialog( this, "Export Saved" );

            PluginInterface.UiBuilder.OnBuildUi += MainUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi += SelectUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi += PreviewUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi += TexToolsUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi += PenumbraUI.Draw;
            //PluginInterface.UiBuilder.OnBuildUi += SavedLoadUI.Draw;
            //PluginInterface.UiBuilder.OnBuildUi += SavedExportUI.Draw;
            PluginDebugTitleStr = $"{Name} - Debug Build";
        }

        public DateTime LastSelect = DateTime.Now;
        public void SelectAVFX(VFXSelectResult selectResult ) {
            if( ( DateTime.Now - LastSelect ).TotalSeconds < 2 ) {
                return;
            }
            LastSelect = DateTime.Now;
            switch( selectResult.Type ) {
                case VFXSelectType.Local:
                    bool localResult = Manager.GetLocalFile( selectResult.Path, out var localAvfx );
                    if( localResult ) {
                        LoadAVFX( localAvfx );
                    }
                    else {
                        PluginLog.Log( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
                case VFXSelectType.GameItem:
                case VFXSelectType.GamePath:
                case VFXSelectType.GameStatus:
                case VFXSelectType.GameAction:
                case VFXSelectType.GameZone:
                case VFXSelectType.GameEmote:
                case VFXSelectType.GameCutscene:
                case VFXSelectType.GameNpc:
                    bool gameResult = Manager.GetGameFile( selectResult.Path, out var gameAvfx );
                    if( gameResult ) {
                        LoadAVFX( gameAvfx );
                    }
                    else {
                        PluginLog.Log( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
                default:
                    return;
            }
            MainUI.sourceString = selectResult.DisplayString;
        }

        public string ReplaceAVFXPath = "";
        public void ReplaceAVFX(VFXSelectResult replaceResult ) {
            switch( replaceResult.Type ) {
                case VFXSelectType.GameItem:
                case VFXSelectType.GamePath:
                case VFXSelectType.GameStatus:
                case VFXSelectType.GameAction:
                case VFXSelectType.GameZone:
                case VFXSelectType.GameEmote:
                case VFXSelectType.GameCutscene:
                case VFXSelectType.GameNpc:
                    ReplaceAVFXPath = replaceResult.Path;
                    break;
                default:
                    return;
            }
            MainUI.previewString = replaceResult.DisplayString;
        }
        public void RemoveReplaceAVFX() {
            ReplaceAVFXPath = "";
            MainUI.previewString = "[NONE]";
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

                PluginLog.Log( "[VERIFY RESULT]: " + verifyResult );
                foreach( var m in messages ) {
                    PluginLog.Log( m );
                }
            }
            MainUI.RefreshAVFX();
        }

        public void UnloadAVFX() {
            AVFX = null;
            MainUI.UnloadAVFX();
        }

        public void Dispose() {
            PluginInterface.UiBuilder.OnBuildUi -= MainUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= SelectUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= PreviewUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= TexToolsUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= PenumbraUI.Draw;
            //PluginInterface.UiBuilder.OnBuildUi -= SavedLoadUI.Draw;
            //PluginInterface.UiBuilder.OnBuildUi -= SavedExportUI.Draw;

            PluginInterface.CommandManager.RemoveHandler( CommandName );
            PluginInterface.Dispose();
            ResourceLoader.Dispose();
        }

        private void OnCommand( string command, string rawArgs ) {
            var args = rawArgs.Split( ' ' );
            if( args.Length > 0 ) {
                //switch( args[ 0 ] ) {} return;
            }
            MainUI.Visible = !MainUI.Visible;
        }
    }
}