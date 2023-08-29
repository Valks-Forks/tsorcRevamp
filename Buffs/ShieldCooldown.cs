﻿using Terraria;
using Terraria.ModLoader;

namespace tsorcRevamp.Buffs;

class ShieldCooldown : ModBuff
{

    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Shield Cooldown");
        Description.SetDefault("You cannot use wall tomes!");
        Main.debuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
        Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true; //prevents nurse clearing
    }
}
