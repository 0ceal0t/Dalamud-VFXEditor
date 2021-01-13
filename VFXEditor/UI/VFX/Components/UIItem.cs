namespace VFXEditor.UI.VFX {
    public abstract class UIItem : UIBase {
        public int Idx;
        public abstract void DrawBody( string parentId );
        public abstract void DrawSelect(string parentId, ref UIItem selected );
        public abstract string GetText();
        public override void Draw( string parentId ) { }

        public UIItem() {
        }
    }
}
