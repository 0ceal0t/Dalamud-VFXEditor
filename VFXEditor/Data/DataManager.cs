using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lumina.Excel.GeneratedSheets;
using System.IO;
using VFXEditor.Data.Texture;

namespace VFXEditor.Data
{
    public class DataManager
    {
        public Plugin _plugin;
        public TextureManager TexManager;
        public string TempPath;
        public string NpcCsv;

        public DataManager(Plugin plugin )
        {
            _plugin = plugin;
            TexManager = new TextureManager( _plugin );
            NpcCsv = Path.Combine( Plugin.TemplateLocation, "Files", "npc.csv" );
        }

        // ========= LOAD ITEMS ===========
        public List<XivItem> Items = new List<XivItem>();
        public bool ItemsLoaded = false;
        public bool ItemsWaiting = false;
        public void LoadItems() {
            if( ItemsWaiting ) { return; }
            ItemsWaiting = true; // start waiting
            PluginLog.Log( "Loading Items" );
            Task.Run( async () => {
                try {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Item>().Where( x => x.EquipSlotCategory.Value?.MainHand == 1 || x.EquipSlotCategory.Value?.OffHand == 1 ).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return new List<Item>();
                }
            } ).ContinueWith( t => {
                foreach( var item in t.Result ) {
                    var i = new XivItem( item );
                    if( i.HasModel ) {
                        Items.Add( i );
                    }
                    if( i.HasSub ) {
                        Items.Add( i.SubItem );
                    }
                }
                ItemsLoaded = true;
            } );
        }
        public bool SelectItem( XivItem item, out XivItemSelected selectedItem ) {
            selectedItem = null;
            string imcPath = item.GetImcPath();
            bool result = _plugin.PluginInterface.Data.FileExists( imcPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    selectedItem = new XivItemSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return false;
                }
            }
            return result;
        }

        // =========== LOAD STATUS =========
        public List<XivStatus> Status = new List<XivStatus>();
        public bool StatusLoaded = false;
        public bool StatusWaiting = false;
        public void LoadStatus() {
            if( StatusWaiting ) { return; }
            StatusWaiting = true;
            PluginLog.Log( "Loading Status" );
            Task.Run( async () => {
                try {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Status>().Where( x => !string.IsNullOrEmpty( x.Name ) ).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return new List<Status>();
                }
            } ).ContinueWith( t => {
                foreach( var item in t.Result ) {
                    var i = new XivStatus( item );
                    if( i.VfxExists ) {
                        Status.Add( i );
                    }
                }
                StatusLoaded = true;
            } );

        }

