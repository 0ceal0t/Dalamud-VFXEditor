using FlatSharp;
using System;
using System.IO;
using VfxEditor.Flatbuffer;
using VfxEditor.Utils.PackStruct;

namespace VfxEditor.Formats.PbdFormat.Extended {
    public class PbdExtended {
        public readonly PbdEpbdData Table;

        public PbdExtended() { }

        public PbdExtended( Pack extendedData ) : this() {
            Table = new( ExtendedPbd.Serializer.Parse( extendedData.Data.ToArray(), FlatBufferDeserializationOption.GreedyMutable ) );
        }

        public Span<byte> GetEpbdData() {
            var table = Table.Export();

            var size = ExtendedPbd.Serializer.GetMaxSize( table );
            var data = new byte[size];
            ExtendedPbd.Serializer.WithSettings( s => s.DisableSharedStrings() );
            var bytes = ExtendedPbd.Serializer.Write( data, table );
            return data.AsSpan( 0, bytes );
        }

        public void Write( BinaryWriter writer ) {
            Pack.Write( writer, PbdFile.ExtendedType, 1, GetEpbdData() );
        }

        public void Draw() => Table.Draw();
    }
}
