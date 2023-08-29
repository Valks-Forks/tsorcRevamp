﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Weapons.Magic;

class WandOfFire2 : ModItem
{
    public override string Texture => "tsorcRevamp/Items/Weapons/Magic/WandOfFire";
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Wand of Fire II");
        Item.staff[Item.type] = true;
    }
    public override void SetDefaults()
    {
        Item.autoReuse = true;
        Item.width = 12;
        Item.height = 17;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 19;
        Item.useTime = 19;
        Item.maxStack = 1;
        Item.damage = 23;
        Item.knockBack = 1;
        Item.mana = 8;
        Item.UseSound = SoundID.Item20;
        Item.shootSpeed = 14;
        Item.noMelee = true;
        Item.value = PriceByRarity.Green_2;
        Item.DamageType = DamageClass.Magic;
        Item.rare = ItemRarityID.Green;
        Item.shoot = ModContent.ProjectileType<Projectiles.FireBall>();
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Stinger, 3);
        recipe.AddIngredient(ModContent.ItemType<WandOfFire>(), 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 2500);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}
