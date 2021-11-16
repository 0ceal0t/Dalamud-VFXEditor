using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VFXEditor.FileManager;
using VFXEditor.Helper;

namespace VFXEditor.Pap {
    public class PapFile : FileDropdown<PapAnimation> {
        private readonly string HkxTempLocation;

        private short ModelId;
        private byte BaseId;
        private byte VariantId;
        private readonly List<PapAnimation> Animations = new();

        public PapFile( BinaryReader reader, string hkxTemp ) {
            HkxTempLocation = hkxTemp;

            reader.ReadInt32(); // magic
            reader.ReadInt32(); // version
            var numAnimations = reader.ReadInt16();
            ModelId = reader.ReadInt16();
            BaseId = reader.ReadByte();
            VariantId = reader.ReadByte();

            reader.ReadInt32(); // info offset
            var havokPosition = reader.ReadInt32(); // from beginning
            var footerPosition = reader.ReadInt32();

            for( var i = 0; i < numAnimations; i++ ) {
                Animations.Add( new PapAnimation( reader ) );
            }

            // ... do something about havok data ...
            var havokDataSize = footerPosition - havokPosition;
            reader.BaseStream.Seek( havokPosition, SeekOrigin.Begin );
            var havokData = reader.ReadBytes( havokDataSize );
            File.WriteAllBytes( HkxTempLocation, havokData );

            reader.BaseStream.Seek( footerPosition, SeekOrigin.Begin );
            for( var i = 0; i < numAnimations; i++ ) {
                Animations[i].ReadTmb( reader );
                reader.ReadBytes( Padding( reader.BaseStream.Position, i, numAnimations ) );
            }
        }

        public byte[] ToBytes() {
            var havokData = File.ReadAllBytes( HkxTempLocation );
            var tmbData = Animations.Select( x => x.GetTmbBytes() );

            var tmbSize = 0;
            var idx = 0;
            foreach( var tmb in tmbData ) {
                tmbSize += tmb.Length;
                tmbSize += Padding( tmbSize, idx, tmbData.Count() );
                idx++;
            }

            var havokSize = havokData.Length;
            var headerSize = 0x1A;
            var infoSize = Animations.Count * 0x28;
            var totalSize = headerSize + infoSize + havokSize + tmbSize;

            var data = new byte[totalSize];
            using MemoryStream dataMs = new( data );
            using BinaryWriter writer = new( dataMs );
            writer.BaseStream.Seek( 0, SeekOrigin.Begin );

            // ====================

            writer.Write( 0x70617020 );
            writer.Write( 0x01000200 );
            writer.Write( ( short )Animations.Count );
            writer.Write( ModelId );
            writer.Write( BaseId );
            writer.Write( VariantId );

            writer.Write( headerSize );
            writer.Write( headerSize + infoSize );
            writer.Write( headerSize + infoSize + havokSize );

            foreach( var anim in Animations ) anim.Write( writer );

            writer.Write( havokData );

            idx = 0;
            foreach( var tmb in tmbData ) {
                writer.Write( tmb );

                var padding = Padding( writer.BaseStream.Position, idx, tmbData.Count() );
                for( var j = 0; j < padding; j++ ) writer.Write( ( byte )0 );

                idx++;
            }

            return data;
        }

        public void Draw( string id ) {
            FileHelper.ShortInput( $"Model Id{id}", ref ModelId );
            FileHelper.ByteInput( $"Base Id{id}", ref BaseId );
            FileHelper.ByteInput( $"Variant Id{id}", ref VariantId );

            DrawDropDown( id );

            if( Selected != null ) {
                Selected.Draw( $"{id}{Animations.IndexOf(Selected)}" );
            }
            else {
                ImGui.Text( "Select an animation..." );
            }
        }

        private static int Padding( long position, int idx, int max ) {
            if( max > 1 && idx < max - 1 ) {
                var leftOver = position % 4;
                return ( int )( leftOver == 0 ? 0 : 4 - leftOver );
            }
            return 0;
        }

        protected override List<PapAnimation> GetOptions() => Animations;

        protected override string GetName( PapAnimation item, int idx ) => item.GetName();

        protected override void OnNew() => Animations.Add( new PapAnimation() );

        protected override void OnDelete( PapAnimation item ) => Animations.Remove( item );
    }
}
