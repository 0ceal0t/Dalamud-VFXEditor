using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.FileBrowser;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Actor;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Tmfcs;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public class Tmtr : TmbItemWithTime, IUiItem {
        public override string Magic => "TMTR";
        public override int Size => 0x18;
        public override int ExtraSize => !LuaAssigned.Value ? 0 : 8 + ( 12 * LuaEntries.Count );

        public readonly List<TmbEntry> Entries = [];
        private readonly List<int> TempIds;
        public DangerLevel MaxDanger => Entries.Count == 0 ? DangerLevel.None : Entries.Select( x => x.Danger ).Max();

        private TmtrLuaEntry DraggingItem;
        private readonly ParsedByteBool LuaAssigned = new( "Use Lua Condition", value: false );
        public readonly List<TmtrLuaEntry> LuaEntries = [];

        private int AllEntriesIdx => Entries.Count == 0 ? 0 : File.AllEntries.IndexOf( Entries.Last() ) + 1;

        private string NewSearchInput = "";

        public Tmtr( TmbFile file ) : base( file ) { }

        public Tmtr( TmbFile file, TmbReader reader ) : base( file, reader ) {
            TempIds = reader.ReadOffsetTimeline();

            reader.ReadAtOffset( ( binaryReader ) => {
                LuaAssigned.Value = true;

                binaryReader.ReadInt32(); // 8
                var count = binaryReader.ReadInt32();
                for( var i = 0; i < count; i++ ) LuaEntries.Add( new TmtrLuaEntry( binaryReader, File, this ) );
            } );
        }

        public void PickEntries( TmbReader reader ) => Entries.AddRange( reader.Pick<TmbEntry>( TempIds ) );

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            writer.WriteOffsetTimeline( Entries );
            if( !LuaAssigned.Value ) writer.Write( 0 );
            else {
                writer.WriteExtra( ( binaryWriter ) => {
                    binaryWriter.Write( 8 );
                    binaryWriter.Write( LuaEntries.Count );
                    LuaEntries.ForEach( x => x.Write( binaryWriter ) );
                } );
            }
        }

        public void Draw() {
            DrawLua();
            DrawHeader();
            DrawEntries();
        }

        private void DrawLua() {
            LuaAssigned.Draw();
            ImGui.SameLine();
            UiUtils.HelpMarker( "The current value of Lua variables can be found in the \"Lua Variables\" tab of File > Tools" );

            if( !LuaAssigned.Value ) return;

            using var _ = ImRaii.PushId( "Lua" );
            using( var indent = ImRaii.PushIndent( 5f ) ) {
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

                var currentX = 0f;
                var maxX = ImGui.GetContentRegionAvail().X;
                foreach( var (lua, idx) in LuaEntries.WithIndex() ) {
                    using var __ = ImRaii.PushId( idx );

                    if( lua.Draw( idx == 0, maxX, ref currentX ) ) break;
                    if( UiUtils.DrawDragDrop( LuaEntries, lua, lua.Text, ref DraggingItem, "LUA", true ) ) break;
                }

                if( UiUtils.IconButton( FontAwesomeIcon.Plus, "New" ) ) {
                    CommandManager.Add( new ListAddCommand<TmtrLuaEntry>( LuaEntries, new TmtrLuaEntry( File, this ) ) );
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
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

            var verified = VerifiedStatus.OK;
            var actors = new List<Tmac>();
            var tracks = new List<Tmtr>();
            var entries = new List<TmbEntry>();
            var tmfcs = new List<Tmfc>();

            tmbReader.ParseItem( File, actors, tracks, entries, tmfcs, ref verified );
            var newEntry = entries[0];

            var commands = new List<ICommand>();
            AddEntry( commands, newEntry );
            CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
        }

        public void AddEntry( List<ICommand> commands, TmbEntry entry ) {
            commands.Add( new ListAddCommand<TmbEntry>( Entries, entry ) );
            commands.Add( new ListAddCommand<TmbEntry>( File.AllEntries, entry, AllEntriesIdx ) );
        }

        public void DeleteEntry( TmbEntry entry ) {
            if( !Entries.Contains( entry ) ) return;
            var commands = new List<ICommand>();
            DeleteEntry( commands, entry );
            CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
        }

        public void DeleteEntry( List<ICommand> commands, TmbEntry entry ) {
            if( !Entries.Contains( entry ) ) return;
            commands.Add( new ListRemoveCommand<TmbEntry>( Entries, entry ) );
            commands.Add( new ListRemoveCommand<TmbEntry>( File.AllEntries, entry ) );
        }

        public void DeleteAllEntries( List<ICommand> commands ) {
            foreach( var entry in Entries ) {
                commands.Add( new ListRemoveCommand<TmbEntry>( Entries, entry ) );
                commands.Add( new ListRemoveCommand<TmbEntry>( File.AllEntries, entry ) );
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
            var constructor = type.GetConstructor( [typeof( TmbFile )] );
            var newEntry = ( TmbEntry )constructor.Invoke( [File] );

            var commands = new List<ICommand>();
            AddEntry( commands, newEntry );
            CommandManager.Add( new CompoundCommand( commands, File.RefreshIds ) );
        }

        private void ImportDialog() {
            FileBrowserManager.OpenFileDialog( "Select a File", "TMB Entry{.tmbentry},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    ImportEntry( System.IO.File.ReadAllBytes( res ) );
                }
                catch( Exception e ) {
                    Dalamud.Error( e, "Could not import data" );
                }
            } );
        }
    }
}
