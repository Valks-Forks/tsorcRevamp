﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Projectiles;

namespace tsorcRevamp.Items.Weapons.Ranged;

public class SuperBlaster : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Super Blaster");
    }

    public override void SetDefaults()
    {
        Item.damage = 42;
        Item.DamageType = DamageClass.Ranged;
        Item.crit = 4;
        Item.width = 44;
        Item.height = 24;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 4f;
        Item.value = 25000;
        Item.scale = 0.7f;
        Item.rare = ItemRarityID.Orange;
        Item.shoot = ModContent.ProjectileType<SuperBlasterShot>();
        Item.shootSpeed = 18f;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ModContent.ItemType<Blaster>());
        recipe.AddIngredient(ItemID.HellstoneBar, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 5000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(2, 2);
    }

    public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 speed, int type, int damage, float knockBack)
    {

        Vector2 muzzleOffset = Vector2.Normalize(speed) * 20f;
        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
        }
        return true;


    }
}
