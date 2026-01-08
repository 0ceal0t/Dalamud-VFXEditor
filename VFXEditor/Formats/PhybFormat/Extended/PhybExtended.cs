using FlatSharp;
using System;
using System.IO;
using VfxEditor.Flatbuffer;
using VfxEditor.Formats.PhybFormat.Extended.Ephb;
using VfxEditor.PhybFormat;
using VfxEditor.Utils.PackStruct;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybExtended {
        public readonly PhybEphbData Table;

        public PhybExtended() { }

        public PhybExtended( Pack extendedData ) : this() {
            Table = new( EphbData.Serializer.Parse( extendedData.Data.ToArray(), FlatBufferDeserializationOption.GreedyMutable ) );
        }

        public Span<byte> GetEphbData() {
            var table = Table.Export();

            var size = EphbData.Serializer.GetMaxSize( table );
            var data = new byte[size];
            EphbData.Serializer.WithSettings( s => s.DisableSharedStrings() );
            var bytes = EphbData.Serializer.Write( data, table );
            return data.AsSpan( 0, bytes );
        }

        public void Write( BinaryWriter writer ) {
            Pack.Write( writer, PhybFile.ExtendedType, 1, GetEphbData() );
        }

        public void Draw() => Table.Draw();
    }
}
