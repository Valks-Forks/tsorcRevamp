﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Buffs;

class PhazonContamination : ModBuff
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Phazon Contamination");
        Description.SetDefault("Radiation is tearing your cells apart");
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;

        BuffID.Sets.LongerExpertDebuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<tsorcRevampPlayer>().PhazonCorruption = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<NPCs.tsorcRevampGlobalNPC>().PhazonCorruption = true;
    }
}
