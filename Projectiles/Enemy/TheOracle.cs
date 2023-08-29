﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Projectiles.Enemy;

class TheOracle : ModProjectile
{

    public override void SetDefaults()
    {
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.height = 16;
        Projectile.width = 16;
        Projectile.timeLeft = 250;

    }
    public override void AI()
    {
        Projectile.rotation += .5f;
        if (Main.rand.NextBool(4))
        {
            int dust = Dust.NewDust(new Vector2((float)Projectile.position.X, (float)Projectile.position.Y), Projectile.width, Projectile.height, 6, 0, 0, 50, Color.Green, 3.0f);
            Main.dust[dust].noGravity = false;
        }
        Lighting.AddLight(Projectile.position, 0.5f, 0.6f, 0.1f);

        if (Projectile.velocity.X <= 4 && Projectile.velocity.Y <= 4 && Projectile.velocity.X >= -4 && Projectile.velocity.Y >= -4)
        {
            float accel = 1f + (Main.rand.Next(10, 30) * 0.001f);
            Projectile.velocity.X *= accel;
            Projectile.velocity.Y *= accel;
        }
    }
    public override bool PreKill(int timeLeft)
    {
        Projectile.type = ProjectileID.Grenade;
        return true;
    }
    public override void OnHitPlayer(Player target, int damage, bool crit)
    {
        int buffLengthMod = 1;
        if (Main.expertMode)
        {
            buffLengthMod = 2;
        }
        target.AddBuff(BuffID.Battle, 600 / buffLengthMod);
        target.AddBuff(BuffID.BrokenArmor, 300 / buffLengthMod);
        target.AddBuff(BuffID.Poisoned, 600 / buffLengthMod);
        target.AddBuff(BuffID.Bleeding, 600 / buffLengthMod);
    }

}
