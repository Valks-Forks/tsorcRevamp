﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.NPCs.Enemies.JungleWyvernJuvenile;

class JungleWyvernJuvenileLegs : ModNPC
{
    public override void SetDefaults()
    {
        NPC.netAlways = true;
        NPC.npcSlots = 1;
        NPC.aiStyle = 6;
        NPC.width = 30;
        NPC.height = 30;
        NPC.knockBackResist = 0f;
        NPC.timeLeft = 1750;
        NPC.damage = 38;
        NPC.defense = 10;
        NPC.HitSound = SoundID.NPCHit7;
        NPC.DeathSound = SoundID.NPCDeath8;
        NPC.lifeMax = 60000000;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = 1500;
        NPC.scale = 0.7f;
        NPC.buffImmune[BuffID.Poisoned] = true;
        NPC.buffImmune[BuffID.OnFire] = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.buffImmune[BuffID.CursedInferno] = true;
    }

    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Jungle Wyvern Juvenile");
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    public override void AI()
    {

        if (!Main.npc[(int)NPC.ai[1]].active)
        {
            NPC.life = 0;
            NPC.HitEffect(0, 10.0);
            OnKill();
            for (int num36 = 0; num36 < 10; num36++)
            {
                Color color = new Color();
                int dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, 62, Main.rand.Next(-20, 20), Main.rand.Next(-20, 20), 100, color, 5f);
                Main.dust[dust].noGravity = false;
                dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, 62, Main.rand.Next(-20, 20), Main.rand.Next(-20, 20), 100, color, 3f);
                Main.dust[dust].noGravity = false;
                dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, 54, Main.rand.Next(-20, 20), Main.rand.Next(-20, 20), 100, color, 3f);
                Main.dust[dust].noGravity = false;
                dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, 62, 0, 0, 100, Color.White, 5f);
                Main.dust[dust].noGravity = true;
                //npc.netUpdate = true; //new
            }

            NPC.active = false;
        }
        if (NPC.position.X > Main.npc[(int)NPC.ai[1]].position.X)
        {
            NPC.spriteDirection = 1;
        }
        if (NPC.position.X < Main.npc[(int)NPC.ai[1]].position.X)
        {
            NPC.spriteDirection = -1;
        }
        if (Main.rand.NextBool(2))
        {
            int dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y + 10), NPC.width, NPC.height, 62, 0, 0, 100, Color.White, 1f);
            Main.dust[dust].noGravity = true;
        }
    }
    public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
    {
        damage *= 2;
        base.OnHitByItem(player, item, damage, knockback, crit);
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
        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, new Vector2(NPC.position.X - Main.screenPosition.X + (float)(NPC.width / 2) - (float)TextureAssets.Npc[NPC.type].Value.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + (float)NPC.height - (float)TextureAssets.Npc[NPC.type].Value.Height * NPC.scale / (float)Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale + 36f), NPC.frame, alpha, NPC.rotation, origin, NPC.scale, effects, 0f);
        NPC.alpha = 255;
        return true;
    }
}
