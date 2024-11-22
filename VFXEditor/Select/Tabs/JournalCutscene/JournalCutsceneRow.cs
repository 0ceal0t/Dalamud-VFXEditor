using Lumina.Excel.Sheets;
using System.Collections.Generic;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.JournalCutscene {
    public class JournalCutsceneRow : ISelectItem {
        public readonly string Name;
        public readonly int RowId;
        public readonly List<string> Paths = [];

        public JournalCutsceneRow( CompleteJournal journal ) {
            RowId = ( int )journal.RowId;
            Name = journal.Name.ToString();

            foreach( var cutscene in journal.Cutscene ) {
                var path = cutscene.ValueNullable?.Path.ToString();
                if( !string.IsNullOrEmpty( path ) ) Paths.Add( $"cut/{path}.cutb" );
            }
        }

        public string GetName() => Name;
    }
}
