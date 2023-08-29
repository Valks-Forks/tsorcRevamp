﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Accessories.TestCheat;

[Autoload(false)]
public class TreasureMap : ModItem
{

    //this thing has no drop location or recipe. what?
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Shows location, and may reveal hidden rewards");

    }

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Expert;
        Item.value = 9000000;
    }

    public override void UpdateEquip(Player player)
    {
        player.accDepthMeter = 1;
        player.accCompass = 1;
        player.findTreasure = true;
    }
}
