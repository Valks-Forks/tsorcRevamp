﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors.Magic;

[AutoloadEquip(EquipType.Legs)]
public class MimeticPants : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Increases movement speed by 10%" +
            "\n15% increased magic critical strike chance");
    }
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.defense = 7;
        Item.rare = ItemRarityID.LightRed;
        Item.value = PriceByRarity.fromItem(Item);
    }
    public override void UpdateEquip(Player player)
    {
        player.moveSpeed += 0.1f;
        player.GetCritChance(DamageClass.Magic) += 15;
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.JunglePants, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 3000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}

