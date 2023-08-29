using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Accessories.Defensive;

public class ZirconRing : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Increases maximum life by 20%");
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.value = PriceByRarity.LightRed_4;
        Item.rare = ItemRarityID.LightRed;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.SilverBar, 1);
        recipe.AddIngredient(ItemID.SoulofNight, 1);
        recipe.AddIngredient(ItemID.LifeCrystal, 10);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 9000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }

    public override void UpdateEquip(Player player)
    {
        player.statLifeMax2 += (player.statLifeMax2 / 5);
    }
}

