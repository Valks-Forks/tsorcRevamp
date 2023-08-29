﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Accessories;

public class MonocleOfPrecision : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Monocle of Precision");
        Tooltip.SetDefault("Increases critical strike chance by 5%");
    }
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 28;
        Item.accessory = true;
        Item.value = PriceByRarity.Green_2;
        Item.rare = ItemRarityID.Green;
    }
    public override void UpdateEquip(Player player)
    {
        player.GetCritChance(DamageClass.Generic) += 5;
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.BlackLens);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 1000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}
