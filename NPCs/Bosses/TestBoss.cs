using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.NPCs.Bosses;

class TestBoss : ModNPC
{
    public override void SetDefaults()
    {

        Main.npcFrameCount[NPC.type] = 6;
        NPC.npcSlots = 10;
        NPC.aiStyle = 0;
        NPC.width = 80;
        NPC.height = 100;
        NPC.damage = 1;
        NPC.defense = 10;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath6;
        NPC.lifeMax = Int32.MaxValue;
        NPC.friendly = false;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0;
        NPC.value = 1;

    }


    public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
    {
        NPC.lifeMax = Int32.MaxValue / 10;
    }

    Stopwatch thisWatch;
    int watchTimer = 0;
    float damageCounter;
    float lastTimer = 1;
    public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
    {
        if(damageCounter == 0)
        {
            thisWatch = new Stopwatch();
            thisWatch.Start();
        }

        watchTimer = 60;
        damageCounter += damage;
        if (crit)
        {
            damageCounter += damage;
        }
        lastTimer = (float)thisWatch.Elapsed.TotalSeconds;
    }

    public override void AI()
    {
        NPC.defense = 0;
        if (damageCounter > 0)
        {
            UsefulFunctions.BroadcastText("DPS: " + damageCounter / thisWatch.Elapsed.TotalSeconds);
        }
        if (watchTimer == 1)
        {
            UsefulFunctions.BroadcastText("DPS measuring ended. DPS at the final hit: " + damageCounter / lastTimer);
            damageCounter = 0;
        }
        if (watchTimer > 0)
        {
            watchTimer--;
        }           
    }



    public override void OnKill()
    {

    }

    public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
    {
        if (item.type == ItemID.WoodenHammer)
        {
            NPC.life = 0;
        }

        if (damageCounter == 0)
        {
            thisWatch = new Stopwatch();
            thisWatch.Start();
        }

        watchTimer = 120;
        damageCounter += damage;
        lastTimer = (float)thisWatch.Elapsed.TotalSeconds;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return true;
    }
}