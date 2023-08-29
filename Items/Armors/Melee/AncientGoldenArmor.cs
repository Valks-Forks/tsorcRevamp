﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors.Melee;

[LegacyName("AncientDwarvenArmor")]
[AutoloadEquip(EquipType.Body)]
public class AncientGoldenArmor : ModItem
{
    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("A lost prince's armor." +
            "\n11% increased melee speed" +
            "\nSet bonus: Increases melee damage by 3 flat" +
            "\nA gi will also proc this set bonus");
    }
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.defense = 6;
        Item.rare = ItemRarityID.Green;
        Item.value = PriceByRarity.fromItem(Item);
    }
    public override void UpdateEquip(Player player)
    {
        player.GetAttackSpeed(DamageClass.Melee) += 0.11f;
    }
    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head.type == ModContent.ItemType<AncientGoldenHelmet>() && legs.type == ModContent.ItemType<AncientGoldenGreaves>();
    }
    public override void UpdateArmorSet(Player player)
    {
        player.GetDamage(DamageClass.Melee).Flat += 3f;
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.GoldChainmail, 1);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 250);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}
