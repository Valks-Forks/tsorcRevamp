using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tsorcRevamp.Items.Accessories.Mobility;

public class ImprovedWhiteBalloon : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Improved Blizzard in a Balloon");
        Tooltip.SetDefault("Allows the holder to double jump" +
                            "\nIncreases jump height + 60% jump speed");
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 28;
        Item.accessory = true;
        Item.value = PriceByRarity.Green_2;
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.BlizzardinaBalloon, 1);
        recipe.AddIngredient(ItemID.Bone, 20);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 900);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
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
            spriteBatch.Draw(texture, position + offsetPositon, null, Color.GreenYellow, 0, origin, scale, SpriteEffects.None, 0);
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);

        return true;
    }

    public override void UpdateEquip(Player player)
    {
        player.jumpSpeedBoost += 0.6f;
        player.jumpBoost = true;
        player.hasJumpOption_Blizzard = true;
    }

}