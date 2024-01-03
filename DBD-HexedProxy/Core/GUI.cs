using ClickableTransparentOverlay;
using Fiddler;
using HexedProxy.Modules;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace HexedProxy.Core
{
    internal class GUI(string windowsName, bool DPIAware) : Overlay(windowsName, DPIAware)
    {
        protected override void Render()
        {
            ImGui.SetNextWindowSize(new Vector2(750, 400), ImGuiCond.Appearing);
            ImGui.SetNextWindowPos(new Vector2(30, 30), ImGuiCond.Appearing);

            bool open = true;
            ImGui.Begin("H E X E D | U N L O C K E R", ref open, ImGuiWindowFlags.NoResize);

            ImGui.BeginChild("Categories", new Vector2(150, 0));

            if (ImGui.Selectable("GENERAL", InternalSettings.SelectedGuiCategory == 0)) InternalSettings.SelectedGuiCategory = 0;
            if (ImGui.Selectable("TOOLS", InternalSettings.SelectedGuiCategory == 1)) InternalSettings.SelectedGuiCategory = 1;
            if (ImGui.Selectable("TEMP MARKET", InternalSettings.SelectedGuiCategory == 2)) InternalSettings.SelectedGuiCategory = 2;
            if (ImGui.Selectable("PERM UNLOCK", InternalSettings.SelectedGuiCategory == 3)) InternalSettings.SelectedGuiCategory = 3;
            if (ImGui.Selectable("INFO", InternalSettings.SelectedGuiCategory == 4)) InternalSettings.SelectedGuiCategory = 4;

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
                    ImGui.Checkbox("Instant Tomes", ref InternalSettings.InstantTomes);
                    ImGui.Checkbox("Block Tomes", ref InternalSettings.BlockTomes);
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

                    ImGui.Checkbox("Match Snipe", ref InternalSettings.MatchSnipe);
                    if (InternalSettings.MatchSnipe)
                    {
                        ImGui.SameLine(0, 10f);
                        ImGui.Checkbox("Streamer", ref InternalSettings.OnlyStreamer);
                        if (!InternalSettings.OnlyStreamer)
                        {
                            ImGui.SameLine(0, 10f);
                            ImGui.InputTextWithHint("ID/Name", "PlayerId/SteamId/Name", ref InternalSettings.TargetSnipeParameter, 36);
                        }
                    }

                    ImGui.Checkbox("Name Spoof", ref InternalSettings.NameSpoof);
                    if (InternalSettings.NameSpoof)
                    {
                        ImGui.SameLine(0, 10f);
                        ImGui.InputTextWithHint("Custom Name", "Nickname", ref InternalSettings.TargetCustomName, 36);
                    }

                    if (ImGui.Button("Add Friend")) Task.Run(() => RequestSender.AddFriend(InternalSettings.TargetFriendId));
                    ImGui.SameLine(0, 10f);
                    if (ImGui.Button("Remove Friend")) Task.Run(() => RequestSender.RemoveFriend(InternalSettings.TargetFriendId));
                    ImGui.SameLine(0, 10f);
                    ImGui.InputTextWithHint("Friend", "PlayerId", ref InternalSettings.TargetFriendId, 36);

                    break;

                case 2:
                    ImGui.Checkbox("Unlock All", ref InternalSettings.UnlockAll);
                    ImGui.Checkbox("Currency Unlock", ref InternalSettings.UnlockCurrencies);
                    break;

                case 3: // UNLOCK
                    if (ImGui.Button("Finish Tutorial")) Misc.UnlockTutorials();

                    ImGui.Dummy(new Vector2(0, 20));

                    ImGui.Text($"Character: {BloodwebManager.GetSelectedCharacter()}");
                    if (ImGui.Button("Add Prestige")) BloodwebManager.AddPrestigeLevels();
                    ImGui.SameLine(0, 10f);
                    ImGui.SliderInt("Prestige", ref BloodwebManager.TargetPrestige, BloodwebManager.GetCurrentPrestige() == 100 ? BloodwebManager.GetCurrentPrestige() : BloodwebManager.GetCurrentPrestige() + 1, 100);
                    break;

                case 4: // INFO
                    ImGui.Text($"Name: {InfoManager.PlayerName}");
                    ImGui.Text($"Platform: {InfoManager.Platform}");
                    ImGui.Text($"PlayerId: {InfoManager.PlayerId}");
                    ImGui.SameLine();
                    if (ImGui.Button("Copy PlayerId")) WindowsClipboard.SetText(InfoManager.PlayerId);

                    ImGui.Dummy(new Vector2(0, 20));

                    ImGui.Text($"Match Region: {InfoManager.MatchRegion}");
                    ImGui.Text($"MatchId: {InfoManager.MatchId}");
                    ImGui.SameLine();
                    if (ImGui.Button("Copy MatchId")) WindowsClipboard.SetText(InfoManager.MatchId);

                    ImGui.Dummy(new Vector2(0, 20));

                    foreach (var Player in InfoManager.Players)
                    {
                        ImGui.Text($"{Player.role}: {Player.name}");
                        if (ImGui.Button($"Copy {Player.name}'s CloudID")) WindowsClipboard.SetText(Player.userId);
                        ImGui.SameLine();
                        if (Player.providerUrl != null)
                        {
                            if (ImGui.Button($"Copy {Player.name}'s URL")) WindowsClipboard.SetText(Player.providerUrl);
                        }

                        ImGui.Dummy(new Vector2(0, 10));
                    }

                    break;
            }
            ImGui.EndChild();

            ImGui.End();
        }
    }
}
