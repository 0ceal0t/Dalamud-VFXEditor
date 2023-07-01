using VfxEditor.FileManager;
using VfxEditor.Select.Cutb.Cutscene;
using VfxEditor.Select.Cutb.JournalCutscene;

namespace VfxEditor.Select.Cutb {
    public class CutbSelectDialog : SelectDialog {
        public CutbSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "cutb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new CutsceneTab( this, "Cutscene" ),
                new JournalCutsceneTab( this, "Journal Cutscene" )
            } );
        }
    }
}
