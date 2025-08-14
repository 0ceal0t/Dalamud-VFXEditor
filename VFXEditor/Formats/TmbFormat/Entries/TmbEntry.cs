using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.FileBrowser;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public abstract class TmbEntry : TmbItemWithTime {
        public abstract string DisplayName { get; }
        public virtual DangerLevel Danger => DangerLevel.None;

        private readonly List<ParsedBase> Parsed;

        public TmbEntry( TmbFile file ) : base( file ) {
            Parsed = GetParsed();
        }

        public TmbEntry( TmbFile file, TmbReader reader ) : base( file, reader ) {
            Parsed = GetParsed();
            foreach( var item in Parsed ) item.Read( reader );
        }

        public bool Draw( Tmtr track ) {
            var isColored = DoColor( Danger, out var col );

            using var color = ImRaii.PushColor( ImGuiCol.Header, col, isColored );
            color.Push( ImGuiCol.HeaderHovered, col * new Vector4( 0.75f, 0.75f, 0.75f, 1f ), isColored );
            color.Push( ImGuiCol.HeaderActive, col * new Vector4( 0.75f, 0.75f, 0.75f, 1f ), isColored );

            if( ImGui.CollapsingHeader( string.IsNullOrEmpty( DisplayName ) ? Magic : $"{DisplayName} ({Magic})" ) ) {
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
            foreach( var item in Parsed ) item.Draw();
        }

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            foreach( var item in Parsed ) item.Write( writer );
        }

        protected abstract List<ParsedBase> GetParsed();

        public byte[] ToBytes() {
            var tmbWriter = new TmbWriter( Size, ExtraSize, 0 );
            tmbWriter.StartPosition = tmbWriter.Position;
            Write( tmbWriter );

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );
            tmbWriter.WriteTo( writer );
            tmbWriter.Dispose();
            return ms.ToArray();
        }

        private void SaveDialog() =>
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".tmbentry,.*", "ExportedTmbEntry", "tmbentry", ( bool ok, string res ) => {
                if( ok ) System.IO.File.WriteAllBytes( res, ToBytes() );
            } );

        public static bool DoColor( DangerLevel level, out Vector4 color ) {
            color = new( 1 );
            if( level < DangerLevel.Yellow ) return false;
            else if( level == DangerLevel.Yellow ) color = UiUtils.DALAMUD_ORANGE;
            else color = UiUtils.RED_COLOR;

            return true;
        }
    }
}
