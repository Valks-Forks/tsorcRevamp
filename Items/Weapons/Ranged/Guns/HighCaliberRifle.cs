﻿using Terraria.ID;
using Terraria.ModLoader;
using Terraria;


namespace tsorcRevamp.Items.Weapons.Ranged.Guns;

class HighCaliberRifle : ModItem
{

    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Incredible damage at the cost of 2.5 second cooldown between shots" +
                            "\nRemember to hold your breath.");
    }

    public override void SetDefaults()
    {
        Item.damage = 700;
        Item.height = 22;
        Item.noMelee = true;
        Item.autoReuse = true;
        Item.DamageType = DamageClass.Ranged;
        Item.rare = ItemRarityID.Lime;
        Item.scale = 1;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 20;
        Item.useAmmo = AmmoID.Bullet;
        Item.useAnimation = 150;
        Item.UseSound = SoundID.Item11;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = 150;
        Item.value = PriceByRarity.Lime_7;
        Item.width = 66;
        Item.knockBack = 50f;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Megashark, 1);
        recipe.AddIngredient(ItemID.IllegalGunParts, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 80000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}
