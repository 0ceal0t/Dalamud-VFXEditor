using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIMain
    {
        public AVFXBase AVFX;

        public UIParameterView ParameterView;
        public UIEffectorView EffectorView;
        public UIEmitterView EmitterView;
        public UIModelView ModelView;
        public UIParticleView ParticleView;
        public UITextureView TextureView;
        public UITimelineView TimelineView;
        public UIScheduleView ScheduleView;
        public UIBinderView BinderView;

        public UIMain(AVFXBase avfx)
        {
            AVFX = avfx;
            // =========================
            ParticleView = new UIParticleView(avfx);
            ParameterView = new UIParameterView(avfx);
            BinderView = new UIBinderView(avfx);
            EmitterView = new UIEmitterView(avfx);
            EffectorView = new UIEffectorView(avfx);
            TimelineView = new UITimelineView(avfx);
            TextureView = new UITextureView(avfx);
            ModelView = new UIModelView(avfx);
            ScheduleView = new UIScheduleView(avfx);
        }

        public void Draw()
        {
            if (ImGui.BeginTabBar("##MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton | ImGuiTabBarFlags.TabListPopupButton))
            {
                if (ImGui.BeginTabItem("Parameters##Main"))
                {
                    ParameterView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Scheduler##Main"))
                {
                    ScheduleView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Timelines##Main"))
                {
                    TimelineView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Emitters##Main"))
                {
                    EmitterView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Particles##Main"))
                {
                    ParticleView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Effectors##Main"))
                {
                    EffectorView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Binders##Main"))
                {
                    BinderView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Textures##Main"))
                {
                    TextureView.Draw();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Models##Main"))
                {
                    ModelView.Draw();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
    }
}
