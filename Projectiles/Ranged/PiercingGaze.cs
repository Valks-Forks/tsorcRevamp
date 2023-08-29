﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Projectiles.Ranged;


public class PiercingGaze : GenericLaser
{
    public override string Texture => "tsorcRevamp/Projectiles/Enemy/Okiku/PoisonSmog";

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 999;


        LaserName = "Piercing Gaze";
        FiringDuration = 90;
        TelegraphTime = 30;
        MaxCharge = 30;
        LaserLength = 5000;
        LaserColor = Color.Red;
        TileCollide = false;
        LaserSize = 1.3f;
        LaserTexture = TransparentTextureHandler.TransparentTextureType.RedLaserTransparent;
        LaserTextureHead = new Rectangle(0, 0, 30, 24);
        LaserTextureBody = new Rectangle(0, 26, 30, 30);
        LaserTextureTail = new Rectangle(0, 58, 30, 24);
        LaserSound = SoundID.Item12 with { Volume = 0.5f };

        LaserDebuffs = new List<int>(); 
        DebuffTimers = new List<int>();

        CastLight = false;

        LaserDebuffs.Add(BuffID.OnFire);
        DebuffTimers.Add(300);

        Additive = true;
    }



    Player targetPlayer;

    bool rapid = false;
    public override void AI()
    {
        if (IsAtMaxCharge && !Main.dedServ)
        {
            if (Main.GameUpdateCount % 8 == 0)
            {
                Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("tsorcRevamp/Sounds/Item/MasterBuster"), Projectile.Center);
            }
        }
        if (Main.player[Projectile.owner].active)
        {
            Projectile.Center = Main.player[Projectile.owner].Center;

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = -Vector2.Normalize(Main.player[Projectile.owner].Center - Main.MouseWorld);
                Projectile.rotation = MathHelper.Pi + Projectile.velocity.ToRotation();
                int dir = 1;
                if (Projectile.velocity.X < 0)
                {
                    dir = -1;
                }
                Main.player[Projectile.owner].ChangeDir(dir);
                
                Main.player[Projectile.owner].itemRotation = Projectile.rotation;
                if(dir == 1)
                {
                    Main.player[Projectile.owner].itemRotation += MathHelper.Pi;
                }
            }



            LineDust = true;
            LaserDust = DustID.OrangeTorch;
            if (targetPlayer == null)
            {
                targetPlayer = Main.player[(int)Projectile.ai[0]];
            }
        }
        else
        {
            Projectile.Kill();
        }

        base.AI();
        if (Charge == MaxCharge - 1)
        {
            LaserSize = 0;
            LaserAlpha = 0;
        }

        if (LaserSize < 1.3f)
        {
            LaserSize += (1.3f / 30f);
            LaserAlpha += 1f / 30f;
        }
    }

    public override Vector2 GetOrigin()
    {
        return Main.player[Projectile.owner].Center;
    }
    public override void OnHitPlayer(Player target, int damage, bool crit)
    {
        target.AddBuff(BuffID.OnFire, 300);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (!additiveContext)
        {
            return false;
        }

        //If no custom shader has been given then load the generic one
        if (LaserShader == null)
        {
            LaserShader = ModContent.Request<Effect>("tsorcRevamp/Effects/GenericLaser", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        //Gives the laser its 'flowing' effect
        timeFactor++;

        //Shifts its color slightly over time
        Vector3 hslColor = Main.rgbToHsl(new Color(1, 0.2f, 0.1f));
        hslColor.X += 0.03f * (float)Math.Cos(timeFactor / 50f);
        Color rgbColor = Main.hslToRgb(hslColor);
        LaserShader.Parameters["Color"].SetValue(rgbColor.ToVector3());

        float modifiedSize = LaserSize * 200;

        //Fade in and out, and pulse while targeting
        if ((IsAtMaxCharge && TargetingMode == 0) || (TargetingMode == 2))
        {
            LaserShader.Parameters["Time"].SetValue(timeFactor * 2);
            if (FiringTimeLeft < FadeOutFrames)
            {
                fadePercent = (float)FiringTimeLeft / (float)FadeOutFrames;
            }
            else
            {
                fadePercent += FadeInSpeed;
                if (fadePercent > 1)
                {
                    fadePercent = 1;
                }
            }

            if (rapid)
            {
                fadePercent = 1;
            }
        }
        else if (TelegraphTime + Charge >= MaxCharge || TargetingMode == 1)
        {
            LaserShader.Parameters["Time"].SetValue(timeFactor);
            modifiedSize /= 2;
            fadePercent = (float)Math.Cos(timeFactor / 30f);
            fadePercent = Math.Abs(fadePercent) * 0.2f;
            fadePercent += 0.2f;
        }
        else
        {
            fadePercent = 0;
        }

        //Apply the rest of the parameters it needs
        LaserShader.Parameters["FadeOut"].SetValue(fadePercent);
        LaserShader.Parameters["SecondaryColor"].SetValue(Color.White.ToVector3());
        LaserShader.Parameters["ProjectileSize"].SetValue(new Vector2(Distance, modifiedSize));
        LaserShader.Parameters["TextureSize"].SetValue(tsorcRevamp.tNoiseTexture1.Width);

        //Calculate where to draw it
        Rectangle sourceRectangle = new Rectangle(0, 0, (int)Distance, (int)(modifiedSize));
        Vector2 origin = new Vector2(0, sourceRectangle.Height / 2f);

        //Apply the shader
        LaserShader.CurrentTechnique.Passes[0].Apply();

        //Draw the laser
        Main.EntitySpriteDraw(tsorcRevamp.tNoiseTexture1, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.velocity.ToRotation(), origin, Projectile.scale, SpriteEffects.None, 0);

        if(Charge >= MaxCharge)
        {
            for(int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(tsorcRevamp.tNoiseTexture1, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.velocity.ToRotation(), origin, Projectile.scale, SpriteEffects.None, 0);
            }
        }

        return false;
    }
}
