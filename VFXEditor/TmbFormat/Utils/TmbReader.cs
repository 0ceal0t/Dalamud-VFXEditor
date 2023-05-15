using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.TmbFormat.Actor;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat.Utils {
    public class TmbReader {
        public readonly BinaryReader Reader;

        private long StartPosition;
        private readonly Dictionary<int, TmbItemWithId> ItemsWithId = new();

        public TmbReader( BinaryReader reader ) {
            Reader = reader;
        }

        public void UpdateStartPosition() {
            StartPosition = Reader.BaseStream.Position;
        }

        public void ParseItem( List<Tmac> actors, List<Tmtr> tracks, List<TmbEntry> entries, List<Tmfc> tmfcs, bool papEmbedded, ref bool ok ) {
            var savePos = Reader.BaseStream.Position;
            var magic = ReadString( 4 );
            var size = ReadInt32();
            Reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            TmbItem entry;

            if( magic == "TMAC" ) {
                var actor = new Tmac( this, papEmbedded );
                actors.Add( actor );
                entry = actor;
            }
            else if( magic == "TMTR" ) {
                var track = new Tmtr( this, papEmbedded );
                tracks.Add( track );
                entry = track;
            }
            else if( magic == "TMFC" ) {
                var tmfc = new Tmfc( this, papEmbedded );
                entries.Add( tmfc );
                tmfcs.Add( tmfc );
                entry = tmfc;
            }
            else {
                if( !TmbUtils.ItemTypes.ContainsKey( magic ) ) {
                    PluginLog.Log( $"Unknown Entry {magic}" );
                    ok = false;
                    Reader.ReadBytes( size ); //  skip
                    return;
                }

                var type = TmbUtils.ItemTypes[magic].Type;
                var constructor = type.GetConstructor( new Type[] { typeof( TmbReader ), typeof( bool ) } );
                if( constructor == null ) {
                    PluginLog.Log( $"TmbReader constructor failed for {magic}" );
                    ok = false;
                    Reader.ReadBytes( size );
                    return;
                }

                var item = constructor.Invoke( new object[] { this, papEmbedded } );
                if( item == null ) {
                    PluginLog.Log( $"Constructor failed for {magic}" );
                    ok = false;
                    Reader.ReadBytes( size );
                    return;
                }

                entry = ( TmbItem )item;
                entries.Add( ( TmbEntry )item );
            }

            if( entry is TmbItemWithId entryId ) ItemsWithId.Add( entryId.Id, entryId );
        }

        public List<T> Pick<T>( List<int> ids ) where T : TmbItemWithId {
            var ret = new List<T>();
            foreach( var id in ids ) {
                if( !ItemsWithId.TryGetValue( id, out var item ) ) continue;
                if( item == null ) continue;
                if( item is T pickedItem ) ret.Add( pickedItem );
            }
            return ret;
        }

        public bool ReadAtOffset( Action<BinaryReader> func ) => ReadAtOffset( Reader.ReadInt32(), func );

        public bool ReadAtOffset( int offset, Action<BinaryReader> func ) {
            if( offset == 0 ) return false; // nothing to read
            var savePos = Reader.BaseStream.Position;
            Reader.BaseStream.Seek( StartPosition + 8 + offset, SeekOrigin.Begin );
            func( Reader );
            Reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            return true;
        }

        public string ReadOffsetString() {
            var offset = Reader.ReadInt32();
            var savePos = Reader.BaseStream.Position;
            Reader.BaseStream.Seek( StartPosition + 8 + offset, SeekOrigin.Begin );
            var res = FileUtils.ReadString( Reader );
            Reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            return res;
        }

        public List<int> ReadOffsetTimeline() {
            var offset = Reader.ReadInt32();
            var count = Reader.ReadInt32();
            var savePos = Reader.BaseStream.Position;
            Reader.BaseStream.Seek( StartPosition + 8 + offset, SeekOrigin.Begin );
            var res = new List<int>();
            for( var i = 0; i < count; i++ ) {
                res.Add( Reader.ReadInt16() );
            }
            Reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            return res;
        }

        public Vector3 ReadOffsetVector3() {
            var offset = Reader.ReadInt32();
            var count = Reader.ReadInt32();

            if( count != 3 ) return new Vector3( 0 );

            var currentPos = Reader.BaseStream.Position;
            Reader.BaseStream.Seek( offset + StartPosition + 8, SeekOrigin.Begin );

            var result = new Vector3() {
                X = Reader.ReadSingle(),
                Y = Reader.ReadSingle(),
                Z = Reader.ReadSingle(),
            };

            Reader.BaseStream.Seek( currentPos, SeekOrigin.Begin );
            return result;
        }

        public Vector4 ReadOffsetVector4() {
            var offset = Reader.ReadInt32();
            var count = Reader.ReadInt32();

            if( count != 4 ) return new Vector4( 0 );

            var currentPos = Reader.BaseStream.Position;
            Reader.BaseStream.Seek( offset + StartPosition + 8, SeekOrigin.Begin );

            var result = new Vector4() {
                X = Reader.ReadSingle(),
                Y = Reader.ReadSingle(),
                Z = Reader.ReadSingle(),
                W = Reader.ReadSingle(),
            };

            Reader.BaseStream.Seek( currentPos, SeekOrigin.Begin );
            return result;
        }

        public int ReadInt32() => Reader.ReadInt32();
        public short ReadInt16() => Reader.ReadInt16();
        public byte ReadByte() => Reader.ReadByte();
        public float ReadSingle() => Reader.ReadSingle();
        public string ReadString( int size ) => FileUtils.ReadString( Reader, size );
    }
}
