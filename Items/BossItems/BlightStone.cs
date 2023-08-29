﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace tsorcRevamp.Items.BossItems;

class BlightStone : ModItem
{

    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Summons The Blight, one of six guardians of The Abyss." +
                            "\nYou must fight this battle on the surface." +
                            "\nThe Blight cannot be fought with the Covenant of Artorias ring equipped.");
    }
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 30;
        Item.consumable = false;
        Item.maxStack = 1;
        Item.value = 100000;
        Item.rare = ItemRarityID.Pink;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.scale = 1f;
        Item.useStyle = ItemUseStyleID.HoldUp;
    }
    public override bool? UseItem(Player player)
    {
        if (player.ZoneOverworldHeight && !Main.bloodMoon && !NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.SuperHardMode.Blight>()))
        {
            NPC.SpawnOnPlayer(Main.myPlayer, ModContent.NPCType<NPCs.Bosses.SuperHardMode.Blight>());
            UsefulFunctions.BroadcastText("\"You will be destroyed\"", 255, 50, 50);
            return true;
        }
        else
        {
            return false;
        }
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();

        recipe.AddIngredient(ItemID.StoneBlock, 1);
        recipe.AddIngredient(ModContent.ItemType<Humanity>(), 10);
        recipe.AddIngredient(ModContent.ItemType<CursedSoul>(), 30);
        recipe.AddIngredient(ModContent.ItemType<BlueTitanite>(), 5);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 1000);
        recipe.AddTile(TileID.DemonAltar);
        recipe.AddCondition(tsorcRevampWorld.AdventureModeDisabled);

        recipe.Register();
    }
}