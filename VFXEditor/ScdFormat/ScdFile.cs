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

        public List<ScdAudioEntry> Audio = new();
        public List<ScdLayoutEntry> Layouts = new();
        public ScdSimpleSplitView<ScdLayoutEntry> LayoutView;
        public List<ScdSoundEntry> Sound = new();

        public ScdFile( BinaryReader reader, bool checkOriginal = true ) {
            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            Header = new( reader );
            OffsetsHeader = new( reader );

            // The acutal sound effect/music data
            foreach( var offset in OffsetsHeader.AudioOffsets.Where( x => x != 0 ) ) {
                Audio.Add( new ScdAudioEntry( reader, offset ) );
            }

            foreach( var offset in OffsetsHeader.LayoutOffsets.Where( x => x != 0 ) ) {
                Layouts.Add( new ScdLayoutEntry( reader, offset ) );
            }
            LayoutView = new( Layouts );

            foreach( var offset in OffsetsHeader.SoundOffsets.Where( x => x != 0 ) ) {
                Sound.Add( new ScdSoundEntry( reader, offset ) );
            }

            reader.BaseStream.Seek( OffsetsHeader.TrackOffsets[0], SeekOrigin.Begin );
            PreSoundData = reader.ReadBytes( OffsetsHeader.AudioOffsets[0] - OffsetsHeader.TrackOffsets[0] );

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
        }

        public override void Draw( string id ) {
            if( ImGui.BeginTabBar( $"{id}-MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Sounds{id}" ) ) {
                    DrawSounds( id );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Table 2{id}" ) ) {
                    LayoutView.Draw( $"{id}/Table2" );
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
            for( var idx = 0; idx < Audio.Count; idx++ ) {
                Audio[idx].Draw( id + idx, idx );
            }
            ImGui.EndChild();
        }

        public override void Write( BinaryWriter writer ) {
            Header.Write( writer );
            OffsetsHeader.Write( writer );

            UpdateOffsets( writer, Layouts, OffsetsHeader.LayoutOffset, ( BinaryWriter bw, ScdLayoutEntry item ) => {
                item.Write( writer );
            } );

            UpdateOffsets( writer, Sound, ( int )OffsetsHeader.SoundOffset, ( BinaryWriter bw, ScdSoundEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            writer.Write( PreSoundData ); // Everything else

            // Sounds
            long paddingSubtract = 0;
            UpdateOffsets( writer, Audio, OffsetsHeader.AudioOffset, ( BinaryWriter bw, ScdAudioEntry music ) => {
                music.Write( writer, out var padding );
                paddingSubtract += padding;
            } );
            if( ( paddingSubtract % 16 ) > 0 ) paddingSubtract -=  paddingSubtract % 16 ;

            ScdHeader.UpdateFileSize( writer, paddingSubtract ); // end with this
        }

        public void Replace( ScdAudioEntry old, ScdAudioEntry newEntry ) {
            var index = Audio.IndexOf( old );
            if( index == -1 ) return;
            Audio.Remove( old );
            Audio.Insert( index, newEntry );
        }

        public override void Dispose() => Audio.ForEach( x => x.Dispose() );

        public async static void Import( string path, ScdAudioEntry music ) {
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
