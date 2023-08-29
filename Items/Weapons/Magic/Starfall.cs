﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Weapons.Magic;

class Starfall : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Summon a rain of homing stars\n'Your very own meteor shower'");
    }
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.useStyle = ItemUseStyleID.Shoot;

        Item.knockBack = 6;
        Item.scale = 0.9f;

        Item.rare = ItemRarityID.Cyan;
        Item.shootSpeed = 10;
        Item.noMelee = true;
        Item.value = PriceByRarity.Cyan_9;
        Item.DamageType = DamageClass.Magic;

        Item.UseSound = SoundID.Item25;
        Item.mana = 14;
        Item.damage = 80;
        Item.useAnimation = Item.useTime = 15;
        Item.shoot = ModContent.ProjectileType<Projectiles.StarfallProjectile>();
        Item.autoReuse = true;

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.SpellTome, 1);
        recipe.AddIngredient(ItemID.FallenStar, 30);
        recipe.AddIngredient(ModContent.ItemType<BlueTitanite>(), 5);
        recipe.AddIngredient(ModContent.ItemType<GuardianSoul>(), 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 50000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }

    public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 speed, int type, int damage, float knockBack)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = new Vector2(player.Center.X + Main.rand.Next(151) * (0f - player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            pos.X = (pos.X + player.Center.X) / 2f + Main.rand.Next(-200, 201);
            pos.Y -= 100 * i; //they all spawn simultaneously, so offset them vertically to slightly stagger them
            float velX = Main.mouseX + Main.screenPosition.X - pos.X + Main.rand.Next(-40, 41) * 0.03f;
            float velY = Main.mouseY + Main.screenPosition.Y - pos.Y;
            if (velY < 0f)
            {
                velY *= -1f;
            }
            if (velY < 20f)
            {
                velY = 20f;
            }
            Vector2 h = Vector2.Normalize(new Vector2(velX, velY)) * Item.shootSpeed;
            h.Y += Main.rand.Next(-40, 41) * 0.02f;
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), pos.X, pos.Y, h.X * 1.5f, h.Y * 1.5f, type, damage, knockBack, player.whoAmI, 0f, 0f);
        }
        return false;
    }
}
