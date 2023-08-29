using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors.Ranged;

[AutoloadEquip(EquipType.Head)]
public class ArcherOfLumeliaHairStyle : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Archer of Lumelia Hairstyle");
        Tooltip.SetDefault("Gifted with bows, repeaters, and other long range weapons");
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 12;
        Item.defense = 1;
        Item.rare = ItemRarityID.Lime;
        Item.value = PriceByRarity.fromItem(Item);
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.AdamantiteMask, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 4000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}
