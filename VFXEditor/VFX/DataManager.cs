using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lumina.Excel.GeneratedSheets;
using System.IO;

namespace VFXEditor
{
    public class DataManager
    {
        public List<XivItem> Items;
        public Plugin _plugin;
        public string TempPath;

        public DataManager(Plugin plugin )
        {
            _plugin = plugin;
            // =======================
            TempPath = Path.Combine( Directory.GetCurrentDirectory(), "VFXTempFile.avfx" );
            PluginLog.Log( "Temp file location: " + TempPath );

            //_plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.VFX>().ToList();

            Items = new List<XivItem>();
            Task.Run( async () => {
                try
                {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Item>().Where( x => x.EquipSlotCategory.Value?.MainHand == 1 || x.EquipSlotCategory.Value?.OffHand == 1).ToList();
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
                        if( i.HasSub )
                        {
                            Items.Add( i.SubItem );
                        }
                    }
                }
            } );
        }

        // ======= Select item =======
        public bool SelectItem(XivItem item, out XivSelectedItem selectedItem)
        {
            selectedItem = null;
            string imcPath = item.GetImcPath();
            bool result = _plugin.PluginInterface.Data.FileExists( imcPath );
            if( result )
            {
                try
                {
                    var file = _plugin.PluginInterface.Data.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    selectedItem = new XivSelectedItem(file, item);
                }
                catch( Exception e )
                {
                    PluginLog.LogError( e.ToString() );
                    return false;
                }
            }
            return result;
        }

        // ======  Export avfx  ======
        public bool SaveLocalFile(string path, AVFXBase avfx )
        {
            try
            {
                var node = avfx.toAVFX();
                var bytes = node.toBytes();
                File.WriteAllBytes( path, bytes );
            }
            catch(Exception ex )
            {
                PluginLog.LogError( "Could not write to file: " + path );
                PluginLog.LogError( ex.ToString() );
                return false;
            }
            return true;
        }

        // ====== Temp avfx for update ====
        public bool SaveTempFile(AVFXBase avfx )
        {
            return SaveLocalFile( TempPath, avfx );
        }

        // ====== Get AVFX from local =====
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

        // ===== Get AVFX from game ======
        public bool GetGameFile(string path, out AVFXBase avfx)
        {
            avfx = null;
            bool result = _plugin.PluginInterface.Data.FileExists( path );
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
