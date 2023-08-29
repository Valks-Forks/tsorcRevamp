﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors.Magic;

[LegacyName("AncientDragonScaleGreaves")]
[AutoloadEquip(EquipType.Legs)]
public class DragonScaleGreaves : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Known to be treasured by assassins.\n+25% movement.");
    }
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.defense = 5;
        Item.rare = ItemRarityID.LightPurple;
        Item.value = PriceByRarity.fromItem(Item);
    }
    public override void UpdateEquip(Player player)
    {
        player.moveSpeed += 0.25f;
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.MythrilGreaves, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 3500);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}

