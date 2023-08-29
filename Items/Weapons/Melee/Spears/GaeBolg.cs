using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace tsorcRevamp.Items.Weapons.Melee.Spears;

public class GaeBolg : ModItem
{

    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Gae Bolg");
        Tooltip.SetDefault("Pierce reality \nCan be upgraded into its mythical form with 70,000 Dark Souls");
    }

    public override void SetDefaults()
    {
        Item.damage = 79;
        Item.knockBack = 5.5f;

        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.shootSpeed = 8;

        Item.height = 40;
        Item.width = 40;

        Item.DamageType = DamageClass.Melee;
        Item.noMelee = true;
        Item.noUseGraphic = true;

        Item.value = PriceByRarity.LightPurple_6;
        Item.rare = ItemRarityID.LightPurple;
        Item.maxStack = 1;
        Item.UseSound = SoundID.Item1;
        Item.shoot = ModContent.ProjectileType<Projectiles.Spears.GaeBolgProj>();

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Gungnir);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 40000);

        recipe.AddTile(TileID.DemonAltar);
        recipe.Register();
    }
}
