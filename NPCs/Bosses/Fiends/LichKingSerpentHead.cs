using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace tsorcRevamp.NPCs.Bosses.Fiends;

class LichKingSerpentHead : ModNPC
{
    public override void SetDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        AnimationType = 10;
        NPC.netAlways = true;
        NPC.npcSlots = 3;
        NPC.width = 40;
        NPC.height = 40;
        NPC.boss = true;
        NPC.aiStyle = 6;
        NPC.defense = 10;
        NPC.timeLeft = 22500;
        NPC.damage = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.Item119;
        NPC.lifeMax = 120000;
        NPC.knockBackResist = 0;
        NPC.lavaImmune = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.behindTiles = true;
        NPC.value = 40000;
        despawnHandler = new NPCDespawnHandler(DustID.GreenFairy);
        DrawOffsetY = 15;

        NPC.buffImmune[BuffID.Poisoned] = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.buffImmune[BuffID.CursedInferno] = true;
        NPC.buffImmune[BuffID.OnFire] = true;

        bodyTypes = new int[43];
        int bodyID = ModContent.NPCType<LichKingSerpentBody>();
        for (int i = 0; i < 43; i++)
        {
            bodyTypes[i] = bodyID;
        }
    }
    int[] bodyTypes;

    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Lich King Serpent");
    }

    public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
    {
        NPC.damage = (int)(NPC.damage * 1.3 / 2);
        NPC.defense = NPC.defense += 12;
    }

    NPCDespawnHandler despawnHandler;
    public override void AI()
    {
        despawnHandler.TargetAndDespawn(NPC.whoAmI);

        tsorcRevampGlobalNPC.AIWorm(NPC, ModContent.NPCType<LichKingSerpentHead>(), bodyTypes, ModContent.NPCType<LichKingSerpentTail>(), 45, .8f, 22, 0.25f, false, false, false, true, true);
    }

    private static int ClosestSegment(NPC head, params int[] segmentIDs)
    {
        List<int> segmentIDList = new List<int>(segmentIDs);
        Vector2 targetPos = Main.player[head.target].Center;
        int closestSegment = head.whoAmI; //head is default, updates later
        float minDist = 1000000f; //arbitrarily large, updates later
        for (int i = 0; i < Main.npc.Length; i++)
        { //iterate through every NPC
            NPC npc = Main.npc[i];
            if (npc != null && npc.active && segmentIDList.Contains(npc.type))
            { //if the npc is part of the wyvern
                float targetDist = (npc.Center - targetPos).Length();
                if (targetDist < minDist)
                { //if we're closer than the previously closer segment (or closer than 1,000,000 if it's the first iteration, so always)
                    minDist = targetDist; //update minDist. future iterations will compare against the updated value
                    closestSegment = i; //and set closestSegment to the whoAmI of the closest segment
                }
            }
        }
        return closestSegment; //the whoAmI of the closest segment
    }

    public override bool PreKill() {
        int closestSegmentID = ClosestSegment(NPC, ModContent.NPCType<LichKingSerpentBody>(), ModContent.NPCType<LichKingSerpentTail>());
        NPC.position = Main.npc[closestSegmentID].position; //teleport the head to the location of the closest segment before running npcloot
        return base.PreKill();
    }
    public override bool CheckActive()
    {
        return false;
    }
    public override void BossLoot(ref string name, ref int potionType)
    {
        potionType = ItemID.SuperHealingPotion;
    }
    public override void OnKill()
    {
        Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
        if (NPC.life <= 0)
        {
            if (!Main.dedServ)
            {
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Head Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Body Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Body Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Body Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Body Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Body Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Body Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Body Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Body Gore").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), Mod.Find<ModGore>("Lich King Serpent Tail Gore").Type, 1f);
            }
        }
        if (!Main.expertMode)
        {
            if (!tsorcRevampWorld.NewSlain.ContainsKey(new NPCDefinition(NPC.type)))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.DarkSoul>(), 2000);
            }
        }
    }
}