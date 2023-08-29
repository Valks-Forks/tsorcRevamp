﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors.Melee;

[AutoloadEquip(EquipType.Legs)]
public class DarkKnightGreaves : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Forsaken by the one who turned Paladin." +
            "\nIncreases movement and melee speed by 17%");
    }
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.defense = 15;
        Item.rare = ItemRarityID.Yellow;
        Item.value = PriceByRarity.fromItem(Item);
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed += 0.17f;
        player.GetAttackSpeed(DamageClass.Melee) += 0.17f;

        if (player.HasBuff(BuffID.ShadowDodge))
        {
            player.moveSpeed += 0.17f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.17f;
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.HallowedGreaves, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 10000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}

