using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using VFXEditor.Helper;

namespace VFXEditor.Pap {
    public class PapFile {
        private readonly string HkxTempLocation;

        private short ModelId;
        private byte BaseId;
        private byte VariantId;
        private readonly List<PapAnimation> Animations = new();

        private PapAnimation SelectedAnimation = null;

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
                if( numAnimations > 0 && i < ( numAnimations - 1 ) ) reader.ReadBytes( Padding( reader.BaseStream.Position ) );
            }
        }

        public byte[] ToBytes() {
            var havokData = File.ReadAllBytes( HkxTempLocation );
            var tmbData = Animations.Select( x => x.GetTmbBytes() );

            var tmbSize = 0;
            var idx = 0;
            foreach( var tmb in tmbData ) {
                tmbSize += tmb.Length;
                if( tmbData.Count() > 1 && idx < tmbData.Count() - 1 ) {
                    tmbSize += Padding( tmbSize );
                }
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

            foreach( var anim in Animations ) {
                anim.Write( writer );
            }

            writer.Write( havokData );

            idx = 0;
            foreach( var tmb in tmbData ) {
                writer.Write( tmb );
                if( tmbData.Count() > 1 && idx < tmbData.Count() - 1 ) {
                    var padding = Padding( writer.BaseStream.Position );
                    for( var j = 0; j < padding; j++ ) writer.Write( ( byte )0 );
                }
                idx++;
            }

            return data;
        }

        public void Draw( string id ) {
            FileHelper.ShortInput( $"Model Id{id}", ref ModelId );
            FileHelper.ByteInput( $"Base Id{id}", ref BaseId );
            FileHelper.ByteInput( $"Variant Id{id}", ref VariantId );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            var selectedIndex = SelectedAnimation == null ? -1 : Animations.IndexOf( SelectedAnimation );
            if( ImGui.BeginCombo( $"{id}-ActorSelect", SelectedAnimation == null ? "[NONE]" : SelectedAnimation.GetName() ) ) {
                for( var i = 0; i < Animations.Count; i++ ) {
                    var animation = Animations[i];
                    if( ImGui.Selectable( $"{animation.GetName()}{id}{i}", animation == SelectedAnimation ) ) {
                        SelectedAnimation = animation;
                        selectedIndex = i;
                    }
                }
                ImGui.EndCombo();
            }

            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SameLine();
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) {
                Animations.Add( new PapAnimation() );
            }
            if( SelectedAnimation != null ) {
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 3 );
                if( UiHelper.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    Animations.Remove( SelectedAnimation );
                    SelectedAnimation = null;
                }
            }
            ImGui.PopFont();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            if( SelectedAnimation != null ) {
                SelectedAnimation.Draw( $"{id}{selectedIndex}" );
            }
            else {
                ImGui.Text( "Select an animation..." );
            }
        }

        private int Padding( long position ) {
            var leftOver = position % 4;
            return ( int )( leftOver == 0 ? 0 : 4 - leftOver );
        }
    }
}
