using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class EmoteSheetLoader : SheetLoader<XivEmote, XivEmoteSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );

            foreach( var item in sheet ) {
                var emoteItem = new XivEmote( item );
                if( emoteItem.PapFiles.Count > 0 ) Items.Add( emoteItem );
            }
        }

        public override bool SelectItem( XivEmote item, out XivEmoteSelected selectedItem ) {
            selectedItem = null;
            var files = new List<Lumina.Data.FileResource>();
            try {
                foreach( var path in item.PapFiles ) {
                    if( Plugin.DataManager.FileExists( path ) ) files.Add( Plugin.DataManager.GetFile( path ) );
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
