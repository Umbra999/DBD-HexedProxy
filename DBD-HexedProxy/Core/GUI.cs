using ClickableTransparentOverlay;
using Fiddler;
using ImGuiNET;
using System.Numerics;

namespace HexedProxy.Core
{
    internal class GUI(string windowsName, bool DPIAware) : Overlay(windowsName, DPIAware)
    {
        protected override void Render()
        {
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
                        ImGui.SliderInt("Rank", ref InternalSettings.TargetRank, 1, 20);
                    }

                    ImGui.Checkbox("Change Region", ref InternalSettings.SpoofRegion);
                    if (InternalSettings.SpoofRegion) 
                    {
                        ImGui.SameLine(0, 10f);
                        ImGui.Combo("Region", ref InternalSettings.TargetQueueRegion, InternalSettings.AvailableRegions, InternalSettings.AvailableRegions.Length);
                    }

                    if (ImGui.Button("Add Friend")) Task.Run(() => RequestSender.SendFriendRequest(InternalSettings.AddFriendId));
                    ImGui.SameLine(0, 10f);
                    ImGui.InputTextWithHint("", "PlayerId", ref InternalSettings.AddFriendId, 36);

                    break;

                case 2: // INFO
                    ImGui.Text($"Player: {InternalSettings.PlayerName}");

                    ImGui.Text($"PlayerId: {InternalSettings.PlayerId}");
                    ImGui.SameLine();
                    if (ImGui.Button("Copy PlayerId")) WindowsClipboard.SetText(InternalSettings.PlayerId);

                    ImGui.Text($"KillerId: {InternalSettings.KillerId}");
                    ImGui.SameLine();
                    if (ImGui.Button("Copy KillerId")) WindowsClipboard.SetText(InternalSettings.KillerId);

                    ImGui.Text($"MatchId: {InternalSettings.MatchId}");
                    ImGui.SameLine();
                    if (ImGui.Button("Copy MatchId")) WindowsClipboard.SetText(InternalSettings.MatchId);

                    ImGui.Text($"Region: {InternalSettings.MatchRegion}");
                    break;
            }
            ImGui.EndChild();

            ImGui.End();
        }
    }
}
