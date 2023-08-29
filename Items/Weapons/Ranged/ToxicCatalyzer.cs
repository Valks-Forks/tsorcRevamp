﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Projectiles;

namespace tsorcRevamp.Items.Weapons.Ranged;

public class ToxicCatalyzer : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Toxic Catalyzer");
        Tooltip.SetDefault("Left-click shots tag and poison enemies"
                            + "\nRight-click shots detonate tags"
                            + "\nThe more tags, the greater the explosion and damage");

    }

    public override void SetDefaults()
    {
        Item.damage = 14;
        Item.DamageType = DamageClass.Ranged;
        Item.crit = 0;
        Item.width = 38;
        Item.height = 28;
        Item.useTime = 18;
        Item.useAnimation = 18;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 2f;
        Item.value = 20000;
        Item.scale = 0.8f;
        Item.rare = ItemRarityID.Green;
        Item.shoot = ModContent.ProjectileType<ToxicCatShot>();
        Item.shootSpeed = 10.6f;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-5, 4);
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool CanUseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.shoot = ModContent.ProjectileType<ToxicCatDetonator>();
        }
        else
        {
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.shoot = ModContent.ProjectileType<ToxicCatShot>();
        }
        return base.CanUseItem(player);
    }

    public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 speed, int type, int damage, float knockBack)
    {
        if (Main.netMode != NetmodeID.Server)
        {
            Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/Item/PulsarShot") with { Volume = 0.6f, PitchVariance = 0.3f }, player.Center);
        }

        {
            Vector2 muzzleOffset = Vector2.Normalize(speed) * 1f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
                position.Y += 3;
            }
        }
        return true;
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        Lighting.AddLight(Item.Right, 0.2496f, 0.4584f, 0.130f);

        if (Main.rand.NextBool(15))
        {
            Dust dust = Main.dust[Dust.NewDust(new Vector2(Item.position.X + 34, Item.position.Y), 8, 28, 75, 1f, 0, 100, default(Color), .8f)];
            dust.noGravity = true;
            dust.velocity += Item.velocity;
            dust.fadeIn = .4f;
        }

        if (Main.rand.NextBool(10))
        {
            Dust dust = Main.dust[Dust.NewDust(new Vector2(Item.position.X + 34, Item.position.Y), 8, 28, 75, 0, 0, 100, default(Color), .8f)];
            dust.velocity *= 0f;
            dust.noGravity = true;
            dust.velocity += Item.velocity;
            dust.fadeIn = .4f;
        }

        Texture2D texture = TransparentTextureHandler.TransparentTextures[TransparentTextureHandler.TransparentTextureType.ToxicCatalyzerGlowmask];
        spriteBatch.Draw(texture, new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height * 0.5f + 2f),
            new Rectangle(0, 0, texture.Width, texture.Height), Color.White, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);
    }
}
