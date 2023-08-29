using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Accessories;

public class HolyNecklace : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Emits blue light, longer invincibility and starcloak effect.");
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = PriceByRarity.LightRed_4;
        Item.rare = ItemRarityID.LightRed;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ModContent.ItemType<FrozenStarlight>(), 1);
        recipe.AddIngredient(ItemID.StarCloak, 1);
        recipe.AddIngredient(ItemID.CrossNecklace, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 1000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();

        Recipe recipe2 = CreateRecipe();
        recipe2.AddIngredient(ModContent.ItemType<FrozenStarlight>(), 1);
        recipe2.AddIngredient(ItemID.StarVeil, 1);
        recipe2.AddIngredient(ModContent.ItemType<DarkSoul>(), 1000);
        recipe2.AddTile(TileID.DemonAltar);
        recipe2.Register();
    }

    public override void UpdateEquip(Player player)
    {
        Lighting.AddLight((int)(player.position.X + (float)(player.width / 2) + (float)(8 * player.direction)) / 16, (int)(player.position.Y + 2f) / 16, 0.75f, 0.75f, 1.5f);
        player.starCloakItem = new Item(ItemID.StarCloak);
        player.longInvince = true;
    }

}