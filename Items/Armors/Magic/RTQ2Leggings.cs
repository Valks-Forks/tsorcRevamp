﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors.Magic;

[AutoloadEquip(EquipType.Legs)]
public class RTQ2Leggings : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("RTQ2 Leggings");
        Tooltip.SetDefault("Increases movement speed by 13%");
    }
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.defense = 7;
        Item.rare = ItemRarityID.Pink;
        Item.value = PriceByRarity.fromItem(Item);
    }
    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Magic).Flat += 4;
        player.moveSpeed += 0.13f;
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.MeteorLeggings, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 3000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}

