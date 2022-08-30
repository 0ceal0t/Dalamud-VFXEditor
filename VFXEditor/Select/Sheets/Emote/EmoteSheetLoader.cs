using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class EmoteSheetLoader : SheetLoader<XivEmote, XivEmoteSelected> {
        public override void OnLoad() {
            var sheet = VfxEditor.DataManager.GetExcelSheet<Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) {
                var i = new XivEmote( item );
                if( i.PapFiles.Count > 0 ) {
                    Items.Add( i );
                }
            }
        }

        public override bool SelectItem( XivEmote item, out XivEmoteSelected selectedItem ) {
            selectedItem = null;
            var files = new List<Lumina.Data.FileResource>();
            try {
                foreach( var path in item.PapFiles ) {
                    var result = VfxEditor.DataManager.FileExists( path );
                    if( result ) {
                        files.Add( VfxEditor.DataManager.GetFile( path ) );
                    }
                }
                selectedItem = new XivEmoteSelected( item, files );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error reading emote file" );
                return false;
            }
            return true;
        }
    }
}
