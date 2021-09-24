﻿using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class AlucardWig : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Part of the TSORC Revamp Team dev set belonging to NephilimDeath");
        }

        public override void SetDefaults()
        {
            item.vanity = true;
            item.width = 26;
            item.height = 20;
            item.value = 100000;
            item.rare = ItemRarityID.Yellow;
        }
    }
}