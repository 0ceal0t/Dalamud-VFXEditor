using ImGuiNET;
using System.IO;
using VFXEditor.AVFXLib;

namespace VFXEditor.AVFX.VFX {
    public abstract class UINodeDropdownView<T> : UIBase, IUINodeView<T> where T : UINode {
        public readonly AVFXMain AVFX;
        public readonly AVFXFile VfxFile;
        public readonly UINodeGroup<T> Group;

        private readonly string Id;
        private readonly string DefaultText;
        private readonly string DefaultPath;
        private readonly bool AllowNew;
        private readonly bool AllowDelete;

        private T Selected = null;

        public UINodeDropdownView( AVFXFile vfxFile, AVFXMain avfx, UINodeGroup<T> group, string name, bool allowNew, bool allowDelete, string defaultPath ) {
            VfxFile = vfxFile;
            AVFX = avfx;
            Group = group;
            Id = $"##{name}";

            var isVowel = "aeiouAEIOU".Contains( name[0] );
            var aOrAn = isVowel ? "an" : "a";
            DefaultText = $"Select {aOrAn} {name}";

            AllowNew = allowNew;
            AllowDelete = allowDelete;
            DefaultPath = Path.Combine( Plugin.RootLocation, "Files", defaultPath );
        }

        public abstract void OnDelete( T item );
        public abstract void OnExport( BinaryWriter writer, T item );
        public virtual void OnSelect( T item ) { }
        public abstract T OnImport( BinaryReader reader, int size, bool has_dependencies = false );

        public void AddToGroup( T item ) => Group.Add( item );

        public override void Draw( string parentId = "" ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ViewSelect();

            if( AllowNew ) ImGui.SameLine();
            IUINodeView<T>.DrawControls( this, VfxFile, Selected, Group, AllowNew, AllowDelete, Id );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );

            if( Selected != null ) {
                Selected.DrawBody( Id );
            }
        }

        public void ViewSelect() {
            var selectedString = ( Selected != null ) ? Selected.GetText() : DefaultText;
            if( ImGui.BeginCombo( Id + "-Select", selectedString ) ) {
                for(var idx = 0; idx < Group.Items.Count; idx++) {
                    var item = Group.Items[idx];
                    if( ImGui.Selectable( $"{item.GetText()}{Id}{idx}", Selected == item ) ) {
                        Selected = item;
                        OnSelect( item );
                    }
                }
                ImGui.EndCombo();
            }
        }

        public void DeleteSelected() {
            Selected = null;
        }

        public void CreateDefault() => VfxFile.Import( DefaultPath );
    }
}
