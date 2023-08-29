﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.NPCs.Enemies.SuperHardMode.SerpentOfTheAbyss;

class SerpentOfTheAbyssTail : ModNPC
{

    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Serpent of the Abyss");
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }

    public override void SetDefaults()
    {
        NPC.netAlways = true;
        NPC.npcSlots = 1;
        NPC.width = 21;
        NPC.height = 14;
        NPC.aiStyle = 6;
        NPC.timeLeft = 750;
        NPC.damage = 120;
        NPC.defense = 228;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.lavaImmune = true;
        NPC.knockBackResist = 0;
        NPC.lifeMax = 50000;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.behindTiles = true;
        NPC.value = 500;

        bodyTypes = new int[33];
        int bodyID = ModContent.NPCType<SerpentOfTheAbyssBody>();
        for (int i = 0; i < 33; i++)
        {
            bodyTypes[i] = bodyID;
        }
    }
    int[] bodyTypes;

    public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
    {
        NPC.lifeMax = (int)(NPC.lifeMax / 2);
        NPC.damage = (int)(NPC.damage / 2);
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    public override void AI()
    {
        tsorcRevampGlobalNPC.AIWorm(NPC, ModContent.NPCType<SerpentOfTheAbyssHead>(), bodyTypes, ModContent.NPCType<SerpentOfTheAbyssTail>(), 35, .8f, 17, 0.25f, false, false, false, true, true);
    }
}
