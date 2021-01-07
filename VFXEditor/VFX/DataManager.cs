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
        public TextureManager TexManager;
        public string TempPath;

        public DataManager(Plugin plugin )
        {
            _plugin = plugin;
            TexManager = new TextureManager( _plugin );
            // =======================
            TempPath = Path.Combine( _plugin.WriteLocation, "VFXTempFile.avfx" );
            PluginLog.Log( "Temp file location: " + TempPath );
        }

        // ========= LOAD NON-PLAYER ACTIONS =======
        public bool NonPlayerActionsLoaded = false;
        public bool NonPlayerActionsWaiting = false;
        public List<XivActionBase> NonPlayerActions = new List<XivActionBase>();
        public Dictionary<string, List<string>> ActionIdToSkelId = new Dictionary<string, List<string>>();
        private struct NonPlayerActionRes
        {
            public List<Lumina.Excel.GeneratedSheets.Action> Actions;
            public List<Lumina.Excel.GeneratedSheets.ActionTimeline> Timelines;
            public NonPlayerActionRes( List<Lumina.Excel.GeneratedSheets.Action> a, List<Lumina.Excel.GeneratedSheets.ActionTimeline> t )
            {
                Actions = a;
                Timelines = t;
            }
        }
        public void LoadNonPlayerActions()
        {
            if( NonPlayerActionsWaiting )
                return;
            NonPlayerActionsWaiting = true; // start waiting
            PluginLog.Log( "Loading Non-Player Actions" );
            Task.Run( async () => {
                try
                {
                    return new NonPlayerActionRes(
                        _plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.IsPlayerAction ).ToList(),
                        _plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.ActionTimeline>().Where( x => !string.IsNullOrEmpty( x.Key )).ToList()
                    );
                }
                catch( Exception e )
                {
                    PluginLog.LogError( e.ToString() );
                    return new NonPlayerActionRes(
                        new List<Lumina.Excel.GeneratedSheets.Action>(),
                        new List<Lumina.Excel.GeneratedSheets.ActionTimeline>()
                    );
                }
            } ).ContinueWith( t =>
            {
                foreach( var tl in t.Result.Timelines ) // SET UP ACTIONS WITH VARIABLE SKELETON
                {
                    string key = tl.Key.ToString();
                    MonsterKey mKey = new MonsterKey( key );
                    if(mKey.isMonster && mKey.skeletonKey == "[SKL_ID]" )
                    {
                        ActionIdToSkelId.Add( mKey.actionId, new List<string>() );
                    }
                }
                foreach( var tl in t.Result.Timelines ) // GET THE POSSIBLE SKELETON ID VALUES
                {
                    string key = tl.Key.ToString();
                    MonsterKey mKey = new MonsterKey( key );
                    if( mKey.isMonster && mKey.skeletonKey != "[SKL_ID]" && ActionIdToSkelId.ContainsKey(mKey.actionId))
                    {
                        if( ActionIdToSkelId[mKey.actionId].Count == 0 ) // TEMP: keep the size of the array small for now, just use 1
                        {
                            ActionIdToSkelId[mKey.actionId].Add( mKey.skeletonKey );
                        }
                    }
                }

                // finally, set up the actions
                foreach( var item in t.Result.Actions )
                {
                    var i = new XivNonPlayerAction( item, ActionIdToSkelId );
                    AddNonPlayerAction( i );
                }

                NonPlayerActionsLoaded = true;
            } );
        }
        private void AddNonPlayerAction(XivNonPlayerAction action)
        {
            if( action.IsPlaceholder )
            {
                foreach( var _i in action.PlaceholderActions )
                {
                    NonPlayerActions.Add( _i );
                }
            }
            if( action.HitVFXExists )
            {
                AddNonPlayerAction( ( XivNonPlayerAction) action.HitAction );
            }
            if( action.VfxExists )
            {
                NonPlayerActions.Add( action );
            }
        }


        // ========= LOAD ITEMS ===========
        public List<XivItem> Items = new List<XivItem>();
        public bool ItemsLoaded = false;
        public bool ItemsWaiting = false;
        public void LoadItems()
        {
            if( ItemsWaiting )
                return;
            ItemsWaiting = true; // start waiting
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
                ItemsLoaded = true;
            } );
        }

        // =========== LOAD STATUS =========
        public List<XivStatus> Status = new List<XivStatus>();
        public bool StatusLoaded = false;
        public bool StatusWaiting = false;
        public void LoadStatus()
        {
            if( StatusWaiting )
                return;
            StatusWaiting = true; // start waiting
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
                StatusLoaded = true;
            } );

        }

        // =========== LOAD ACTION =========
        public List<XivActionBase> Actions = new List<XivActionBase>();
        public bool ActionsLoaded = false;
        public bool ActionsWaiting = false;
        public void LoadActions()
        {
            if( ActionsWaiting )
                return;
            ActionsWaiting = true; // start waiting
            PluginLog.Log( "Loading Actions" );
            Task.Run( async () => {
                try
                {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.IsPlayerAction ).ToList();
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
                    if( i.VfxExists )
                    {
                        Actions.Add( i );
                    }
                    if( i.HitVFXExists )
                    {
                        Actions.Add( i.HitAction );
                    }
                }
                ActionsLoaded = true;
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
        public bool SelectAction( XivActionBase action, out XivSelectedAction selectedAction )
        {
            selectedAction = null;
            if( !action.SelfVFXExists ) // no need to get a file
            {
                selectedAction = new XivSelectedAction( null, action );
                PluginLog.Log( "no need" );
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
            else
            {
                PluginLog.Log( tmbPath + " does not exist" );
            }
            return result;
        }

        // ======  EXPORT AVFX  ======
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

        // ====== TEMP AVFX ====
        public bool SaveTempFile(AVFXBase avfx )
        {
            return SaveLocalFile( TempPath, avfx );
        }

        // ====== LOCAL AVFX =====
        public AVFXNode LastImportNode = null;
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

        // ===== GAME AVFX ======
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
