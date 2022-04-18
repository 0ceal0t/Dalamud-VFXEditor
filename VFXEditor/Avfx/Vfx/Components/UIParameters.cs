using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public class UIParameters : UIItem {
        public string Name;
        private readonly List<UIBase> Parameters;

        public UIParameters( string name ) {
            Name = name;
            Parameters = new List<UIBase>();
        }

        public void Add( UIBase item ) {
            Parameters.Add( item );
        }

        public void Remove( UIBase item ) {
            Parameters.Remove( item );
        }

        public void Prepend( UIBase item ) {
            Parameters.Insert( 0, item );
        }

        public override void DrawBody( string id ) {
            DrawList( Parameters, id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => true;
    }
}
