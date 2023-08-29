using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using tsorcRevamp.Buffs.Runeterra.Ranged;

namespace tsorcRevamp.Projectiles.Ranged.Runeterra;

	public class NuclearMushroomExplosion: ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nuclear Mushroom Explosion");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        Main.projFrames[Projectile.type] = 16;
    }

		public override void SetDefaults()
		{
			Projectile.width = 500;
			Projectile.height = 500;

			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 80;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 8;
		}

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        Projectile.CritChance = 100 + owner.GetWeaponCrit(owner.HeldItem);
        Visuals();
    }
    public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
    {
			target.AddBuff(ModContent.BuffType<IrradiatedByShroomDebuff>(), 600);
    }
    private void Visuals()
    {
        int frameSpeed = 5;

        Projectile.frameCounter++;

        if (Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }

        // Some visuals here
        Lighting.AddLight(Projectile.Center, Color.GreenYellow.ToVector3() * 10f);
    }
}