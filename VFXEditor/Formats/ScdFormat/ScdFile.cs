using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VfxEditor.FileManager;
using VfxEditor.ScdFormat.Music;
using VfxEditor.ScdFormat.Music.Data;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdFile : FileManagerFile {
        private readonly ScdHeader Header;
        private readonly ScdOffsetsHeader OffsetsHeader;

        public readonly List<ScdAudioEntry> Audio = new();
        public readonly List<ScdSoundEntry> Sounds = new();
        public readonly List<ScdTrackEntry> Tracks = new();
        public readonly List<ScdAttributeEntry> Attributes = new();

        public readonly ScdAudioEntrySplitView AudioSplitView;
        public readonly CommandSplitView<ScdSoundEntry> SoundView;
        public readonly CommandSplitView<ScdTrackEntry> TrackView;
        public readonly UiSplitView<ScdAttributeEntry> AttributeView;

        public ScdFile( BinaryReader reader, bool verify ) : base() {
            Header = new( reader );
            OffsetsHeader = new( reader );

            // The acutal sound effect/music data
            foreach( var offset in OffsetsHeader.AudioOffsets.Where( x => x != 0 ) ) {
                var newAudio = new ScdAudioEntry( this );
                newAudio.Read( reader, offset );
                Audio.Add( newAudio );
            }

            var layouts = new List<ScdLayoutEntry>();
            foreach( var offset in OffsetsHeader.LayoutOffsets.Where( x => x != 0 ) ) {
                var newLayout = new ScdLayoutEntry();
                newLayout.Read( reader, offset );
                layouts.Add( newLayout );
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

            foreach( var (offset, index) in OffsetsHeader.SoundOffsets.Where( x => x != 0 ).WithIndex() ) {
                var newSound = new ScdSoundEntry( layouts[index] );
                newSound.Read( reader, offset );
                Sounds.Add( newSound );
            }

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes(), null );
            if( OffsetsHeader.Modded ) Verified = VerifiedStatus.UNSUPPORTED;

            AudioSplitView = new( Audio );
            SoundView = new( "Sound", Sounds, true, null, () => new ScdSoundEntry() );
            TrackView = new( "Track", Tracks, false, null, () => new ScdTrackEntry() );
            AttributeView = new( "Attribute", Attributes, false );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawAudio();
            DrawSounds();
            DrawTracks();
            DrawAttributes();
        }

        private void DrawAudio() {
            using var tabItem = ImRaii.TabItem( "Audio" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Audio" );

            if( ImGui.CollapsingHeader( "Settings" ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                ImGui.TextDisabled( "Audio player settings. These do not have any effect on the .scd file" );
                if( ImGui.Checkbox( "Loop Music", ref Plugin.Configuration.LoopMusic ) ) Plugin.Configuration.Save();
                if( ImGui.Checkbox( "Loop Sound Effects", ref Plugin.Configuration.LoopSoundEffects ) ) Plugin.Configuration.Save();
                if( ImGui.Checkbox( "Simulate Loop Start/End", ref Plugin.Configuration.SimulateScdLoop ) ) Plugin.Configuration.Save();
                ImGui.SetNextItemWidth( 50 );
                if( ImGui.InputFloat( "Volume", ref Plugin.Configuration.ScdVolume ) ) {
                    Plugin.Configuration.Save();
                    Audio.ForEach( x => x.Player.UpdateVolume() );
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );

            AudioSplitView.Draw();
        }

        private void DrawSounds() {
            using var tabItem = ImRaii.TabItem( "Sounds" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Sounds" );
            SoundView.Draw();
        }

        private void DrawTracks() {
            using var tabItem = ImRaii.TabItem( "Tracks" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Tracks" );
            TrackView.Draw();
        }

        private void DrawAttributes() {
            using var tabItem = ImRaii.TabItem( "Attributes" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Attributes" );
            AttributeView.Draw();
        }

        public override void Write( BinaryWriter writer ) {
            Header.Write( writer );
            OffsetsHeader.Write( writer );

            UpdateOffsets( writer, Sounds.Select( x => x.Layout ).ToList(), OffsetsHeader.LayoutOffset, ( BinaryWriter bw, ScdLayoutEntry item ) => {
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
                if( music.Format == SscfWaveFormat.Vorbis ) { // .ogg
                    if( Path.GetExtension( path ) == ".wav" ) ScdVorbis.ImportWav( path, music );
                    else ScdVorbis.ImportOgg( path, music );
                }
                else { // .wav
                    if( Path.GetExtension( path ) == ".wav" ) ScdAdpcm.ImportWav( path, music );
                    else ScdVorbis.ImportOgg( path, music ); // kind of jank, but hopefully works
                }
            } );
        }

        private static void UpdateOffsets<T>( BinaryWriter writer, List<T> items, int offsetLocation, Action<BinaryWriter, T> action ) where T : ScdEntry {
            List<int> positions = new();
            foreach( var item in items ) {
                positions.Add( ( int )writer.BaseStream.Position );
                action.Invoke( writer, item );
            }
            var savePos = writer.BaseStream.Position;

            writer.BaseStream.Position = offsetLocation;
            foreach( var position in positions ) {
                writer.Write( position );
            }

            writer.BaseStream.Position = savePos;
        }
    }
}
