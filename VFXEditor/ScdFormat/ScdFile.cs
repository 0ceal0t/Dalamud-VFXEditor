using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VfxEditor.FileManager;
using VfxEditor.ScdFormat.Layout;
using VfxEditor.ScdFormat.Music;
using VfxEditor.ScdFormat.Music.Data;
using VfxEditor.ScdFormat.Sound;
using VfxEditor.ScdFormat.Track;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdFile : FileManagerFile {
        private readonly ScdHeader Header;
        private readonly ScdOffsetsHeader OffsetsHeader;

        public List<ScdAudioEntry> Audio = new();
        public List<ScdLayoutEntry> Layouts = new();
        public List<ScdSoundEntry> Sounds = new();
        public List<ScdTrackEntry> Tracks = new();
        public List<ScdAttributeEntry> Attributes = new();

        public ScdAudioEntrySplitView AudioSplitView;
        public ScdSoundSplitView SoundView;
        public ScdLayoutSplitView LayoutView;
        public ScdTrackSplitView TrackView;
        public ScdSimpleSplitView<ScdAttributeEntry> AttributeView;

        public ScdFile( BinaryReader reader, bool checkOriginal = true ) : base( new( Plugin.ScdManager.GetCopyManager() ) ) {
            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            Header = new( reader );
            OffsetsHeader = new( reader );

            // The acutal sound effect/music data
            foreach( var offset in OffsetsHeader.AudioOffsets.Where( x => x != 0 ) ) {
                var newAudio = new ScdAudioEntry();
                newAudio.Read( reader, offset );
                Audio.Add( newAudio );
            }

            foreach( var offset in OffsetsHeader.LayoutOffsets.Where( x => x != 0 ) ) {
                var newLayout = new ScdLayoutEntry();
                newLayout.Read( reader, offset );
                Layouts.Add( newLayout );
            }

            foreach( var offset in OffsetsHeader.TrackOffsets.Where( x => x != 0 ) ) {
                var newTrack = new ScdTrackEntry();
                newTrack.Read( reader, offset );
                Tracks.Add( newTrack );
            }

            foreach( var offset in OffsetsHeader.AttributeOffsets.Where( x => x != 0 ) ) {
                var newAttribute = new ScdAttributeEntry();
                newAttribute.Read( reader, offset );
                Attributes.Add( newAttribute );
            }

            foreach( var offset in OffsetsHeader.SoundOffsets.Where( x => x != 0 ) ) {
                var newSound = new ScdSoundEntry();
                newSound.Read( reader, offset );
                Sounds.Add( newSound );
            }

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );

            AudioSplitView = new( Audio );
            LayoutView = new( Layouts );
            SoundView = new( Sounds );
            TrackView = new( Tracks );
            AttributeView = new( "Attribute", Attributes );
        }

        public override void Draw( string id ) {
            if( ImGui.BeginTabBar( $"{id}-MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Audio{id}" ) ) {
                    DrawSounds( id );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Sounds{id}" ) ) {
                    SoundView.Draw( $"{id}/Sounds" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Layouts{id}" ) ) {
                    LayoutView.Draw( $"{id}/Layouts" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Tracks{id}" ) ) {
                    TrackView.Draw( $"{id}/Tacks" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Attributes{id}" ) ) {
                    AttributeView.Draw( $"{id}/Attributes" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawSounds( string id ) {
            if( ImGui.CollapsingHeader( "Settings##Audio" ) ) {
                ImGui.Indent();

                ImGui.TextDisabled( "Audio player settings. These do not have any effect on the .scd file" );
                if( ImGui.Checkbox( $"Loop Music{id}", ref Plugin.Configuration.LoopMusic ) ) Plugin.Configuration.Save();
                if( ImGui.Checkbox( $"Loop Sound Effects{id}", ref Plugin.Configuration.LoopSoundEffects ) ) Plugin.Configuration.Save();
                if( ImGui.Checkbox( $"Simulate Loop Start/End{id}", ref Plugin.Configuration.SimulateScdLoop ) ) Plugin.Configuration.Save();
                ImGui.SetNextItemWidth( 50 );
                if( ImGui.InputFloat( $"Volume{id}", ref Plugin.Configuration.ScdVolume ) ) {
                    Plugin.Configuration.Save();
                    Audio.ForEach( x => x.Player.UpdateVolume() );
                }

                ImGui.Unindent();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );

            ImGui.BeginChild( $"{id}-Child" );
            AudioSplitView.Draw( id + "/Audio" );
            ImGui.EndChild();
        }

        public override void Write( BinaryWriter writer ) {
            Header.Write( writer );
            OffsetsHeader.Write( writer );

            UpdateOffsets( writer, Layouts, OffsetsHeader.LayoutOffset, ( BinaryWriter bw, ScdLayoutEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            UpdateOffsets( writer, Sounds, ( int )OffsetsHeader.SoundOffset, ( BinaryWriter bw, ScdSoundEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            UpdateOffsets( writer, Tracks, OffsetsHeader.TrackOffset, ( BinaryWriter bw, ScdTrackEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            UpdateOffsets( writer, Attributes, OffsetsHeader.AttributeOffset, ( BinaryWriter bw, ScdAttributeEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            // Sounds
            long paddingSubtract = 0;
            UpdateOffsets( writer, Audio, OffsetsHeader.AudioOffset, ( BinaryWriter bw, ScdAudioEntry music ) => {
                music.Write( writer, out var padding );
                paddingSubtract += padding;
            } );
            if( ( paddingSubtract % 16 ) > 0 ) paddingSubtract -= paddingSubtract % 16;

            ScdHeader.UpdateFileSize( writer, paddingSubtract ); // end with this
        }

        public void Replace( ScdAudioEntry old, ScdAudioEntry newEntry ) {
            var index = Audio.IndexOf( old );
            if( index == -1 ) return;
            Audio.Remove( old );
            Audio.Insert( index, newEntry );
        }

        public override void Dispose() => Audio.ForEach( x => x.Dispose() );

        public static async void Import( string path, ScdAudioEntry music ) {
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