        // ========= LOAD NON-PLAYER ACTIONS =======
        public bool NonPlayerActionsLoaded = false;
        public bool NonPlayerActionsWaiting = false;
        public List<XivActionBase> NonPlayerActions = new List<XivActionBase>();
        public void LoadNonPlayerActions() {
            if( NonPlayerActionsWaiting ) { return; }
            NonPlayerActionsWaiting = true;
            PluginLog.Log( "Loading Non-Player Actions" );
            Task.Run( async () => {
                try {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.IsPlayerAction ).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return new List<Lumina.Excel.GeneratedSheets.Action>();
                }
            } ).ContinueWith( t =>
            {
                foreach( var item in t.Result ) {
                    var action = new XivActionNonPlayer( item );
                    if( !action.IsPlaceholder ) {
                        if( action.HitVFXExists ) {
                            NonPlayerActions.Add( (XivActionNonPlayer) action.HitAction );
                        }
                        if( action.VfxExists ) {
                            NonPlayerActions.Add( action );
                        }
                    }
                }
                NonPlayerActionsLoaded = true;
            } );
        }

        // =========== LOAD ACTION =========
        public List<XivActionBase> Actions = new List<XivActionBase>();
        public bool ActionsLoaded = false;
        public bool ActionsWaiting = false;
        public void LoadActions() {
            if( ActionsWaiting ) { return; }
            ActionsWaiting = true;
            PluginLog.Log( "Loading Actions" );
            Task.Run( async () => {
                try {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.IsPlayerAction ).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return new List<Lumina.Excel.GeneratedSheets.Action>();
                }
            } ).ContinueWith( t => {
                foreach( var item in t.Result )
                {
                    var i = new XivAction( item );
                    if( i.VfxExists ) {
                        Actions.Add( i );
                    }
                    if( i.HitVFXExists ) {
                        Actions.Add( i.HitAction );
                    }
                }
                ActionsLoaded = true;
            } );

        }
        public bool SelectAction( XivActionBase action, out XivActionSelected selectedAction ) {
            selectedAction = null;
            if( !action.SelfVFXExists ) // no need to get a file
            {
                selectedAction = new XivActionSelected( null, action );
                return true;
            }
            string tmbPath = action.GetTmbPath();
            bool result = _plugin.PluginInterface.Data.FileExists( tmbPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile( tmbPath );
                    selectedAction = new XivActionSelected( file, action );
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return false;
                }
            }
            else {
                PluginLog.Log( tmbPath + " does not exist" );
            }
            return result;
        }

        // ========= LOAD ZONES ===========
        public List<XivZone> Zones = new List<XivZone>();
        public bool ZonesLoaded = false;
        public bool ZonesWaiting = false;
        public void LoadZones() {
            if( ZonesWaiting ) { return; }
            ZonesWaiting = true;
            PluginLog.Log( "Loading Zones" );
            Task.Run( async () => {
                try {
                    return _plugin.PluginInterface.Data.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty(x.Name)).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e, "Could not load zones" );
                    return new List<TerritoryType>();
                }
            } ).ContinueWith( t => {
                foreach( var item in t.Result ) {
                    Zones.Add( new XivZone( item ) );
                }
                ZonesLoaded = true;
            } );
        }
        public bool SelectZone( XivZone zone, out XivZoneSelected selectedZone ) {
            selectedZone = null;
            string lgbPath = zone.GetLgbPath();
            bool result = _plugin.PluginInterface.Data.FileExists( lgbPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile<Lumina.Data.Files.LgbFile>( lgbPath );
                    selectedZone = new XivZoneSelected( file, zone );
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return false;
                }
            }
            return result;
        }

        // ========= LOAD NPC ===========
        public List<XivNpc> Npcs = new List<XivNpc>();
        public Dictionary<int, string> NpcIdToName = new Dictionary<int, string>();
        public bool NpcLoaded = false;
        public bool NpcWaiting = false;
        public void LoadNpc() {
            if( NpcWaiting ) { return; }
            NpcWaiting = true;
            PluginLog.Log( "Loading Npc" );
            Task.Run( async () => {
                try {
                    var npcList = fastCSV.ReadFile<NpcCsvRow>( NpcCsv, true, ',', ( o, c ) => {
                         o.Id = Int32.Parse(c[0]);
                         o.Name = c[1];
                         NpcIdToName[o.Id] = o.Name;
                         return true;
                     } );
                    return _plugin.PluginInterface.Data.GetExcelSheet<ModelChara>().Where( x => (x.Model != 0 && (x.Type == 2 || x.Type == 3)) ).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e, "Could not load npc" );
                    return new List<ModelChara>();
                }
            } ).ContinueWith( t =>  {
                foreach( var item in t.Result ) {
                    var i = new XivNpc( item, NpcIdToName );
                    if(i.CSV_Defined ) {
                        Npcs.Add( i );
                    }
                }
                NpcLoaded = true;
            } );
        }
        public bool SelectNpc( XivNpc npc, out XivNpcSelected selectedNpc ) {
            selectedNpc = null;
            string imcPath = npc.GetImcPath();
            bool result = _plugin.PluginInterface.Data.FileExists( imcPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    var tmbPath = npc.GetTmbPath();
                    List< Lumina.Data.FileResource> files = new List<Lumina.Data.FileResource>();
                    for(int spIdx = 1; spIdx < 35; spIdx++ ) {
                        var mainTmb = tmbPath + "mon_sp" + spIdx.ToString().PadLeft( 3, '0' ) + ".tmb";
                        var hitTmb = tmbPath + "mon_sp" + spIdx.ToString().PadLeft( 3, '0' ) + "_hit.tmb";
                        if( _plugin.PluginInterface.Data.FileExists( mainTmb ) ) {
                            files.Add( _plugin.PluginInterface.Data.GetFile( mainTmb ) );
                        }
                        if( _plugin.PluginInterface.Data.FileExists( hitTmb ) ) {
                            files.Add( _plugin.PluginInterface.Data.GetFile( hitTmb ) );
                        }
                    }
                    selectedNpc = new XivNpcSelected( file, npc, files );
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return false;
                }
            }
            return result;
        }

        // ========= LOAD EMOTE ===========
        public List<XivEmote> Emotes = new List<XivEmote>();
        public bool EmoteLoaded = false;
        public bool EmoteWaiting = false;
        public void LoadEmote() {
            if( EmoteWaiting ) { return; }
            EmoteWaiting = true;
            PluginLog.Log( "Loading Emote" );
            Task.Run( async () => {
                try {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Emote>().Where( x => !string.IsNullOrEmpty(x.Name) ).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e, "Could not load emote" );
                    return new List<Emote>();
                }
            } ).ContinueWith( t => {
                foreach( var item in t.Result ) {
                    var i = new XivEmote( item );
                    if( i.PapFiles.Count > 0 ) {
                        Emotes.Add( i );
                    }
                }
                EmoteLoaded = true;
            } );
        }
        public bool SelectEmote( XivEmote emote, out XivEmoteSelected selectedEmote ) {
            selectedEmote = null;
            List<Lumina.Data.FileResource> files = new List<Lumina.Data.FileResource>();
            try {
                foreach( string path in emote.PapFiles ) {
                    var result = _plugin.PluginInterface.Data.FileExists( path );
                    if( result ) {
                        files.Add( _plugin.PluginInterface.Data.GetFile( path ) );
                    }
                }
                selectedEmote = new XivEmoteSelected( emote, files );
            }
            catch(Exception e ) {
                PluginLog.LogError( e.ToString() );
                return false;
            }
            return true;
        }

        // ============ LOAD GIMMICKS ===========
        public List<XivGimmick> Gimmicks = new List<XivGimmick>();
        public bool GimmickLoaded = false;
        public bool GimmickWaiting = false;
        public void LoadGimmicks() {
            if( GimmickWaiting ) { return; }
            GimmickWaiting = true;
            PluginLog.Log( "Loading Gimmicks" );
            Task.Run( async () => {
                try {
                    return _plugin.PluginInterface.Data.GetExcelSheet<ActionTimeline>().Where( x => x.Key.ToString().Contains("gimmick") ).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return new List<ActionTimeline>();
                }
            } ).ContinueWith( t => {
                var territories = _plugin.PluginInterface.Data.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty(x.Name) ).ToList();
                Dictionary<string, string> suffixToName = new Dictionary<string, string>();
                foreach(var _zone in territories ) {
                    suffixToName[_zone.Name.ToString()] = _zone.PlaceName.Value?.Name.ToString();
                }

                foreach( var item in t.Result ) {
                    var i = new XivGimmick( item, suffixToName );
                    Gimmicks.Add( i );
                }
                GimmickLoaded = true;
            } );

        }
        public bool SelectGimmick( XivGimmick gimmick, out XivGimmickSelected selectedGimmick ) {
            selectedGimmick = null;
            string tmbPath = gimmick.GetTmbPath();
            bool result = _plugin.PluginInterface.Data.FileExists( tmbPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile( tmbPath );
                    selectedGimmick = new XivGimmickSelected( file, gimmick );
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return false;
                }
            }
            else {
                PluginLog.Log( tmbPath + " does not exist" );
            }
            return result;
        }

        // ============ LOAD CUTSCENES ===========
        public List<XivCutscene> Cutscenes = new List<XivCutscene>();
        public bool CutsceneLoaded = false;
        public bool CutsceneWaiting = false;
        public void LoadCutscenes() {
            if( CutsceneWaiting ) { return; }
            CutsceneWaiting = true;
            PluginLog.Log( "Loading Cutscenes" );
            Task.Run( async () => {
                try {
                    return _plugin.PluginInterface.Data.GetExcelSheet<Cutscene>().Where( x => !string.IsNullOrEmpty(x.Path) ).ToList();
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return new List<Cutscene>();
                }
            } ).ContinueWith( t => {
                foreach( var item in t.Result ) {
                    Cutscenes.Add( new XivCutscene( item ) );
                }
                CutsceneLoaded = true;
            } );

        }
        public bool SelectCutscene( XivCutscene cutscene, out XivCutsceneSelected selectedCutscene ) {
            selectedCutscene = null;
            bool result = _plugin.PluginInterface.Data.FileExists( cutscene.Path );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile( cutscene.Path );
                    selectedCutscene = new XivCutsceneSelected( cutscene, file );
                }
                catch( Exception e ) {
                    PluginLog.LogError( e.ToString() );
                    return false;
                }
            }
            else {
                PluginLog.Log( cutscene.Path + " does not exist" );
            }
            return result;
        }


        // ======  EXPORT AVFX  ======
        public bool SaveLocalFile(string path, AVFXBase avfx ) {
            try {
                var node = avfx.toAVFX();
                var bytes = node.toBytes();
                File.WriteAllBytes( path, bytes );
            }
            catch(Exception ex ) {
                PluginLog.LogError( "Could not write to file: " + path );
                PluginLog.LogError( ex.ToString() );
                return false;
            }
            return true;
        }

        // ====== LOCAL AVFX =====
        public AVFXNode LastImportNode = null;
        public bool GetLocalFile(string path, out AVFXBase avfx) {
            avfx = null;
            try {
                AVFXNode node = AVFXLib.Main.Reader.readAVFX( path, out List<string> messages );
                foreach( string message in messages ) {
                    PluginLog.Log( message );
                }
                if( node == null )
                    return false;
                LastImportNode = node;
                AVFXBase _avfx = new AVFXBase();
                _avfx.read( node );
                avfx = _avfx;
            }
            catch(Exception e ) {
                PluginLog.LogError( e.ToString() );
                return false;
            }
            return true;
        }

        // ===== GAME AVFX ======
        public bool GetGameFile(string path, out AVFXBase avfx) {
            avfx = null;
            bool result = _plugin.PluginInterface.Data.FileExists( path );
            if( result )  {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile( path );
                    AVFXNode node = AVFXLib.Main.Reader.readAVFX( file.Data, out List<string> messages );
                    foreach(string message in messages ) {
                        PluginLog.Log( message );
                    }
                    if( node == null )
                        return false;
                    LastImportNode = node;
                    AVFXBase _avfx = new AVFXBase();
                    _avfx.read( node );
                    avfx = _avfx;
                }
                catch(Exception e) {
                    PluginLog.LogError( e.ToString() );
                    result = false;
                }
            }
            return result;
        }
    }
}
