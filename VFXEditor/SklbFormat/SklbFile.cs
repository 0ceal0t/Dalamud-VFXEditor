using Dalamud.Logging;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public class SklbFile : FileManagerFile {
        public readonly string HkxTempLocation;

        private readonly short Version1;
        private readonly short Version2;

        public SklbFile( BinaryReader reader, string hkxTemp, bool checkOriginal = true ) : base( new( Plugin.SklbManager ) ) {
            HkxTempLocation = hkxTemp;

            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            reader.ReadInt32(); // Magic
            Version1 = reader.ReadInt16();
            Version2 = reader.ReadInt16();
            var havokOffset = -1;
            var skeletonOffset = -1;

            if( Version2 == 0x3133 ) { // New
                skeletonOffset = reader.ReadInt32();
                havokOffset = reader.ReadInt32();
            }
            else if( Version2 == 0x3132 ) { // Old
                skeletonOffset = reader.ReadInt16();
                havokOffset = reader.ReadInt16();
            }
            else {
                PluginLog.Error( $"Invalid SKLB version: {Version1:X8} {Version2:X8}" );
            }

            /*
             * NEW:
             * 
             * 4: Unk1 [ 2 + 2 ]
             * 4: Skeleton [ 2 + 2 ]
             * 4 x 4 : Parent Skeleton [ 2 + 2 ]
             * 4: Unk2
             * 4: Unk3
             * 
             * ----- offsets from here -----
             * alph: MAGIC
             * count: 2
             * offsets [count]: 2 each
             *  id: 4
             *  count2: 2
             *  values [count2]: 2 each
             *  
             *  
             *  
             *  pad to 16
             */

            reader.BaseStream.Seek( skeletonOffset, SeekOrigin.Begin );

            reader.BaseStream.Seek( havokOffset, SeekOrigin.Begin );
            var havokData = reader.ReadBytes( ( int )( reader.BaseStream.Length - havokOffset ) );
            File.WriteAllBytes( HkxTempLocation, havokData );

            // TODO: read havok

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
        }

        public override void Write( BinaryWriter writer ) {

        }

        public override void Draw() {

        }

        public override void Dispose() {

        }
    }
}
