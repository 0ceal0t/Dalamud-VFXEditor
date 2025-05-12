using Dalamud.Interface.Utility.Raii;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxOptional : AvfxItem {
        public readonly bool Locked;

        public AvfxOptional( string avfxName, bool locked = false ) : base( avfxName ) {
            Locked = locked;
        }

        public override void SetAssigned( bool assigned, bool recurse = false ) {
            base.SetAssigned( assigned, recurse );
            if( recurse && assigned ) { // never recursively unassign
                foreach( var child in GetChildren() ) child?.SetAssigned( assigned, recurse );
            }
        }

        public abstract void DrawBody();

        public override void Draw() {
            if( IsAssigned() ) {
                using var _ = ImRaii.PushId( GetDefaultText() );
                AssignedCopyPaste( GetDefaultText() );
                if( !Locked && DrawUnassignButton( GetDefaultText() ) ) return;

                DrawBody();
            }
            else {
                using var _ = ImRaii.PushId( GetDefaultText() );
                AssignedCopyPaste( GetDefaultText() );
                DrawAssignButton( GetDefaultText(), true );
            }
        }
    }
}
