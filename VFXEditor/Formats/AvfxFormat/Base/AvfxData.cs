using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Assign;
using VfxEditor.Parsing.Data;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxData : AvfxItem, IData {
        protected List<AvfxBase>? Parsed;

        public readonly bool Optional;
        public readonly List<AvfxItem> Tabs = [];

        private readonly AvfxDisplaySplitView<AvfxItem> SplitView;

        public AvfxData( bool optional = false ) : base( "Data" ) {
            Optional = optional;
            SplitView = new AvfxDisplaySplitView<AvfxItem>( "Data", Tabs );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed!, size );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed! );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed! ) yield return item;
        }

        public override string GetDefaultText() => "Data";

        public override void Draw() {
            if( Tabs.Count == 1 ) {
                using var child = ImRaii.Child( "Child" );
                Tabs[0].Draw();
            }
            else SplitView.Draw();
        }

        public virtual void Enable() { }

        public virtual void Disable() { }

        public void DrawEnableCheckbox() {
            if( !Optional ) return;
            var assigned = Assigned;
            if( ImGui.Checkbox( "Enable Data", ref assigned ) ) {
                CommandManager.Add( new AvfxAssignCommand( this, assigned ) );
            }
        }
    }
}
