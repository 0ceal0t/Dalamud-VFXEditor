using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Actor;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Tmfcs;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public class Tmtr : TmbItemWithTime {
        public override string Magic => "TMTR";
        public override int Size => 0x18;
        public override int ExtraSize => !UseUnknownExtra ? 0 : 8 + ( 12 * LuaEntries.Count );

        public readonly List<TmbEntry> Entries = new();
        private readonly List<int> TempIds;
        public DangerLevel MaxDanger => Entries.Count == 0 ? DangerLevel.None : Entries.Select( x => x.Danger ).Max();

        private bool UseUnknownExtra => UnknownExtraAssigned.Value == true;
        private readonly ParsedByteBool UnknownExtraAssigned = new( "Use Lua", defaultValue: false );
        private readonly List<TmtrLuaEntry> LuaEntries = new();

        private int AllEntriesIdx => Entries.Count == 0 ? 0 : File.AllEntries.IndexOf( Entries.Last() ) + 1;

        private string NewSearchInput = "";

        public Tmtr( TmbFile file ) : base( file ) { }

        public Tmtr( TmbFile file, TmbReader reader ) : base( file, reader ) {
            TempIds = reader.ReadOffsetTimeline();

            reader.ReadAtOffset( ( binaryReader ) => {
                UnknownExtraAssigned.Value = true;

                binaryReader.ReadInt32(); // 8
                var count = binaryReader.ReadInt32();
                for( var i = 0; i < count; i++ ) LuaEntries.Add( new TmtrLuaEntry( binaryReader, File ) );
            } );
        }

        public void PickEntries( TmbReader reader ) => Entries.AddRange( reader.Pick<TmbEntry>( TempIds ) );

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            writer.WriteOffsetTimeline( Entries );
            if( !UseUnknownExtra ) writer.Write( 0 );
            else {
                writer.WriteExtra( ( binaryWriter ) => {
                    binaryWriter.Write( 8 );
                    binaryWriter.Write( LuaEntries.Count );
                    LuaEntries.ForEach( x => x.Write( binaryWriter ) );
                } );
            }
        }

        public void Draw() {
            DrawHeader();
            DrawUnknownData();
            DrawEntries();
        }

        private void DrawUnknownData() {
            UnknownExtraAssigned.Draw( Command );
            if( UseUnknownExtra ) {
                using var _ = ImRaii.PushId( "Lua" );

                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                        Command.Add( new GenericAddCommand<TmtrLuaEntry>( LuaEntries, new TmtrLuaEntry( File ) ) );
                    }
                }

                for( var idx = 0; idx < LuaEntries.Count; idx++ ) {
                    var lua = LuaEntries[idx];

                    if( ImGui.CollapsingHeader( $"Lua Entry {idx}" ) ) {
                        using var __ = ImRaii.PushId( idx );
                        using var indent = ImRaii.PushIndent();

                        if( UiUtils.RemoveButton( "Delete", true ) ) {
                            Command.Add( new GenericRemoveCommand<TmtrLuaEntry>( LuaEntries, lua ) );
                            break;
                        }
                        lua.Draw();
                    }
                }
            }
        }

        private void DrawEntries() {
            for( var idx = 0; idx < Entries.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                if( Entries[idx].Draw( this ) ) break;
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            if( UiUtils.IconButton( FontAwesomeIcon.Plus, "New" ) ) ImGui.OpenPopup( "NewEntryPopup" );

            if( ImGui.BeginPopup( "NewEntryPopup" ) ) {
                DrawNewPopup();
                ImGui.EndPopup();
            }
        }

        public void DuplicateEntry( TmbEntry entry ) => ImportEntry( entry.ToBytes() );

        private void ImportEntry( byte[] data ) {
            using var ms = new MemoryStream( data );
            using var reader = new BinaryReader( ms );
            var tmbReader = new TmbReader( reader );

            var verified = true;
            var actors = new List<Tmac>();
            var tracks = new List<Tmtr>();
            var entries = new List<TmbEntry>();
            var tmfcs = new List<Tmfc>();

            tmbReader.ParseItem( File, actors, tracks, entries, tmfcs, ref verified );
            var newEntry = entries[0];

            TmbRefreshIdsCommand command = new( File, false, true );
            AddEntry( command, newEntry );
            Command.Add( command );
        }

        public void AddEntry( CompoundCommand command, TmbEntry entry ) {
            command.Add( new GenericAddCommand<TmbEntry>( Entries, entry ) );
            command.Add( new GenericAddCommand<TmbEntry>( File.AllEntries, entry, AllEntriesIdx ) );
        }

        public void DeleteEntry( TmbEntry entry ) {
            if( !Entries.Contains( entry ) ) return;
            TmbRefreshIdsCommand command = new( File, false, true );
            DeleteEntry( command, entry );
            Command.Add( command );
        }

        public void DeleteEntry( CompoundCommand command, TmbEntry entry ) {
            if( !Entries.Contains( entry ) ) return;
            command.Add( new GenericRemoveCommand<TmbEntry>( Entries, entry ) );
            command.Add( new GenericRemoveCommand<TmbEntry>( File.AllEntries, entry ) );
        }

        public void DeleteAllEntries( TmbRefreshIdsCommand command ) {
            foreach( var entry in Entries ) {
                command.Add( new GenericRemoveCommand<TmbEntry>( Entries, entry ) );
                command.Add( new GenericRemoveCommand<TmbEntry>( File.AllEntries, entry ) );
            }
        }

        private void DrawNewPopup() {
            using var _ = ImRaii.PushId( "NewEntry" );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.FileImport.ToIconString() ) ) ImportDialog();
            }

            var resetScroll = false;

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) ) ) {
                ImGui.SameLine();
                ImGui.SetNextItemWidth( 200f );
                resetScroll = ImGui.InputTextWithHint( "##Search", "Search", ref NewSearchInput, 255 );
            }

            ImGui.Separator();

            var maxLength = TmbUtils.ItemTypes.Select( x => ImGui.CalcTextSize( $"{x.Key} | {x.Value.DisplayName}" ).X ).Max();
            var imguiStyle = ImGui.GetStyle();

            using var child = ImRaii.Child( "Child", new Vector2( maxLength + imguiStyle.FramePadding.X * 3 + imguiStyle.ScrollbarSize, 800 ) );

            if( resetScroll ) ImGui.SetScrollHereY();

            foreach( var option in TmbUtils.ItemTypes ) {
                if( !string.IsNullOrEmpty( NewSearchInput ) ) {
                    var combinedText = $"{option.Key} {option.Value.DisplayName}".ToLower();
                    if( !combinedText.Contains( NewSearchInput.ToLower() ) ) continue;
                }


                ImGui.TextDisabled( option.Key );
                ImGui.SameLine();

                if( ImGui.Selectable( $"{option.Value.DisplayName}##{option.Key}", false, ImGuiSelectableFlags.SpanAllColumns ) ) AddEntry( option.Value.Type );
            }
        }

        private void AddEntry( Type type ) {
            var constructor = type.GetConstructor( new Type[] { typeof( TmbFile ) } );
            var newEntry = ( TmbEntry )constructor.Invoke( new object[] { File } );

            TmbRefreshIdsCommand command = new( File, false, true );
            AddEntry( command, newEntry );
            Command.Add( command );
        }

        private void ImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", "TMB Entry{.tmbentry},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    ImportEntry( System.IO.File.ReadAllBytes( res ) );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not import data" );
                }
            } );
        }
    }
}
