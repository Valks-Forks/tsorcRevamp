﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Accessories.Defensive;

public class BloodbiteRing : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("One of the infamous bite rings commissioned by Sir Arstor of Carim" +
                            "\nDespite the dreadful rumors surrounding its creation, this ring" +
                            "\nis an unmistakable asset, due to its ability to prevent bleeding.");
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.defense = 3;
        Item.accessory = true;
        Item.value = PriceByRarity.Blue_1;
        Item.rare = ItemRarityID.Blue;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ModContent.ItemType<SilverRing>(), 1);
        recipe.AddIngredient(ModContent.ItemType<BloodredMossClump>(), 10);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 2000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }

    public override void UpdateEquip(Player player)
    {
        player.buffImmune[BuffID.Bleeding] = true;
    }

}
