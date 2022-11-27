using Dalamud.Logging;
using ImGuiNET;
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdFile : FileManagerFile {
        public readonly CommandManager Command = new( Data.CopyManager.Scd );

        private readonly ScdHeader Header;
        private readonly ScdOffsetsHeader OffsetsHeader;
        private readonly byte[] PreSoundData;

        public List<ScdSoundEntry> Music = new();
        public List<ScdTable2Entry> Table2 = new();
        public ScdSimpleSplitView<ScdTable2Entry> Table2View;

        public ScdFile( BinaryReader reader, bool checkOriginal = true ) {
            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            Header = new( reader );
            OffsetsHeader = new( reader );

            // Sounds
            foreach( var offset in OffsetsHeader.OffsetListSound.Where( x => x != 0 ) ) {
                Music.Add( new ScdSoundEntry( reader, offset ) );
            }

            // Table 2
            foreach( var offset in OffsetsHeader.OffsetList2.Where( x => x != 0 ) ) {
                Table2.Add( new ScdTable2Entry( reader, offset ) );
            }
            Table2View = new( Table2 );

            reader.BaseStream.Seek( OffsetsHeader.StartOffsetList[0], SeekOrigin.Begin );
            PreSoundData = reader.ReadBytes( ( int )( OffsetsHeader.OffsetListSound[0] - OffsetsHeader.StartOffsetList[0] ) );

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
        }

        public override void Draw( string id ) {
            if( ImGui.BeginTabBar( $"{id}-MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Sounds{id}" ) ) {
                    DrawSounds( id );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Table 2{id}" ) ) {
                    Table2View.Draw( $"{id}/Table2" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawSounds( string id ) {
            if( ImGui.Checkbox( $"Loop Music{id}", ref Plugin.Configuration.LoopMusic ) ) Plugin.Configuration.Save();
            if( ImGui.Checkbox( $"Loop Sound Effects{id}", ref Plugin.Configuration.LoopSoundEffects ) ) Plugin.Configuration.Save();
            ImGui.Separator();
            ImGui.BeginChild( $"{id}-Child" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            for( var idx = 0; idx < Music.Count; idx++ ) {
                Music[idx].Draw( id + idx, idx );
            }
            ImGui.EndChild();
        }

        public override void Write( BinaryWriter writer ) {
            Header.Write( writer );
            OffsetsHeader.Write( writer );

            // Table2
            UpdateOffsets( writer, Table2, OffsetsHeader.Offset2, ( BinaryWriter bw, ScdTable2Entry item ) => {
                item.Write( writer );
            } );

            writer.Write( PreSoundData ); // Everything else

            // Sounds
            long paddingSubtract = 0;
            UpdateOffsets( writer, Music, OffsetsHeader.OffsetSound, ( BinaryWriter bw, ScdSoundEntry music ) => {
                music.Write( writer, out var padding );
                paddingSubtract += padding;
            } );
            if( ( paddingSubtract % 16 ) > 0 ) paddingSubtract -=  paddingSubtract % 16 ;

            ScdHeader.UpdateFileSize( writer, paddingSubtract ); // end with this
        }

        public void Replace( ScdSoundEntry old, ScdSoundEntry newEntry ) {
            var index = Music.IndexOf( old );
            if( index == -1 ) return;
            Music.Remove( old );
            Music.Insert( index, newEntry );
        }

        public override void Dispose() => Music.ForEach( x => x.Dispose() );

        public async static void Import( string path, ScdSoundEntry music ) {
            await Task.Run( () => {
                if( music.Format == SscfWaveFormat.Vorbis ) {
                    var ext = Path.GetExtension( path );
                    if( ext == ".wav" ) ScdVorbis.ImportWav( path, music );
                    else ScdVorbis.ImportOgg( path, music );
                }
                else ScdAdpcm.Import( path, music );
            } );
        }

        private static void UpdateOffsets<T>( BinaryWriter writer, List<T> items, int offsetLocation, Action<BinaryWriter, T> action ) where T : ScdEntry {
            List<int> positions = new();
            foreach( var item in items ) {
                positions.Add( ( int )writer.BaseStream.Position );
                action.Invoke( writer, item );
            }
            var savePos = writer.BaseStream.Position;

            writer.BaseStream.Seek( offsetLocation, SeekOrigin.Begin );
            foreach( var position in positions ) {
                writer.Write( position );
            }

            writer.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }
    }
}
