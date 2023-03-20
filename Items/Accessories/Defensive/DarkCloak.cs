﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Accessories.Defensive
{
    public class DarkCloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("[c/ffbf00:Dark Cloak gives +15 defense when life falls below 40%]" +
                                "\n+5 defense normally");
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
            recipe.AddIngredient(ItemID.SoulofNight, 1);
            recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 15000);
            recipe.AddTile(TileID.DemonAltar);

            recipe.Register();
        }

        public override void UpdateEquip(Player player)
        {
            if (player.statLife <= (player.statLifeMax / 5 * 2))
            {
                player.statDefense += 15;
            }
            else
            {
                player.statDefense += 5;
            }

            
        }

    }
}
