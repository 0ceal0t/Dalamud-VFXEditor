using Dalamud.Logging;
using Lumina.Data;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class JournalCutsceneSheetLoader : SheetLoader<XivJournalCutscene, XivJournalCutsceneSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<CompleteJournal>().Where( x => !string.IsNullOrEmpty( x.Name ) );

            foreach( var item in sheet ) Items.Add( new XivJournalCutscene( item ) );
        }

        public override bool SelectItem( XivJournalCutscene item, out XivJournalCutsceneSelected selectedItem ) {
            selectedItem = null;
            var files = new List<FileResource>();
            try {
                foreach( var path in item.Paths ) {
                    if( Plugin.DataManager.FileExists( path ) ) files.Add( Plugin.DataManager.GetFile( path ) );
                }
                selectedItem = new XivJournalCutsceneSelected( item, files );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error reading cutscene file" );
                return false;
            }
            return true;
        }
    }
}
