using ClickableTransparentOverlay;
using Fiddler;
using ImGuiNET;
using System.Numerics;

namespace HexedProxy.Core
{
    internal class GUI(string windowsName, bool DPIAware) : Overlay(windowsName, DPIAware)
    {
        private void AddStyles()
        {
            ImGuiStylePtr style = ImGui.GetStyle();

            style.FrameRounding = 4.0f;
            style.WindowBorderSize = 2.0f;
            style.PopupBorderSize = 0.0f;
            style.GrabRounding = 4.0f;

            var colors = style.Colors;

            colors[(int)ImGuiCol.Text] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.73f, 0.75f, 0.74f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.09f, 0.09f, 0.09f, 0.87f);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.34f, 0, 0.6f, 0.50f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.34f, 0, 0.6f, 0.54f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.54f, 0, 0.9f, 0.40f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.34f, 0, 0.6f, 0.67f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.34f, 0, 0.6f, 0.67f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.34f, 0, 0.6f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.34f, 0, 0.6f, 0.67f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.34f, 0.16f, 0.16f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.02f, 0.02f, 0.02f, 0.53f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.34f, 0, 0.6f, 1.00f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.54f, 0, 0.9f, 1.00f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.34f, 0, 0.6f, 0.65f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.54f, 0, 0.9f, 0.65f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.34f, 0, 0.6f, 0.50f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.34f, 0, 0.6f, 0.54f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.54f, 0, 0.9f, 0.65f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.34f, 0, 0.6f, 0.00f);
            colors[(int)ImGuiCol.Separator] = new Vector4(0.34f, 0, 0.6f, 0.50f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.54f, 0, 0.9f, 0.54f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.34f, 0, 0.6f, 0.54f);
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.34f, 0, 0.6f, 0.54f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.54f, 0, 0.9f, 0.66f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.34f, 0, 0.6f, 0.66f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.34f, 0, 0.6f, 0.54f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.54f, 0, 0.9f, 0.66f);
            colors[(int)ImGuiCol.TabActive] = new Vector4(0.34f, 0, 0.6f, 0.66f);
            colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.07f, 0.10f, 0.15f, 0.97f);
            colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.14f, 0.26f, 0.42f, 1.00f);
            colors[(int)ImGuiCol.PlotLines] = new Vector4(0.34f, 0, 0.6f, 1.00f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.54f, 0, 0.9f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.34f, 0, 0.6f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.54f, 0, 0.9f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.26f, 0.59f, 0.98f, 0.35f);
            colors[(int)ImGuiCol.DragDropTarget] = new Vector4(1.00f, 1.00f, 0.00f, 0.90f);
            colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);
        }

        protected override void Render()
        {
            AddStyles();

            ImGui.SetNextWindowSize(new Vector2(700, 400), ImGuiCond.Appearing);
            ImGui.SetNextWindowPos(new Vector2(30, 30), ImGuiCond.Appearing);

            bool open = true;
            ImGui.Begin("H E X E D | U N L O C K E R", ref open, ImGuiWindowFlags.NoResize);

            ImGui.BeginChild("Categories", new Vector2(150, 0));

            if (ImGui.Selectable("GENERAL", InternalSettings.SelectedGuiCategory == 0)) InternalSettings.SelectedGuiCategory = 0;
            if (ImGui.Selectable("TOOLS", InternalSettings.SelectedGuiCategory == 1)) InternalSettings.SelectedGuiCategory = 1;
            if (ImGui.Selectable("INFO", InternalSettings.SelectedGuiCategory == 2)) InternalSettings.SelectedGuiCategory = 2;

            ImGui.EndChild();

            ImGui.SameLine();

            ImGui.BeginChild("Options");
            switch (InternalSettings.SelectedGuiCategory)
            {
                case 0: // GENERAL
                    if (ImGui.Button("Start")) ProxyManager.Connect();
                    ImGui.SameLine();
                    if (ImGui.Button("Stop")) ProxyManager.Disconnect();
                    ImGui.SameLine();
                    if (ImGui.Button("Exit"))
                    {
                        ProxyManager.Disconnect();
                        Close();
                    }
                    ImGui.Text($"Proxy is {(FiddlerApplication.IsStarted() ? "RUNNING" : "NOT RUNNING")}");
                    break;

                case 1: // TOOLS
                    ImGui.Checkbox("Cosmetic Unlock", ref InternalSettings.UnlockCosmetics);
                    ImGui.Checkbox("Item Unlock", ref InternalSettings.UnlockItems);
                    ImGui.Checkbox("Level Unlock", ref InternalSettings.UnlockLevel);

                    ImGui.Checkbox("Spoof Offline", ref InternalSettings.SpoofOffline);

                    ImGui.Checkbox("Change Rank", ref InternalSettings.SpoofRank);
                    if (InternalSettings.SpoofRank)
                    {
                        ImGui.SameLine(0, 10f);
                        ImGui.SliderInt("", ref InternalSettings.TargetRank, 1, 20);
                    }

                    ImGui.Checkbox("Change Region", ref InternalSettings.SpoofRegion);
                    if (InternalSettings.SpoofRegion) 
                    {
                        ImGui.SameLine(0, 10f);
                        ImGui.ListBox("", ref InternalSettings.TargetQueueRegion, InternalSettings.AvailableRegions, InternalSettings.AvailableRegions.Length);
                    }

                    if (ImGui.Button("Add Friend")) Task.Run(() => RequestSender.SendFriendRequest(InternalSettings.AddFriendId));
                    ImGui.SameLine(0, 10f);
                    ImGui.InputTextMultiline("UserID", ref InternalSettings.AddFriendId, 100, new Vector2(200, 20));

                    break;

                case 2: // INFO
                    ImGui.Text($"Player: {InternalSettings.PlayerName}");
                    ImGui.Text($"Killer: {InternalSettings.KillerId}");
                    ImGui.Text($"MatchId: {InternalSettings.MatchId}");
                    ImGui.Text($"Region: {InternalSettings.MatchRegion}");
                    break;
            }
            ImGui.EndChild();

            ImGui.End();
        }
    }
}
