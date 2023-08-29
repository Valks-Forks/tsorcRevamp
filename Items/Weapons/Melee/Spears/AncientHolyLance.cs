using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace tsorcRevamp.Items.Weapons.Melee.Spears;

public class AncientHolyLance : ModItem
{

    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Ancient Holy Lance");
        Tooltip.SetDefault("Bright Holy Spear.");
    }

    public override void SetDefaults()
    {
        Item.damage = 59;
        Item.knockBack = 8.5f;
        Item.scale = 0.9f;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 11;
        Item.useTime = 7;
        Item.shootSpeed = 8;
        //item.shoot = ProjectileID.DarkLance;

        Item.height = 50;
        Item.width = 50;

        Item.DamageType = DamageClass.Melee;
        Item.noMelee = true;
        Item.noUseGraphic = true;

        Item.value = PriceByRarity.LightRed_4;
        Item.rare = ItemRarityID.LightPurple;
        Item.maxStack = 1;
        Item.UseSound = SoundID.Item1;
        Item.shoot = ModContent.ProjectileType<Projectiles.Spears.AncientHolyLanceProj>();

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.MythrilHalberd);
        recipe.AddIngredient(ItemID.MythrilBar);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 6000);

        recipe.AddTile(TileID.DemonAltar);
        recipe.Register();
    }
}
