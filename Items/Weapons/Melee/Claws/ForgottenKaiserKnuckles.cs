﻿using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using tsorcRevamp.Projectiles;

namespace tsorcRevamp.Items.Weapons.Melee.Claws;

class ForgottenKaiserKnuckles : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Great spiked knuckles.");
    }

    public override void SetDefaults()
    {
        Item.autoReuse = true;
        Item.useTurn = false;
        Item.rare = ItemRarityID.Green;
        Item.damage = 17;
        Item.width = 21;
        Item.height = 23;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 3;
        Item.DamageType = DamageClass.Melee;
        Item.useAnimation = 8;
        Item.useTime = 8;
        Item.UseSound = SoundID.Item1;
        Item.value = PriceByRarity.Green_2;
        Item.shoot = ModContent.ProjectileType<Nothing>();
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.BladedGlove);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 1000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}
