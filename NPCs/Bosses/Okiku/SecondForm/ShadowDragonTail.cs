using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.NPCs.Bosses.Okiku.SecondForm;

public class ShadowDragonTail : ModNPC
{
    public override void SetDefaults()
    {
        NPC.width = 12;
        NPC.height = 12;
        NPC.aiStyle = 6;
        NPC.damage = 80;
        NPC.defense = 20;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.lifeMax = 91000000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath8;
        NPC.knockBackResist = 0f;
        DrawOffsetY = 50;
        NPC.dontCountMe = true;
        bodyTypes = new int[] {
        ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(),
        ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonLegs>(), ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(),
        ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonLegs>(), ModContent.NPCType<ShadowDragonBody>(),
        ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonLegs>(),
        ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody>(),
        ModContent.NPCType<ShadowDragonLegs>(), ModContent.NPCType<ShadowDragonBody>(), ModContent.NPCType<ShadowDragonBody2>(), ModContent.NPCType<ShadowDragonBody3>()
        };
    }
    public static int[] bodyTypes;


    public override void SetStaticDefaults()
    {
        base.DisplayName.SetDefault("Shadow Dragon");
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }
    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }
    public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
    {
    }

    public override void AI()
    {
        tsorcRevampGlobalNPC.AIWorm(NPC, ModContent.NPCType<ShadowDragonHead>(), bodyTypes, ModContent.NPCType<ShadowDragonTail>(), 25, 0.8f, 16f, 0.33f, true, false, true, false, false);

        if (Main.rand.NextBool(3))
        {
            int dust = Dust.NewDust(new Vector2(base.NPC.position.X, base.NPC.position.Y), base.NPC.width, base.NPC.height, 62, 0f, 0f, 100, Color.White, 2f);
            Main.dust[dust].noGravity = true;
        }
    }
}
