﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace tsorcRevamp.Projectiles;

class PolarisShot : ModProjectile
{
    public override string Texture => "tsorcRevamp/Projectiles/PulsarShot";
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 8;
    }
    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.aiStyle = 0;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
    }

    public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
    {
        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 107)
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(ModContent.BuffType<Buffs.PolarisElectrocutedBuff>(), 360);
            }
            Projectile.timeLeft = 2;
        }

        else
        {
            Projectile.tileCollide = true;
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.damage = (int)(originalDamage * 1f);
            Projectile.knockBack = 3f;
            Projectile.DamageType = DamageClass.Ranged;

            if (Main.rand.NextBool(2))
            {
                target.AddBuff(ModContent.BuffType<Buffs.PolarisElectrocutedBuff>(), 240);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.VFX.ShockwaveEffect>(), 0, 0, Main.myPlayer, 200, 30);
            }

            Projectile.timeLeft = 0;
        }
    }

    float rotation = 0;
    public override bool PreDraw(ref Color lightColor)
    {
        rotation += 0.1f;
        Texture2D texture = (Texture2D)Terraria.GameContent.TextureAssets.Projectile[Projectile.type];
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 32, 32, 32), Color.White, Projectile.rotation, new Vector2(16, 16), Projectile.scale, SpriteEffects.None, 0);

        
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(16, 16).RotatedBy(rotation), new Rectangle(0, Projectile.frame * 32, 32, 32), Color.White, Projectile.rotation, new Vector2(16, 16), Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(-16, 16).RotatedBy(rotation), new Rectangle(0, Projectile.frame * 32, 32, 32), Color.White, Projectile.rotation, new Vector2(16, 16), Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(16, -16).RotatedBy(rotation), new Rectangle(0, Projectile.frame * 32, 32, 32), Color.White, Projectile.rotation, new Vector2(16, 16), Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(-16, -16).RotatedBy(rotation), new Rectangle(0, Projectile.frame * 32, 32, 32), Color.White, Projectile.rotation, new Vector2(16, 16), Projectile.scale, SpriteEffects.None, 0);
        


        return false;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft >= 109)
        {
            Projectile.timeLeft = 0;

            if (Main.netMode != NetmodeID.Server)
            {
                Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/Item/PulsarBump") with { Volume = 0.6f, PitchVariance = .3f }, Projectile.Center);
            }

        }
        else
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 134;
            Projectile.height = 134;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.damage = (int)(originalDamage * 1.36f);
            Projectile.knockBack = 10f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 0;
            if (Main.netMode != NetmodeID.Server)
            {
                Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/Item/PulsarBoom") with { Volume = 0.6f, PitchVariance = .3f }, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.VFX.ShockwaveEffect>(), 0, 0, Main.myPlayer, 200, 30);
            }
            for (int i = 0; i < 100; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 30, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 100; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 70, default(Color), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= Main.rand.NextFloat(3f, 4.5f);
                Main.dust[dust].scale = 0.2f + (float)Main.rand.Next(5) * 0.1f;
            }

        }
        return false;
    }

    public int polarisdusttimer;
    public int originalDamage = 0;
    public bool spawned = false;

    public override void AI()
    {
        Lighting.AddLight(Projectile.position, 0.0452f, 0.21f, 0.073f);

        float rotationsPerSecond = 1.6f;
        bool rotateClockwise = true;
        Projectile.rotation += (rotateClockwise ? 1 : -1) * MathHelper.ToRadians(rotationsPerSecond * 6f);

        if (!spawned)
        {
            spawned = true;
            originalDamage = Projectile.damage;
        }

        //DUST SPAWNING
        if (Main.rand.NextBool(3))
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 100, default(Color), .4f);
            Main.dust[dust].noGravity = true;
        }
        polarisdusttimer++;
        if (polarisdusttimer == 15)
        {
            for (int a = 0; a < 15; a++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width - 9, Projectile.height - 9, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 100, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }
        }
        if (polarisdusttimer == 30)
        {
            for (int a = 0; a < 15; a++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width - 9, Projectile.height - 9, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 100, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }
        }
        if (polarisdusttimer == 45)
        {
            for (int a = 0; a < 15; a++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width - 9, Projectile.height - 9, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 100, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }
        }
        if (polarisdusttimer == 60)
        {
            for (int a = 0; a < 15; a++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width - 9, Projectile.height - 9, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 100, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }
        }
        if (polarisdusttimer == 75)
        {
            for (int a = 0; a < 15; a++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width - 9, Projectile.height - 9, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 100, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }
        }

        //ANIMATION
        if (polarisdusttimer > 90)
        {
            polarisdusttimer = 0;
        }

        if (Projectile.timeLeft >= 109)
        {
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame == 4)
                {
                    Projectile.frame = 0;
                }
            }
        }

        if (Projectile.timeLeft <= 108)
        {
            if (Projectile.frame < 5)
            {
                Projectile.frame = 5;
            }

            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                {
                    Projectile.frame = 5;
                }
            }
        }

        Vector2 oldSize = Projectile.Size;

        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 107)
        {
            Projectile.tileCollide = true;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.damage = (int)(originalDamage * 1.36f);
            Projectile.knockBack = 10f;
            Projectile.DamageType = DamageClass.Ranged;
        }

        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft == 108)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/Item/PulsarGrow") with { Volume = 0.6f, PitchVariance = .3f }, Projectile.Center);
            }
        }

        Projectile.position = Projectile.position - (Projectile.Size - oldSize) / 2f;

        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft == 1)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/Item/PulsarBoom") with { Volume = 0.6f, PitchVariance = .3f }, Projectile.Center);
            }
            for (int i = 0; i < 200; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 30, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 150; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 70, default(Color), .6f);
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 250; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 70, default(Color), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= Main.rand.NextFloat(3f, 4.5f);
                Main.dust[dust].scale = 0.2f + (float)Main.rand.Next(5) * 0.1f;
            }
        }

        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 2)
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 134;
            Projectile.height = 134;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.damage = (int)(originalDamage * 1.36f);
            Projectile.knockBack = 10f;
            Projectile.DamageType = DamageClass.Ranged;
        }
        if (Projectile.wet)
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 134;
            Projectile.height = 134;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.damage = (int)(originalDamage * 1.5f);
            Projectile.knockBack = 10f;
            Projectile.DamageType = DamageClass.Ranged;
            if (Main.netMode != NetmodeID.Server)
            {
                Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/Item/PulsarBoom") with { Volume = 0.6f, PitchVariance = .3f }, Projectile.Center);
            }
            for (int i = 0; i < 70; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 30, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 70, default(Color), .6f);
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 70; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 70, default(Color), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= Main.rand.NextFloat(2.5f, 4f);
                Main.dust[dust].scale = 0.2f + (float)Main.rand.Next(5) * 0.1f;


                Projectile.timeLeft = 0;
            }
        }
    }

    public override void Kill(int timeLeft)
    {
        for (int i = 0; i < 100; i++)
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, Projectile.velocity.X * 0f, Projectile.velocity.Y * 0f, 70, default(Color), .6f);
            Main.dust[dust].noGravity = true;
        }
    }
}