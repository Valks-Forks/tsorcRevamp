using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Buffs.Runeterra.Summon;

	public class SunburnDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			Player player = Main.player[Main.myPlayer];
			npc.GetGlobalNPC<SunburnDebuffNPC>().Sunburnt = true;
        if (Main.GameUpdateCount % 5 == 0)
        {
            Dust.NewDust(npc.Center, 20, 20, DustID.GoldFlame);
        }
    }
	}

	public class SunburnDebuffNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool Sunburnt;

		public override void ResetEffects(NPC npc)
		{
			Sunburnt = false;
		}
		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
        Player player = Main.player[Main.myPlayer];
        int DoTPerS = (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(110);
        if (Sunburnt)
			{
				npc.lifeRegen -= DoTPerS * 2;
				damage += DoTPerS;
			}
    }
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (Sunburnt && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.IsAWhip[projectile.type]))
        {
            if (Main.rand.NextBool(100 / (int)MathF.Round(Main.player[Main.myPlayer].GetTotalCritChance(DamageClass.Generic) / 5f)))
            {
					crit = true;
				}
			}
		}
	}