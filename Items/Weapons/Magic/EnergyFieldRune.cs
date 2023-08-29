﻿using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace tsorcRevamp.Items.Weapons.Magic;

public class EnergyFieldRune : ModItem
{

    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Creates an electric energy orb at the point of impact." +
                            "\nLasts for a few seconds.");
    }
    public override void SetDefaults()
    {
        Item.damage = 30;
        Item.height = 28;
        Item.knockBack = 4;
        Item.rare = ItemRarityID.LightRed;
        Item.autoReuse = true;
        Item.shootSpeed = 6;
        Item.DamageType = DamageClass.Magic;
        Item.noMelee = true;
        Item.useAnimation = 40;
        Item.UseSound = SoundID.Item21;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = 40;
        Item.value = PriceByRarity.LightRed_4;
        Item.width = 20;
        Item.mana = 20;
        Item.shoot = ModContent.ProjectileType<Projectiles.EnergyFieldBall>();
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.SpellTome, 1);
        recipe.AddIngredient(ItemID.SoulofLight, 3);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 8000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}
