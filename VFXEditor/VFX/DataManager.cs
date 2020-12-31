using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lumina.Excel.GeneratedSheets;

namespace VFXEditor
{
    public class DataManager
    {
        public List<XivItem> Items;
        public Plugin _plugin;

        public DataManager(Plugin plugin )
        {
            _plugin = plugin;
            // =======================

            /*
            Task.Run( async () => {
                try
                {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.VFX>().ToList();
                }
                catch( Exception e)
                {
                    PluginLog.LogError( e.ToString() );
                    return new List<Lumina.Excel.GeneratedSheets.VFX>();
                }
            } ).ContinueWith( t => {
                foreach(var vfx in t.Result )
                {
                    PluginLog.Log( vfx.Location );
                }
            } );
            */

            Items = new List<XivItem>();
            Task.Run( async () => {
                try
                {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Item>().Where( i => !string.IsNullOrEmpty( i.Name ) ).ToList();
                }
                catch( Exception e )
                {
                    PluginLog.LogError( e.ToString() );
                    return new List<Item>();
                }
            } ).ContinueWith( t =>
            {
                foreach( var item in t.Result )
                {
                    var i = new XivItem( item );
                    if( i.HasModel )
                    {
                        Items.Add( i );
                        /*if( i.HasSub )
                        {
                            Items.Add( i.SubItem );
                        }*/
                    }
                }
            } );
        }

        // Export avfx

        // Temp avfx for update

        // Get AVFX from local
        public bool GetLocalFile(string path, out AVFXBase avfx)
        {
            avfx = null;
            try
            {
                AVFXNode node = AVFXLib.Main.Reader.readAVFX( path, out List<string> messages );
                foreach( string message in messages )
                {
                    PluginLog.Log( message );
                }
                if( node == null )
                    return false;
                AVFXBase _avfx = new AVFXBase();
                _avfx.read( node );
                avfx = _avfx;
            }
            catch(Exception e )
            {
                PluginLog.LogError( e.ToString() );
                return false;
            }
            return true;
        }

        // Get AVFX from game
        public bool GetGameFile(string path, out AVFXBase avfx)
        {
            avfx = null;
            bool result = !_plugin.PluginInterface.Data.FileExists( path );
            if( result )
            {
                try
                {
                    var file = _plugin.PluginInterface.Data.GetFile( path );
                    AVFXNode node = AVFXLib.Main.Reader.readAVFX( file.Data, out List<string> messages );
                    foreach(string message in messages )
                    {
                        PluginLog.Log( message );
                    }
                    AVFXBase _avfx = new AVFXBase();
                    _avfx.read( node );
                    avfx = _avfx;
                }
                catch(Exception e)
                {
                    PluginLog.LogError( e.ToString() );
                    result = false;
                }
            }
            return result;
        }
    }
}
