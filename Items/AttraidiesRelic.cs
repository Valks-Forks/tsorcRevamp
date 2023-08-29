﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.NPCs.Enemies;

namespace tsorcRevamp.Items;

class AttraidiesRelic : ModItem
{

    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Summons a mindflayer illusion from the legion army of Attraidies!" +
                            "\n\"[c/00ffd4:You feel compelled to try this...]\"");
    }
    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 40;
        Item.consumable = true;
        Item.maxStack = 99;
        Item.value = 1000;
        Item.rare = ItemRarityID.Blue;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.UseSound = new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/Custom/EvilLaugh");
        Item.scale = 1f;
        Item.useStyle = ItemUseStyleID.HoldUp;
    }
    public override bool? UseItem(Player player)
    {
        if (!NPC.AnyNPCs(ModContent.NPCType<AttraidiesIllusion>()))
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AttraidiesIllusion>());
            return true;
        }
        else
        {
            return false;
        }
    }
}