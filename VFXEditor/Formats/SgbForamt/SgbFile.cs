using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Formats.SgbForamt.Scenes;

namespace VfxEditor.Formats.SgbForamt {
    // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Files/SgbFile.cs#L14
    // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Parsing/Layer/LayerGroup.cs#L12
    // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Parsing/Scene/Scene.cs#L8

    public class SgbFile : FileManagerFile {
        public readonly List<SgbScene> Scenes = new();

        public SgbFile( BinaryReader reader, bool verify ) : base() {
            reader.ReadInt32(); // magic
            reader.ReadInt32(); // total file size

            var chunkCount = reader.ReadInt32();
            for( var i = 0; i < chunkCount; i++ ) { // TODO: find example with multiple chunks
                Scenes.Add( new( this, reader ) );
            }
        }

        public override void Write( BinaryWriter writer ) {

        }

        public override void Draw() {

        }
    }
}
