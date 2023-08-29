﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.BossItems;

class HellkiteStone : ModItem
{

    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Summons a Hellkite Dragon from the sky...");
    }

    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.LightRed;
        Item.width = 38;
        Item.height = 34;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useAnimation = 45;
        Item.useTime = 45;
        Item.maxStack = 1;
        Item.consumable = false;
    }


    public override bool? UseItem(Player player)
    {

        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.Bosses.SuperHardMode.HellkiteDragon.HellkiteDragonHead>());
        return true;
    }

    public override bool CanUseItem(Player player)
    {
        if (Main.dayTime)
        {
            UsefulFunctions.BroadcastText("Nothing happens... Retry at night.", 175, 75, 255);
            return false;
        }
        if (NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.SuperHardMode.HellkiteDragon.HellkiteDragonHead>()))
        {
            return false;
        }
        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ModContent.ItemType<RedTitanite>(), 5);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 1000);
        recipe.AddTile(TileID.DemonAltar);
        recipe.AddCondition(tsorcRevampWorld.AdventureModeDisabled);

        recipe.Register();
    }
}
