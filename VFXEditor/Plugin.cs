using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using AVFXLib.Models;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using VFXEditor.UI;

namespace VFXEditor
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "VFXEditor";
        private const string CommandName = "/vfxedit";

        public DalamudPluginInterface PluginInterface { get; set; }
        public Configuration Configuration { get; set; }
        public ResourceLoader ResourceLoader { get; set; }

        public MainInterface MainUI { get; set; }
        public VFXSelectDialog SelectUI { get; set; }

        public AVFXBase AVFX = null;
        public DataManager Manager;

        public string PluginDebugTitleStr { get; private set; }

        //https://git.sr.ht/~jkcclemens/NoSoliciting/tree/master/item/NoSoliciting/Plugin.cs#L53

        public void Initialize( DalamudPluginInterface pluginInterface )
        {
            PluginInterface = pluginInterface;

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize( PluginInterface );

            ResourceLoader = new ResourceLoader( this );

            PluginInterface.CommandManager.AddHandler( CommandName, new CommandInfo( OnCommand )
            {
                HelpMessage = "/VFXEditor - toggle ui\n/VFXEditor reload - reload mod file lists & discover any new mods"
            } );

            ResourceLoader.Init();
            ResourceLoader.Enable();

            MainUI = new MainInterface( this );
            SelectUI = new VFXSelectDialog( this, "LoadDialog");
            Manager = new DataManager( this );
            PluginInterface.UiBuilder.OnBuildUi += MainUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi += SelectUI.Draw;

            PluginDebugTitleStr = $"{Name} - Debug Build";
        }

        public void LoadAVFX(AVFXBase avfx )
        {
            if( avfx == null )
                return;
            AVFX = avfx;
            MainUI.RefreshAVFX();
        }

        public void UnloadAVFX()
        {
            AVFX = null;
            MainUI.UnloadAVFX();
        }

        public void Dispose()
        {
            PluginInterface.UiBuilder.OnBuildUi -= MainUI.Draw;
            PluginInterface.UiBuilder.OnBuildUi -= SelectUI.Draw;

            PluginInterface.CommandManager.RemoveHandler( CommandName );
            PluginInterface.Dispose();

            ResourceLoader.Dispose();
        }

        private void OnCommand( string command, string rawArgs )
        {
            var args = rawArgs.Split( ' ' );
            if( args.Length > 0 )
            {
                switch( args[ 0 ] )
                {
                }

                return;
            }

            MainUI.Visible = !MainUI.Visible;
        }
    }
}