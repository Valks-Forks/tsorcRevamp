﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Weapons.Magic;

class WoodenWand : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("An unenchanted wooden wand \nCan be upgraded many different ways");
    }

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 34;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 25;
        Item.useTime = 25;
        Item.damage = 8;
        Item.DamageType = DamageClass.Magic;
        Item.value = 100;
        Item.knockBack = 3.5f;
        Item.UseSound = SoundID.Item1;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Wood, 3);

        recipe.Register();
    }
}
