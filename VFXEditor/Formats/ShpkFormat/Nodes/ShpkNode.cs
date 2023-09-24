using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Nodes {
    public class ShpkNode : IUiItem {
        private readonly List<ShpkPass> Passes = new();

        public ShpkNode() { }

        public ShpkNode( BinaryReader reader, int systemKeyCount, int sceneKeyCount, int materialKeyCount, int subViewKeyCount ) {
            var selector = reader.ReadUInt32();
            var passCount = reader.ReadUInt32();
            var passIndexes = reader.ReadBytes( 16 );

            var systemKeys = new List<uint>();
            var sceneKeys = new List<uint>();
            var materialKeys = new List<uint>();
            var subViewKeys = new List<uint>();

            for( var i = 0; i < systemKeyCount; i++ ) systemKeys.Add( reader.ReadUInt32() );
            for( var i = 0; i < sceneKeyCount; i++ ) sceneKeys.Add( reader.ReadUInt32() );
            for( var i = 0; i < materialKeyCount; i++ ) materialKeys.Add( reader.ReadUInt32() );
            for( var i = 0; i < subViewKeyCount; i++ ) subViewKeys.Add( reader.ReadUInt32() );

            for( var i = 0; i < passCount; i++ ) Passes.Add( new( reader ) );
        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw() {

        }
    }
}
