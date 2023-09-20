using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VfxEditor.Data;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxBase {
        protected readonly string AvfxName;
        protected bool Assigned = true;

        public AvfxBase( string avfxName ) {
            AvfxName = avfxName;
        }

        public bool IsAssigned() => Assigned;

        public void SetAssigned( bool assigned ) {
            Assigned = assigned;
        }

        public string GetAvfxName() => AvfxName;

        // ==== PARSING =====

        public void Read( BinaryReader reader, int size ) {
            Assigned = true;
            RecurseChildrenAssigned( false );
            ReadContents( reader, size );
        }

        protected abstract void RecurseChildrenAssigned( bool assigned );

        public abstract void ReadContents( BinaryReader reader, int size ); // size is the contents size (does not include the name and size of this element)

        public void Write( BinaryWriter writer ) {
            if( !Assigned ) return;

            WriteAvfxName( writer, AvfxName );

            var sizePos = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder

            WriteContents( writer );

            var endPos = writer.BaseStream.Position;
            var size = endPos - sizePos - 4;

            WritePad( writer, CalculatePadding( ( int )size ) );

            endPos = writer.BaseStream.Position;

            writer.BaseStream.Seek( sizePos, SeekOrigin.Begin );
            writer.Write( ( int )size );

            writer.BaseStream.Seek( endPos, SeekOrigin.Begin );
        }

        public abstract void WriteContents( BinaryWriter writer );

        // ===== STATIC PARSING =====

        public static int CalculatePadding( int size ) => size % 4 == 0 ? 0 : 4 - size % 4;

        public static void Peek( BinaryReader reader, AvfxBase item, int size ) => Peek( reader, new List<AvfxBase> { item }, size );

        public static void Peek( BinaryReader reader, List<AvfxBase> items, int size ) {
            var startPosition = reader.BaseStream.Position;
            ReadNested( reader, items, size );
            reader.BaseStream.Seek( startPosition, SeekOrigin.Begin ); // reset position
        }

        public static void WriteNested<T>( BinaryWriter writer, List<T> children ) where T : AvfxBase => children.ForEach( child => child.Write( writer ) );

        public static void ReadNested<T>( BinaryReader reader, List<T> children, int size ) where T : AvfxBase => ReadNested( reader, ( _reader, _name, _size ) => {
            foreach( var child in children ) {
                if( child.GetAvfxName() == _name ) {
                    child.Read( _reader, _size );
                    break;
                }
            }
        }, size );

        public static void RecurseAssigned<T>( T child, bool assigned ) where T : AvfxBase {
            if( child != null ) {
                child.SetAssigned( assigned );
                child.RecurseChildrenAssigned( assigned );
            }
        }

        public static void RecurseAssigned<T>( List<T> children, bool assigned ) where T : AvfxBase {
            foreach( var child in children ) {
                if( child == null ) continue;
                child.SetAssigned( assigned );
                child.RecurseChildrenAssigned( assigned );
            }
        }

        public static void ReadNested( BinaryReader reader, Action<BinaryReader, string, int> onRead, int size ) {
            var bytesRead = 0;
            while( bytesRead < size ) {
                var avfxName = ReadAvfxName( reader );
                bytesRead += 4;

                var contentSize = reader.ReadInt32();
                bytesRead += 4;

                var finalPosition = reader.BaseStream.Position + contentSize;

                onRead( reader, avfxName, contentSize );

                reader.BaseStream.Seek( finalPosition, SeekOrigin.Begin );
                bytesRead += contentSize;

                var padding = CalculatePadding( contentSize );
                reader.ReadBytes( padding );
                bytesRead += padding;
            }
        }

        public static void WriteLeaf( BinaryWriter writer, string avfxName, int size, int value ) {
            WriteAvfxName( writer, avfxName );
            writer.Write( size );
            writer.Write( value );
        }

        public static void WriteAvfxName( BinaryWriter writer, string avfxName ) {
            var nameBytes = Encoding.ASCII.GetBytes( avfxName ).Reverse().ToArray();
            writer.Write( nameBytes );
            WritePad( writer, 4 - avfxName.Length );
        }

        public static string ReadAvfxName( BinaryReader reader ) => ReadAvfxName( BitConverter.GetBytes( reader.ReadInt32() ) );

        public static string ReadAvfxName( byte[] bytes ) {
            var nonZeroBytes = bytes.Reverse().Where( x => x != 0 ).ToArray();
            return Encoding.ASCII.GetString( nonZeroBytes );
        }

        public static void WritePad( BinaryWriter writer, int count ) {
            for( var i = 0; i < count; i++ ) writer.Write( ( byte )0 );
        }

        public static byte[] FloatTo2Bytes( float floatVal ) => BitConverter.GetBytes( Pack( floatVal ) );

        public static float Bytes2ToFloat( byte[] bytes ) => Unpack( bytes, 0 );

        public static unsafe ushort Pack( float value ) {
            var num5 = *( uint* )&value;
            var num3 = ( uint )( ( num5 & -2147483648 ) >> 0x10 );
            var num = num5 & 0x7fffffff;
            if( num > 0x47ffefff ) {
                return ( ushort )( num3 | 0x7fff );
            }
            if( num >= 0x38800000 ) return ( ushort )( num3 | num + -939524096 + 0xfff + ( num >> 13 & 1 ) >> 13 );

            var num6 = num & 0x7fffff | 0x800000;
            var num4 = 0x71 - ( int )( num >> 0x17 );
            num = num4 > 0x1f ? 0 : num6 >> num4;
            return ( ushort )( num3 | num + 0xfff + ( num >> 13 & 1 ) >> 13 );
        }

        public static float Unpack( byte[] buffer, int offset ) => Unpack( BitConverter.ToUInt16( buffer, offset ) );

        public static unsafe float Unpack( ushort value ) {
            uint num3;
            if( ( value & -33792 ) == 0 ) {
                if( ( value & 0x3ff ) != 0 ) {
                    var num2 = 0xfffffff2;
                    var num = ( uint )( value & 0x3ff );
                    while( ( num & 0x400 ) == 0 ) {
                        num2--;
                        num <<= 1;
                    }
                    num &= 0xfffffbff;
                    num3 = ( uint )( ( value & 0x8000 ) << 0x10 | num2 + 0x7f << 0x17 ) | num << 13;
                }
                else {
                    num3 = ( uint )( ( value & 0x8000 ) << 0x10 );
                }
            }
            else {
                num3 =
                    ( uint )
                    ( ( value & 0x8000 ) << 0x10 | ( value >> 10 & 0x1f ) - 15 + 0x7f << 0x17
                     | ( value & 0x3ff ) << 13 );
            }
            return *( float* )&num3;
        }

        // ========= STATIC DRAWING =============

        public static bool DrawUnassignContextMenu( string name ) {
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"Unassign/{name}" );

            using var popup = ImRaii.Popup( $"Unassign/{name}" );
            if( popup ) {
                if( ImGui.Selectable( $"Unassign {name}" ) ) {
                    ImGui.CloseCurrentPopup();
                    return true;
                }
            }

            return false;
        }

        public static bool DrawAddButton<T>( T assignable, string name ) where T : AvfxBase {
            if( !assignable.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {name}" ) ) CommandManager.Avfx.Add( new AvfxAssignCommand( assignable, true ) );
                return true;
            }
            return false;
        }

        public static void DrawRemoveContextMenu<T>( T assignable, string name ) where T : AvfxBase {
            if( DrawUnassignContextMenu( name ) ) CommandManager.Avfx.Add( new AvfxAssignCommand( assignable, false ) );
        }

        public static bool DrawAddButton<T>( List<T> assignable, string name ) where T : AvfxBase {
            if( !assignable[0].IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {name}" ) ) CommandManager.Avfx.Add( new AvfxAssignCommandToggle<T>( assignable, true ) );
                return true;
            }
            return false;
        }

        public static void DrawRemoveContextMenu<T>( List<T> assignable, string name ) where T : AvfxBase {
            if( DrawUnassignContextMenu( name ) ) CommandManager.Avfx.Add( new AvfxAssignCommandToggle<T>( assignable, false ) );
        }

        public static bool DrawAddButtonRecurse<T>( T assignable, string name ) where T : AvfxBase {
            if( !assignable.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {name}" ) ) CommandManager.Avfx.Add( new AvfxAssignCommandMulti( assignable, true ) );
                return true;
            }
            return false;
        }

        public static void DrawRemoveContextMenuRecurse<T>( T assignable, string name ) where T : AvfxBase {
            if( DrawUnassignContextMenu( name ) ) CommandManager.Avfx.Add( new AvfxAssignCommandMulti( assignable, false ) );
        }

        public static bool DrawRemoveButton<T>( T assignable, string name ) where T : AvfxBase {
            if( UiUtils.RemoveButton( $"Delete {name}", small: true ) ) {
                CommandManager.Avfx.Add( new AvfxAssignCommand( assignable, false ) );
                return true;
            }
            return false;
        }

        public static void AssignedCopyPaste<T>( T assignable, string name ) where T : AvfxBase {
            var copyManager = CopyManager.Avfx;
            if( copyManager.IsCopying ) copyManager.Assigned[name] = assignable.IsAssigned();
            if( copyManager.IsPasting ) {
                if( copyManager.Assigned.TryGetValue( name, out var a ) ) copyManager.PasteCommand.Add( new AvfxAssignCommand( assignable, a ) );
            }
        }

        public static void DrawNamedItems<T>( List<T> items ) where T : INamedUiItem {
            SplitUnassigned( items, out var assigned, out var unassigned );

            if( unassigned.Count > 0 ) {
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                for( var idx = 0; idx < unassigned.Count; idx++ ) {
                    if( idx > 0 ) ImGui.SameLine();
                    unassigned[idx].Draw();
                }
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            }

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            foreach( var item in assigned ) {
                using var tabBarItem = ImRaii.TabItem( $"{item.GetText()}" );
                if( tabBarItem ) item.Draw();
            }
        }

        public static void DrawItems<T>( List<T> items ) where T : IUiItem {
            SplitUnassigned( items, out var assigned, out var unassigned );

            assigned.ForEach( x => x.Draw() );
            unassigned.ForEach( x => x.Draw() );
        }

        public static void SplitUnassigned<T>( List<T> items, out List<T> assigned, out List<T> unassigned ) {
            assigned = [];
            unassigned = [];
            foreach( var item in items ) {
                if( item is AvfxOptional optionalItem && !optionalItem.IsAssigned() ) unassigned.Add( item );
                else assigned.Add( item );
            }
        }
    }
}
