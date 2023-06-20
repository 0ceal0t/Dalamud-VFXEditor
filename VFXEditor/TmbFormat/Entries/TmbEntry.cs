using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum DangerLevel : int {
        None,
        Yellow,
        Red,
        Detectable,
        DontAddRemove
    }

    public abstract class TmbEntry : TmbItemWithTime {
        public abstract string DisplayName { get; }
        public virtual DangerLevel Danger => DangerLevel.None;

        private readonly List<ParsedBase> Parsed;

        public TmbEntry( TmbFile file ) : base( file ) {
            Parsed = GetParsed();
        }

        public TmbEntry( TmbFile file, TmbReader reader ) : base( file, reader ) {
            Parsed = GetParsed();
        }

        public bool Draw( Tmtr track ) {
            var isColored = DoColor( Danger, out var col );

            using var color = ImRaii.PushColor( ImGuiCol.Header, col, isColored );
            color.Push( ImGuiCol.HeaderHovered, col * new Vector4( 0.75f, 0.75f, 0.75f, 1f ), isColored );
            color.Push( ImGuiCol.HeaderActive, col * new Vector4( 0.75f, 0.75f, 0.75f, 1f ), isColored );

            if( ImGui.CollapsingHeader( DisplayName ) ) {
                if( isColored ) color.Pop( 2 );

                DragDrop();

                using var indent = ImRaii.PushIndent();

                using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) ) )
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                        track.DeleteEntry( this );
                        return true;
                    }

                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.Save.ToIconString() ) ) {
                        SaveDialog();
                    }

                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.Copy.ToIconString() ) ) {
                        track.DuplicateEntry( this );
                        return true;
                    }
                }

                DrawBody();

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            }
            else DragDrop();

            return false;
        }

        private void DragDrop() {
            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                File.StartDragging( this );
                ImGui.Text( DisplayName );
                ImGui.EndDragDropSource();
            }
        }

        public virtual void DrawBody() {
            DrawHeader();
            DrawParsed();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );
        }

        protected void ReadParsed( TmbReader reader ) {
            foreach( var item in Parsed ) item.Read( reader );
        }

        protected void WriteParsed( TmbWriter writer ) {
            foreach( var item in Parsed ) item.Write( writer );
        }

        protected void DrawParsed() {
            foreach( var item in Parsed ) item.Draw( Command );
        }

        protected abstract List<ParsedBase> GetParsed();

        public byte[] ToBytes() {
            var tmbWriter = new TmbWriter( Size, ExtraSize, sizeof( short ) );
            tmbWriter.StartPosition = tmbWriter.Writer.BaseStream.Position;
            Write( tmbWriter );

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );

            tmbWriter.WriteTo( writer );
            tmbWriter.Dispose();

            return ms.ToArray();
        }

        private void SaveDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".tmbentry,.*", "ExportedTmbEntry", "tmbentry", ( bool ok, string res ) => {
                if( ok ) System.IO.File.WriteAllBytes( res, ToBytes() );
            } );
        }

        public static bool DoColor( DangerLevel level, out Vector4 color ) {
            color = new( 1 );
            if( level < DangerLevel.Yellow ) return false;
            else if( level == DangerLevel.Yellow ) color = UiUtils.YELLOW_COLOR;
            else color = UiUtils.RED_COLOR;

            return true;
        }
    }
}
