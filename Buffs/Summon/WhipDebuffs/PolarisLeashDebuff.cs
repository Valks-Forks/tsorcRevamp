using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Buffs.Summon.WhipDebuffs;

	public class PolarisLeashDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
			// Other mods may check it for different purposes.
			BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<PolarisLeashDebuffNPC>().markedByPolarisLeash = true;
		}
	}

	public class PolarisLeashDebuffNPC : GlobalNPC
	{
		// This is required to store information on entities that isn't shared between them.
		public override bool InstancePerEntity => true;

		public bool markedByPolarisLeash;

		public override void ResetEffects(NPC npc)
		{
			markedByPolarisLeash = false;
		}

		// TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			int whipDamage = (int)(Main.player[projectile.owner].GetTotalDamage(DamageClass.SummonMeleeSpeed).ApplyTo((int)(66f * 0.5f))); //66 is the base dmg of Polaris Leash
        Vector2 starvector1 = new Vector2(-640, -800) + npc.Center;
        Vector2 starvector2 = new Vector2(640, -800) + npc.Center;
        Vector2 starvector3 = new Vector2(0, -800) + npc.Center;
        Vector2 starmove1 = new Vector2(+32, 40);
        Vector2 starmove2 = new Vector2(-32, 40);
        Vector2 starmove3 = new Vector2(0, 40);
        // Only player attacks should benefit from this buff, hence the NPC and trap checks.
        if (markedByPolarisLeash && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
			{
				if(Main.rand.NextBool(3))
				{
                Projectile.NewProjectile(Projectile.GetSource_None(), starvector1, starmove1, ModContent.ProjectileType<Projectiles.Summon.Whips.PolarisLeashFallingStar>(), whipDamage, 1f, Main.myPlayer);
            } else if (Main.rand.NextBool(3))
				{
                Projectile.NewProjectile(Projectile.GetSource_None(), starvector2, starmove2, ModContent.ProjectileType<Projectiles.Summon.Whips.PolarisLeashFallingStar>(), whipDamage, 1f, Main.myPlayer);
            } else
				{
                Projectile.NewProjectile(Projectile.GetSource_None(), starvector3, starmove3, ModContent.ProjectileType<Projectiles.Summon.Whips.PolarisLeashFallingStar>(), whipDamage, 1f, Main.myPlayer);
            }
				damage += 6;
			}
		}
	}
