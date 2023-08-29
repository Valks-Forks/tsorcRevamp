﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors;

[AutoloadEquip(EquipType.Legs)]
public class DragoonGreaves2 : ModItem
{
    public override string Texture => "tsorcRevamp/Items/Armors/DragoonGreaves";
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Supreme Dragoon Greaves");
        Tooltip.SetDefault("Grants an extra double jump and Shiny Red Balloon effect\nIncreases your max number of minions by 3");
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.defense = 15;
        Item.rare = ItemRarityID.Expert;
        Item.value = PriceByRarity.fromItem(Item);
    }

    public override void UpdateEquip(Player player)
    {
        player.hasJumpOption_Unicorn = true;
        player.jumpBoost = true;
        player.spawnMax = true; //?????
        player.maxMinions += 3;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        RasterizerState OverflowHiddenRasterizerState = new RasterizerState
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
        Texture2D texture = (Texture2D)Terraria.GameContent.TextureAssets.Item[Item.type];
        for (int i = 0; i < 4; i++)
        {
            Vector2 offsetPositon = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * 3;
            spriteBatch.Draw(texture, position + offsetPositon, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);

        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ModContent.ItemType<DragoonGreaves>());
        //recipe.AddIngredient(ModContent.ItemType<DragonEssence>(), 1);
        recipe.AddIngredient(ModContent.ItemType<FlameOfTheAbyss>(), 10);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 40000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}

