﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Projectiles.VFX;

namespace tsorcRevamp.Projectiles.Enemy.Okiku;

class PoisonTrail : DynamicTrail
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DisplayName.SetDefault("Poison Wave");
    }
    public override void SetDefaults()
    {
        Projectile.damage = 0;
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 400;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.hide = true;

        trailWidth = 45;
        trailPointLimit = 2000;
        trailMaxLength = 9999999;
        collisionPadding = 50;
        NPCSource = true;
        trailCollision = true;
        collisionFrequency = 5;
        noFadeOut = true;
        customEffect = ModContent.Request<Effect>("tsorcRevamp/Effects/CataluminanceTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
    }

    float timer = 0;
    float fixedSpeed = 1;
    public bool FinalStandMode = false;
    List<Vector2> trailVelocities;
    public override void AI()
    {
        timer++;
        float timerLimit = 45;
        if (Projectile.ai[0] == 2)
        {
            if(timer == 1)
            {
                Projectile.timeLeft = 800;
                fadeOut = 0;
                trailPositions = new List<Vector2>();
                trailRotations = new List<float>();
                trailVelocities = new List<Vector2>();
                for (int i = 0; i < 110; i++)
                {
                    float angle = MathHelper.TwoPi * i / 95;
                    trailPositions.Add(Projectile.Center + new Vector2(5, 0).RotatedBy(angle));
                    trailRotations.Add(angle + MathHelper.PiOver2);
                    trailVelocities.Add(new Vector2(1, 0).RotatedBy(angle));
                }
                trailCurrentLength = CalculateLength();
            }
            else
            {
                if(timer < 90 && fixedSpeed < 5)
                {
                    fixedSpeed += 0.1f;
                }
                for (int i = 0; i < trailPositions.Count; i++)
                {
                    trailPositions[i] += fixedSpeed * new Vector2(1, 0).RotatedBy(trailRotations[i] - MathHelper.PiOver2);
                }

                for (int i = 0; i < trailPositions.Count; i++)
                {
                    if (i < trailPositions.Count - 1 && Vector2.Distance(trailPositions[i], trailPositions[i + 1]) > 20 && trailPositions.Count < trailPointLimit)
                    {
                        trailPositions.Insert(i + 1, (trailPositions[i] + trailPositions[i + 1]) / 2f);
                        trailRotations.Insert(i + 1, (trailRotations[i] + trailRotations[i + 1]) / 2f);
                        trailVelocities.Insert(i + 1, (trailVelocities[i] + trailVelocities[i + 1]) / 2f);
                    }
                }
            }
        }
        else
        {
            //A phase is 900 seconds long
            //Once that is over, stop adding new positions
            if (timer <= timerLimit)
            {
                if (!initialized)
                {
                    trailVelocities = new List<Vector2>();
                    Initialize();
                }

                if ((!HostEntityValid() || Projectile.timeLeft < 1f / deathSpeed || dying) && !noFadeOut)
                {
                    dying = true;
                    hostNPC = null;

                    deathProgress += deathSpeed;
                    if (deathProgress > 1)
                    {
                        deathProgress = 1;
                        Projectile.Kill();
                    }
                    fadeOut = 1f - deathProgress;
                }
                else
                {
                    Projectile.Center = HostEntity.Center;

                    //Don't add new trail segments if it has not travelled far enough
                    if (Vector2.Distance(lastPosition, HostEntity.Center) > 1f)
                    {
                        lastPosition = HostEntity.Center;
                        trailPositions.Add(HostEntity.Center);
                        trailRotations.Add(HostEntity.velocity.ToRotation());
                        trailVelocities.Add(Vector2.Zero);
                    }

                    if (trailPositions.Count > 2)
                    {
                        trailPositions[trailPositions.Count - 1] = HostEntity.Center;
                        trailRotations[trailRotations.Count - 1] = HostEntity.velocity.ToRotation();

                        trailCurrentLength = CalculateLength();

                        if (trailCurrentLength > trailMaxLength)
                        {
                            float shorteningDistance = trailCurrentLength - trailMaxLength;

                            while (shorteningDistance > Vector2.Distance(trailPositions[0], trailPositions[1]))
                            {
                                trailPositions.RemoveAt(0);
                                trailRotations.RemoveAt(0);
                                trailVelocities.RemoveAt(0);
                                trailCurrentLength = CalculateLength();
                                shorteningDistance = trailCurrentLength - trailMaxLength;
                            }
                            if (shorteningDistance < Vector2.Distance(trailPositions[0], trailPositions[1]))
                            {
                                Vector2 diff = trailPositions[1] - trailPositions[0];
                                float currentDistance = diff.Length();
                                float newDistance = currentDistance - shorteningDistance;
                                trailPositions[0] = trailPositions[1] - Vector2.Normalize(diff) * newDistance;
                                if (Vector2.Distance(trailPositions[0], trailPositions[1]) < 0.1f)
                                {
                                    trailPositions.RemoveAt(0);
                                    trailRotations.RemoveAt(0);
                                    trailVelocities.RemoveAt(0);
                                    trailCurrentLength = CalculateLength();
                                }
                            }
                        }
                    }

                    //This could be optimized to not require recomputing the length after each removal
                    while (trailPositions.Count > trailPointLimit)
                    {
                        trailPositions.RemoveAt(0);
                        trailRotations.RemoveAt(0);
                        trailVelocities.RemoveAt(0);
                        trailCurrentLength = CalculateLength();
                    }
                }
            }
            else
            {
                Player target = UsefulFunctions.GetClosestPlayer(Projectile.Center);
                if (timer < timerLimit + 30)
                {
                    if (target != null)
                    {
                        for (int i = 0; i < trailPositions.Count; i++)
                        {
                            trailVelocities[i] += UsefulFunctions.GenerateTargetingVector(trailPositions[i], target.Center, 0.5f);
                            trailPositions[i] += trailVelocities[i];
                        }
                    }
                }
                else
                {
                    //These *must* be done one then the other. Otherwise adding velocities in between adding new trail points can cause new trail points to be added infinitely.
                    for (int i = 0; i < trailPositions.Count; i++)
                    {
                        trailPositions[i] += trailVelocities[i];
                    }

                    for (int i = 0; i < trailPositions.Count; i++)
                    {
                        if (i < trailPositions.Count - 1 && Vector2.Distance(trailPositions[i], trailPositions[i + 1]) > 20 && trailPositions.Count < trailPointLimit)
                        {
                            trailPositions.Insert(i + 1, (trailPositions[i] + trailPositions[i + 1]) / 2f);
                            trailRotations.Insert(i + 1, (trailRotations[i] + trailRotations[i + 1]) / 2f);
                            trailVelocities.Insert(i + 1, (trailVelocities[i] + trailVelocities[i + 1]) / 2f);
                        }
                    }
                }
            }
        }
    }
    

    public override float CollisionWidthFunction(float progress)
    {
        return 25;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCs.Add(index);
    }

    bool PreSetTrail = false;
    Color trailColor = new Color(0.2f, 0.7f, 1f);
    float timeFactor = 0;
    public override void SetEffectParameters(Effect effect)
    {
        collisionFrequency = 2;
        visualizeTrail = false;
        collisionPadding = 8;
        collisionEndPadding = trailPositions.Count / 24;
        trailWidth = 25;

        timeFactor++;

        //Shifts its color slightly over time
        Vector3 hslColor = Main.rgbToHsl(Color.YellowGreen);
        hslColor.X += 0.03f * (float)Math.Cos(timeFactor / 25f);
        Color rgbColor = Main.hslToRgb(hslColor);

        effect.Parameters["noiseTexture"].SetValue(tsorcRevamp.tNoiseTexture3);
        effect.Parameters["fadeOut"].SetValue(0.85f);
        effect.Parameters["finalStand"].SetValue(FinalStandMode.ToInt());
        effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
        effect.Parameters["shaderColor"].SetValue(rgbColor.ToVector4());
        effect.Parameters["shaderColor2"].SetValue(new Color(0.2f, 0.7f, 1f).ToVector4());
        effect.Parameters["length"].SetValue(trailCurrentLength);
        effect.Parameters["WorldViewProjection"].SetValue(GetWorldViewProjectionMatrix());
    }

}