﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors.Melee;

[AutoloadEquip(EquipType.Body)]
public class FighterBreastplate : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Adept at close combat" +
            "\n+20% melee speed" +
            "\nSet Bonus: +25% Melee damage");
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.defense = 24;
        Item.rare = ItemRarityID.Lime;
        Item.value = PriceByRarity.fromItem(Item);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetAttackSpeed(DamageClass.Melee) += 0.2f;
    }
    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head.type == ModContent.ItemType<FighterHairStyle>() && legs.type == ModContent.ItemType<FighterGreaves>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetDamage(DamageClass.Melee) += 0.25f;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.AdamantiteBreastplate, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 4000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}
