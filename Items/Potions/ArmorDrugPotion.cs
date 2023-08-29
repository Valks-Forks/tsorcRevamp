﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Potions;

public class ArmorDrugPotion : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Armor Drug");
        Tooltip.SetDefault("Increases defense by 25 and damage reduction by 15" +
                           "\nDoes not stack with Demon Drug, Strength, or Battlefront Potions.");

    }

    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 24;
        Item.useStyle = ItemUseStyleID.EatFood;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
        Item.maxStack = 9999;
        Item.consumable = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = 300000;
        Item.buffType = ModContent.BuffType<Buffs.ArmorDrug>();
        Item.buffTime = 28800;
    }

    public override bool? UseItem(Player player)
    {
        int currentBuff = 0;
        foreach (int buffType in player.buffType)
        {
            if (buffType == ModContent.BuffType<Buffs.Strength>() || buffType == ModContent.BuffType<Buffs.DemonDrug>() || buffType == ModContent.BuffType<Buffs.Battlefront>())
            {
                player.DelBuff(currentBuff);
            }
            currentBuff++;
        }
        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe(4);
        recipe.AddIngredient(ItemID.EndurancePotion, 4);
        recipe.AddIngredient(ItemID.IronskinPotion, 4);
        recipe.AddIngredient(ItemID.Sapphire, 4);
        recipe.AddIngredient(ItemID.SoulofNight, 4);
        recipe.AddTile(TileID.Bottles);

        recipe.Register();
    }
}
