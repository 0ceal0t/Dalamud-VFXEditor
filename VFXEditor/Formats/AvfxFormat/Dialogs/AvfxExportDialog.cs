using Dalamud.Interface.Windowing;
using VfxEditor.Ui;

namespace VfxEditor.Formats.AvfxFormat.Dialogs {
    public class AvfxExportDialog : DalamudWindow {
        public AvfxExportDialog( WindowSystem windowSystem ) : base( "Export", false, new( 600, 400 ), windowSystem ) { }

        public override bool DrawConditions() => Plugin.AvfxManager?.File != null;

        public override void DrawBody() => Plugin.AvfxManager?.File?.ExportUi.Draw();
    }
}
