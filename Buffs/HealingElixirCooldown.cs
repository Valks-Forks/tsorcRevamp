﻿using Terraria;
using Terraria.ModLoader;

namespace tsorcRevamp.Buffs;

class HealingElixirCooldown : ModBuff
{

    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Healing Elixir Cooldown");
        Description.SetDefault("You cannot use Healing Elixirs!");
        Main.debuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
        Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true; //prevents nurse clearing
    }
}
