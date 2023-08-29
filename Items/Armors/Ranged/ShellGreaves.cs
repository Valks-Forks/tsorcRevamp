﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors.Ranged;

[AutoloadEquip(EquipType.Legs)]
class ShellGreaves : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Armor made from the shell of a legendary creature" +
            "\n+15% movement speed");
    }

    public override void SetDefaults()
    {
        Item.defense = 8;
        Item.rare = ItemRarityID.LightRed;
        Item.value = PriceByRarity.fromItem(Item);
        Item.width = 18;
        Item.height = 18;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.NecroGreaves);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 3000);
        recipe.AddTile(TileID.DemonAltar);
        recipe.Register();
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed += 0.15f;
    }
}
