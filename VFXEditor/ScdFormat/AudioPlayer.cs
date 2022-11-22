using ImGuiNET;
using NAudio.Wave;
using System;
using Dalamud.Logging;
using Dalamud.Interface;
using ImGuiFileDialog;

namespace VfxEditor.ScdFormat {
    public class AudioPlayer {
        private readonly ScdSoundEntry Entry;
        private PlaybackState State => CurrentOutput == null ? PlaybackState.Stopped : CurrentOutput.PlaybackState;

        private WaveStream CurrentStream;
        private WaveChannel32 CurrentChannel;
        private WasapiOut CurrentOutput;

        private double TotalTime => CurrentStream == null ? 0 : CurrentStream.TotalTime.TotalSeconds;
        private double CurrentTime => CurrentStream == null ? 0 : CurrentStream.CurrentTime.TotalSeconds;

        public AudioPlayer( ScdSoundEntry entry ) {
            Entry = entry;
        }

        public void Draw( string id ) {
            // Controls
            ImGui.PushFont( UiBuilder.IconFont );
            if( State == PlaybackState.Stopped ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Play}" + id ) ) {
                    Reset();
                    try {
                        var stream = Entry.Data.GetStream();
                        CurrentStream = stream.WaveFormat.Encoding switch {
                            WaveFormatEncoding.Pcm => WaveFormatConversionStream.CreatePcmStream( stream ),
                            WaveFormatEncoding.Adpcm => WaveFormatConversionStream.CreatePcmStream( stream ),
                            _ => stream
                        };

                        CurrentChannel = new WaveChannel32( CurrentStream ) {
                            Volume = 1f,
                            PadWithZeroes = false,
                        };
                        CurrentOutput = new WasapiOut();

                        CurrentOutput.Init( CurrentChannel );
                        CurrentOutput.Play();
                    }
                    catch( Exception e ) {
                        PluginLog.LogError( e, "Error playing sound" );
                    }
                }
            }
            else if( State == PlaybackState.Playing ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Pause}" + id ) ) CurrentOutput.Pause();
            }
            else if( State == PlaybackState.Paused ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Play}" + id ) ) CurrentOutput.Play();
            }

            ImGui.PopFont();

            if( State == PlaybackState.Stopped ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            var selectedTime = ( float )CurrentTime;
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 150f );
            if( ImGui.SliderFloat( $"{id}-Drag", ref selectedTime, 0, ( float )TotalTime) ) {
                if( State != PlaybackState.Stopped ) {
                    CurrentOutput.Pause();
                    CurrentStream.CurrentTime = TimeSpan.FromSeconds( selectedTime );
                }
            }

            if( State == PlaybackState.Stopped ) ImGui.PopStyleVar();

            // Save
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SameLine();
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Download}" + id ) ) {
                FileDialogManager.SaveFileDialog( "Select a Save Location", ".wav", "ExportedSound", "wav", ( bool ok, string res ) => {
                    if( ok ) {
                        using var stream = Entry.Data.GetStream();
                        WaveFileWriter.CreateWaveFile( res, stream );
                    }
                } );
            }
            // Import
            ImGui.SameLine();
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Upload}" + id ) ) {
                var text = Entry.Format == SscfWaveFormat.Vorbis ? "Audio files{.ogg,.wav},.*" : "Audio files{.wav},.*";
                FileDialogManager.OpenFileDialog( "Import File", text, ( bool ok, string res ) => {
                    if( ok ) ScdFile.Import( res, Entry );
                } );
            }
            ImGui.PopFont();

            ImGui.Indent();
            var frequency = Entry.Frequency;
            var loopStartSeconds = frequency == 0 ? 0 : Entry.LoopStart / frequency;
            var loopEndSeconds = frequency == 0 ? 0 : Entry.LoopEnd / frequency;

            ImGui.TextDisabled( $"{Entry.Format} / {Entry.NumChannels} Channels / Loop [{loopStartSeconds}, {loopEndSeconds}] seconds" );
            ImGui.Unindent();
        }

        public void Reset() {
            CurrentOutput?.Dispose();
            CurrentChannel?.Dispose();
            CurrentStream?.Dispose();
        }

        public void Dispose() {
            CurrentOutput?.Stop();
            Reset();
        }
    }
}
