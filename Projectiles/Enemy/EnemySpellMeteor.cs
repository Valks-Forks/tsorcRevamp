using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace tsorcRevamp.Projectiles.Enemy;

class EnemySpellMeteor : ModProjectile
{

    public override void SetDefaults()
    {
        Projectile.aiStyle = 1;
        Projectile.hostile = true;
        Projectile.height = 40;
        Projectile.light = 0.5f;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 50;
        Projectile.tileCollide = false;
        Projectile.width = 40;
    }
    public override void Kill(int timeLeft)
    {
        Projectile.type = 29;
    }

    #region AI
    public override void AI()
    {
        if (Projectile.aiStyle == 1)
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
                return;
            }
        }
    }
    #endregion

    #region Kill
    public override bool PreKill(int timeLeft)
    {
        if (!Projectile.active)
        {
            return true;
        }
        Projectile.timeLeft = 0;
        {
            for (int num40 = 0; num40 < 10; num40++)
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
                Vector2 arg_1394_0 = new Vector2(Projectile.position.X - Projectile.velocity.X, Projectile.position.Y - Projectile.velocity.Y);
                int arg_1394_1 = Projectile.width;
                int arg_1394_2 = Projectile.height;
                int arg_1394_3 = 15;
                float arg_1394_4 = 0f;
                float arg_1394_5 = 0f;
                int arg_1394_6 = 100;
                Color newColor = default(Color);
                int num41 = Dust.NewDust(arg_1394_0, arg_1394_1, arg_1394_2, arg_1394_3, arg_1394_4, arg_1394_5, arg_1394_6, newColor, 2f);
                Main.dust[num41].noGravity = true;
                Dust expr_13B1 = Main.dust[num41];
                expr_13B1.velocity *= 2f;
                Vector2 arg_1422_0 = new Vector2(Projectile.position.X - Projectile.velocity.X, Projectile.position.Y - Projectile.velocity.Y);
                int arg_1422_1 = Projectile.width;
                int arg_1422_2 = Projectile.height;
                int arg_1422_3 = 15;
                float arg_1422_4 = 0f;
                float arg_1422_5 = 0f;
                int arg_1422_6 = 100;
                newColor = default(Color);
                num41 = Dust.NewDust(arg_1422_0, arg_1422_1, arg_1422_2, arg_1422_3, arg_1422_4, arg_1422_5, arg_1422_6, newColor, 1f);
            }
        }
        Projectile.active = false;
        return true;
    }
    #endregion

}