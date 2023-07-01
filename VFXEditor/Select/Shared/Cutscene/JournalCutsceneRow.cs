using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace VfxEditor.Select.Shared.Cutscene {
    public class JournalCutsceneRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly List<string> Paths = new();

        public JournalCutsceneRow( CompleteJournal journal ) {
            RowId = ( int )journal.RowId;
            Name = journal.Name.ToString();

            foreach( var cutscene in journal.Cutscene ) {
                var path = cutscene.Value?.Path.ToString();
                if( !string.IsNullOrEmpty( path ) ) Paths.Add( $"cut/{path}.cutb" );
            }
        }
    }
}
