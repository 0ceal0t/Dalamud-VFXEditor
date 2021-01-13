using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VFXEditor.UI.VFX
{
    public abstract class UIDropdownView<T> : UIBase where T : UIItem {
        public string id;
        public string defaultText;

        public List<T> Items;
        public T Selected;

        public bool AllowNew;
        public bool AllowDelete;

        public UIDropdownView(string _id, string _defaultText, bool allowNew = true, bool allowDelete = true )
        {
            id = _id;
            defaultText = _defaultText;
            AllowNew = allowNew;
            AllowDelete = allowDelete;
            // ===============
            Selected = null;
            Items = new List<T>();
        }

        public abstract T OnNew();
        public abstract void OnDelete( T item );
        public abstract byte[] OnExport( T item );
        public abstract T OnImport( AVFXLib.AVFX.AVFXNode node );

        public override void Draw( string parentId = "" )
        {
            ViewSelect();
            if( AllowNew )
            {
                ImGui.SameLine();
                if( ImGui.SmallButton( "+ New" + id ) )
                {
                    ImGui.OpenPopup( "New_Popup" + id );
                }
                if( ImGui.BeginPopup( "New_Popup" + id ) )
                {
                    if( ImGui.Selectable( "Create" + id ) )
                    {
                        AddItem( OnNew() );
                    }
                    if( ImGui.Selectable("Load" + id ) )
                    {
                        Load();
                    }
                    ImGui.EndPopup();
                }
            }
            if( Selected != null && AllowDelete )
            {
                ImGui.SameLine();
                if( ImGui.SmallButton( "Save" + id ) )
                {
                    Save( Selected );
                }
                ImGui.SameLine();
                if( UIUtils.RemoveButton( "Delete" + id, small:true) )
                {
                    OnDelete( Selected );
                    Items.Remove( Selected );
                    // TODO: fix IDX
                    Selected = null;
                }
            }
            ImGui.Separator();
            // ====================
            if( Selected != null )
            {
                Selected.DrawBody( id );
            }
        }
        public void AddItem(T item ) {
            Items.Add( item );
            // TODO: fix IDX
        }
        // ========================
        public void ViewSelect() {
            var selectedString = (Selected != null) ? Selected.GetText(Items.IndexOf(Selected)) : defaultText;
            if( ImGui.BeginCombo( "Select" + id, selectedString ) ) {
                for( int idx = 0; idx < Items.Count; idx++ ) {
                    var item = Items[idx];
                    if( ImGui.Selectable( item.GetText(idx) + id, Selected == item ) ) {
                        Selected = item;
                    }
                }
                ImGui.EndCombo();
            }
        }
        // ========= DIALOGS ==================
        public void Load()
        {
            Task.Run( async () => {
                var picker = new OpenFileDialog
                {
                    Filter = "Partial AVFX (*.vfxedit)|*.vfxedit*|All files (*.*)|*.*",
                    Title = "Select a File Location.",
                    CheckFileExists = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        var node = AVFXLib.Main.Reader.readAVFX( picker.FileName, out List<string> messages );
                        foreach( string message in messages ) {
                            PluginLog.Log( message );
                        }
                        AddItem(OnImport( node ));
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a file" );
                    }
                }
            } );
        }

        public void Save( T item)
        {
            Task.Run( async () => {
                var picker = new SaveFileDialog
                {
                    Filter = "Partial AVFX (*.vfxedit)|*.vfxedit*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "vfxedit",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        var data = OnExport( item );
                        File.WriteAllBytes( picker.FileName, data );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a file" );
                    }
                }
            } );
        }
    }
}
