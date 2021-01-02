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
        public Plugin _plugin;
        public string TempPath;

        public DataManager(Plugin plugin )
        {
            _plugin = plugin;
            // =======================
            TempPath = Path.Combine( Directory.GetCurrentDirectory(), "VFXTempFile.avfx" );
            PluginLog.Log( "Temp file location: " + TempPath );
        }

        // ========= LOAD ITEMS ===========
        public List<XivItem> Items = new List<XivItem>();
        public bool ItemsLoaded = false;
        public void LoadItems()
        {
            if( ItemsLoaded )
                return;
            ItemsLoaded = true;
            PluginLog.Log( "Loading Items" );
            Task.Run( async () => {
                try
                {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Item>().Where( x => x.EquipSlotCategory.Value?.MainHand == 1 || x.EquipSlotCategory.Value?.OffHand == 1 ).ToList();
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
                    }
                    if( i.HasSub )
                    {
                        Items.Add( i.SubItem );
                    }
                }
            } );
        }

        // =========== LOAD STATUS =========
        public List<XivStatus> Status = new List<XivStatus>();
        public bool StatusLoaded = false;
        public void LoadStatus()
        {
            if( StatusLoaded )
                return;
            StatusLoaded = true;
            PluginLog.Log( "Loading Status" );
            Task.Run( async () => {
                try
                {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Status>().Where( x => !string.IsNullOrEmpty( x.Name ) ).ToList();
                }
                catch( Exception e )
                {
                    PluginLog.LogError( e.ToString() );
                    return new List<Status>();
                }
            } ).ContinueWith( t =>
            {
                foreach( var item in t.Result )
                {
                    var i = new XivStatus( item );
                    if( i.VfxExists )
                    {
                        Status.Add( i );
                    }
                }
            } );

        }

        // =========== LOAD ACTION =========
        public List<XivAction> Actions = new List<XivAction>();
        public bool ActionsLoaded = false;
        public void LoadActions()
        {
            if( ActionsLoaded )
                return;
            ActionsLoaded = true;
            PluginLog.Log( "Loading Actions" );
            Task.Run( async () => {
                try
                {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) ).ToList();
                }
                catch( Exception e )
                {
                    PluginLog.LogError( e.ToString() );
                    return new List<Lumina.Excel.GeneratedSheets.Action>();
                }
            } ).ContinueWith( t =>
            {
                foreach( var item in t.Result )
                {
                    var i = new XivAction( item );
                    if( i.PlayerAction )
                    {
                        if( i.VfxExists )
                        {
                            Actions.Add( i );
                        }
                        if( i.HitVFXExists )
                        {
                            Actions.Add( i.HitAction );
                        }
                    }
                }
            } );

        }

        // ======= SELECT ITEM =======
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

        // ======= SELECT ACTION =======
        public bool SelectAction( XivAction action, out XivSelectedAction selectedAction )
        {
            selectedAction = null;
            if( !action.SelfVFXExists ) // no need to get a file
            {
                selectedAction = new XivSelectedAction( null, action );
                return true;
            }
            string tmbPath = action.GetTmbPath();
            bool result = _plugin.PluginInterface.Data.FileExists( tmbPath );
            if( result )
            {
                try
                {
                    var file = _plugin.PluginInterface.Data.GetFile( tmbPath );
                    selectedAction = new XivSelectedAction( file, action );
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

        public AVFXNode LastImportNode = null;
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
                LastImportNode = node;
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
                    if( node == null )
                        return false;
                    LastImportNode = node;
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
