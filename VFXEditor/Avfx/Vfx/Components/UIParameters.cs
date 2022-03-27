using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public class UIParameters : UIItem {
        public string Name;
        private readonly List<UIBase> Parameters;

        public UIParameters( string name ) {
            Assigned = true;
            Name = name;
            Parameters = new List<UIBase>();
        }

        public void Add( UIBase item ) {
            Parameters.Add( item );
        }

        public override void DrawBody( string id ) {
            DrawList( Parameters, id );
        }

        public override string GetDefaultText() => Name;
    }
}
