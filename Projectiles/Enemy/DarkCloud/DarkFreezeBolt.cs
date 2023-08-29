﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Projectiles.Enemy.DarkCloud;

class DarkFreezeBolt : ModProjectile
{

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.alpha = 0;
        Projectile.timeLeft = 400;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.penetrate = 10;
        Projectile.tileCollide = true;
        Projectile.DamageType = DamageClass.Magic;
    }

    public override void AI()
    {
        if (Projectile.type == 96 && Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
        }

        if (Main.rand.NextBool(10))
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.MagicMirror, Projectile.velocity * 0.5f, 100, default, 2f).noLight = true;
        }
        if (Main.rand.NextBool(10))
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.ShadowbeamStaff, Projectile.velocity * 0.5f, 100, default, 2f).noLight = true;
        }

        Projectile.rotation += 0.3f * (float)Projectile.direction;
    }

    public override bool OnTileCollide(Vector2 CollideVel)
    {
        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        Projectile.ai[0] += 1f;

        Projectile.velocity *= 0.5f;

        if (Projectile.ai[0] >= 3f)
        {
            Projectile.position += Projectile.velocity;
            Projectile.Kill();
        }
        else
        {
            if (Projectile.velocity.Y > 4f)
            {
                if (Projectile.velocity.Y != CollideVel.Y)
                {
                    Projectile.velocity.Y = -CollideVel.Y * 0.8f;
                }
            }
            else
            {
                if (Projectile.velocity.Y != CollideVel.Y)
                {
                    Projectile.velocity.Y = -CollideVel.Y;
                }
            }
            if (Projectile.velocity.X != CollideVel.X)
            {
                Projectile.velocity.X = -CollideVel.X;
            }
        }
        return false;
    }

    static Texture2D texture;
    public override bool PreDraw(ref Color lightColor)
    {
        UsefulFunctions.DrawSimpleLitProjectile(Projectile, ref texture);
        return false;
    }
}
