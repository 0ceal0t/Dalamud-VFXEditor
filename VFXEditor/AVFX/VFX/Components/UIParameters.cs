using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public class UIParameters : UIItem {
        public string Name;
        private readonly List<IUIBase> Parameters;

        public UIParameters( string name ) {
            Name = name;
            Parameters = new List<IUIBase>();
        }

        public void Add( IUIBase item ) {
            Parameters.Add( item );
        }

        public void Remove( IUIBase item ) {
            Parameters.Remove( item );
        }

        public void Prepend( IUIBase item ) {
            Parameters.Insert( 0, item );
        }

        public override void DrawInline( string id ) {
            IUIBase.DrawList( Parameters, id );
        }

        public override string GetDefaultText() => Name;
    }
}
