namespace VFXEditor.UI.VFX {
    public abstract class UIItem : UIBase {
        public int Idx;
        public abstract void DrawBody( string parentId );
        public abstract void DrawSelect( int idx, string parentId, ref UIItem selected );
        public abstract string GetText(int idx);
        public override void Draw( string parentId ) { }

        public UIItem() {
        }
    }
}
