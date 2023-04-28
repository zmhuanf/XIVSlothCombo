using Dalamud.Interface.Colors;
using Dalamud.Utility;
using ImGuiNET;
using System.Numerics;

namespace XIVSlothCombo.Window.Tabs
{
    internal class AboutUs : ConfigWindow
    {
        internal static new void Draw()
        {
            ImGui.BeginChild("About", new Vector2(0, 0), true);

            ImGui.TextColored(ImGuiColors.ParsedGreen, $"v3.0.18.1\n- with love from Team Sloth.");
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.TextWrapped("Brought to you by: \nAki, k-kz, ele-starshade, damolitionn, Taurenkey, Augporto, grimgal, Genesis-Nova, Tartarga and many other contributors!");
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.ParsedPurple);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGuiColors.HealerGreen);

            if (ImGui.Button("Click here to join our Discord Server!"))
            {
                Util.OpenLink("https://discord.gg/xT7zyjzjtY");
            }

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();

            if (ImGui.Button("Got an issue? Click this button and report it!"))
            {
                Util.OpenLink("https://github.com/Nik-Potokar/XIVSlothCombo/issues");
            }
            if (ImGui.Button("想来汉化？点击这里"))
            {
                Util.OpenLink("https://github.com/44451516/XIVSlothCombo");
            }

            ImGui.TextColored(ImGuiColors.ParsedGreen, $"由于写的汉化脚本不算智能，部分区域未匹配到而没有汉化。");
            ImGui.TextColored(ImGuiColors.ParsedGreen, $"如果你要是从某个咸鱼小铺买的，恭喜你，上大当了，请立刻退款差评免得被人嘲笑。");
            ImGui.TextColored(ImGuiColors.ParsedGreen, $"温馨提醒：怂就别开，但开别怂，少开点挂。");
            ImGui.EndChild();
        }
    }
}
