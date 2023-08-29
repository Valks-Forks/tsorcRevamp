﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.NPCs.Bosses.WyvernMage;

class MechaDragonBody2 : ModNPC
{

    public int Timer = -1000;

    public override void SetDefaults()
    {
        NPC.netAlways = true;
        NPC.boss = true;
        NPC.npcSlots = 1;
        NPC.aiStyle = 6;
        NPC.width = 45;
        NPC.height = 45;
        NPC.knockBackResist = 0f;
        NPC.timeLeft = 750;
        NPC.damage = 70;
        NPC.defense = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath10;
        NPC.lifeMax = 91000000;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.behindTiles = true;
        NPC.value = 25000;
        NPC.buffImmune[BuffID.Poisoned] = true;
        NPC.buffImmune[BuffID.OnFire] = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.buffImmune[BuffID.CursedInferno] = true;
        bodyTypes = new int[] { ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonLegs>(), ModContent.NPCType<MechaDragonBody>(),
            ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonLegs>(), ModContent.NPCType<MechaDragonBody>(),
            ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonLegs>(), ModContent.NPCType<MechaDragonBody>(),
            ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonBody>(), ModContent.NPCType<MechaDragonLegs>(), ModContent.NPCType<MechaDragonBody>(),
            ModContent.NPCType<MechaDragonBody2>(), ModContent.NPCType<MechaDragonBody3>() };

    }
    public static int[] bodyTypes;

    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Wyvern Mage Disciple");
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }
    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    int CrystalFireDamage = 35;
    public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
    {
        CrystalFireDamage /= 2;
    }

    public override void AI()
    {

        //Generic Worm Part Code:
        tsorcRevampGlobalNPC.AIWorm(NPC, ModContent.NPCType<MechaDragonHead>(), bodyTypes, ModContent.NPCType<MechaDragonTail>(), 23, -1f, 12f, 0.13f, true, false);

        //Code unique to this body part:
        Timer++;
        if (!Main.npc[(int)NPC.ai[1]].active)
        {
            NPC.life = 0;
            for (int num36 = 0; num36 < 50; num36++)
            {
                Color color = new Color();
                int dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y - 10), NPC.width, NPC.height, 6, Main.rand.Next(-20, 20) * 2, Main.rand.Next(-20, 20) * 2, 100, color, 4f);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y - 10), NPC.width, NPC.height, 6, Main.rand.Next(-20, 20) * 2, Main.rand.Next(-20, 20) * 2, 100, color, 4f);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y - 10), NPC.width, NPC.height, 6, Main.rand.Next(-20, 20) * 2, Main.rand.Next(-20, 20) * 2, 100, color, 4f);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y - 10), NPC.width, NPC.height, 6, 0, 0, 100, Color.White, 2.0f);
                Main.dust[dust].noGravity = true;
            }
            NPC.HitEffect(0, 10.0);
            NPC.active = false;
        }

        if (Timer >= 0)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float num48 = 7f;
                Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                float rotation = (float)Math.Atan2(vector8.Y - (Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)), vector8.X - (Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)));
                rotation += Main.rand.Next(-50, 50) / 100; 
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, (float)((Math.Cos(rotation) * num48) * -1), (float)((Math.Sin(rotation) * num48) * -1), ModContent.ProjectileType<Projectiles.Enemy.CrystalFire>(), CrystalFireDamage, 0f, Main.myPlayer);
                }
                Timer = -1200 - Main.rand.Next(1200);
            }
            //npc.netUpdate=true; //new
        }

        if (Main.rand.NextBool(2))
        {
            //int dust = Dust.NewDust(new Vector2((float)npc.position.X, (float)npc.position.Y), npc.width, npc.height, 6, 0, 0, 100, Color.White, 2.0f);
            int dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, Type: DustID.WhiteTorch, 0, 0, 100, Color.White, 2.0f);
            //Main.dust[dust].noGravity = true;
        }
    }

    public override bool CheckActive()
    {
        return false;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 origin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
        Color alpha = Color.White;
        SpriteEffects effects = SpriteEffects.None;
        if (NPC.spriteDirection == 1)
        {
            effects = SpriteEffects.FlipHorizontally;
        }
        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, new Vector2(NPC.position.X - Main.screenPosition.X + (float)(NPC.width / 2) - (float)TextureAssets.Npc[NPC.type].Value.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + (float)NPC.height - (float)TextureAssets.Npc[NPC.type].Value.Height * NPC.scale / (float)Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale + 56f), NPC.frame, alpha, NPC.rotation, origin, NPC.scale, effects, 0f);
        NPC.alpha = 255;
        return true;
    }
}
