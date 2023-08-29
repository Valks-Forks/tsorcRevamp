using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Projectiles.Summon;

	public class ShatteredReflectionProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true; // This is necessary for right-click targeting
			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
		}

		public sealed override void SetDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
			Projectile.width = 40;
			Projectile.height = 50;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 2; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 15;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public override bool MinionContactDamage()
		{
			return false;
		}

		bool indexSet = false;
		List<float> foundIndicies = new List<float>();
		int timer = 0;
		public override void AI()
		{
			timer++;
			Player owner = Main.player[Projectile.owner];

			if (!indexSet)
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].whoAmI != Projectile.whoAmI)
					{
						foundIndicies.Add(Main.projectile[i].ai[0]);
					}
				}

				for (int i = 0; i < 6; i++)
				{
					if (foundIndicies.Contains(i))
					{
						continue;
					}
					else
					{
						Projectile.ai[0] = i;
						break;
					}
				}
				indexSet = true;
			}

			CheckActive(owner);

			float rotationTime = 820;
			float percentage = MathHelper.TwoPi * ((Main.GameUpdateCount % rotationTime) / rotationTime);

			float radius = 300;
			float offset = 0;
			if (timer < 60)
			{
				radius *= timer / 60f;
				offset = 1 - timer / 60f;
			}
			if (Projectile.timeLeft < 60)
			{
				radius *= (Projectile.timeLeft) / 60f;
				offset = 1 - (Projectile.timeLeft) / 60f;
			}
			int ownedCount = owner.ownedProjectileCounts[ModContent.ProjectileType<ShatteredReflectionProjectile>()];
			Vector2 target = owner.Center + new Vector2(radius, 0).RotatedBy(offset + percentage + MathHelper.TwoPi * Projectile.ai[0] / ownedCount);
			UsefulFunctions.SmoothHoming(Projectile, target, 0.5f, 20);

			//Just skip the smoothing if it's in the despawn animation
			if(Projectile.timeLeft < 60)
        {
				Projectile.Center = target;
        }
			

			if (owner.whoAmI == Main.myPlayer && Main.GameUpdateCount % 60 == Projectile.ai[0] * 60f / ownedCount)
        {
				int? closest = UsefulFunctions.GetClosestEnemyNPC(Projectile.Center);
            if (closest.HasValue && (Main.npc[closest.Value].type != NPCID.TargetDummy || Main.npc[closest.Value].Distance(Projectile.Center) < 2000))
            {
					Vector2 velocity = UsefulFunctions.GenerateTargetingVector(Projectile.Center, Main.npc[closest.Value].Center, 3);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SRBlackFire>(), Projectile.damage, 0, Main.myPlayer, 1);
				}
        }
		}		

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner)
		{
			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(ModContent.BuffType<Buffs.Summon.ShatteredReflectionBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<Buffs.Summon.ShatteredReflectionBuff>()))
			{
				Projectile.timeLeft = 60;
			}

			return true;
		}

		public static Effect attraidiesEffect;
		float effectTimer;
    public override bool PreDraw(ref Color lightColor)
    {
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Color color = Color.Purple * 3;
			Lighting.AddLight(Projectile.Center, color.ToVector3());
			Rectangle baseRectangle = new Rectangle(0, 0, 200, 200);
			Vector2 baseOrigin = baseRectangle.Size() / 2f;
			effectTimer++;

			//Apply the shader, caching it as well
			if (attraidiesEffect == null)
			{
				attraidiesEffect = ModContent.Request<Effect>("tsorcRevamp/Effects/AttraidiesAura", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			}

			//Pass relevant data to the shader via these parameters
			attraidiesEffect.Parameters["textureSize"].SetValue(tsorcRevamp.tNoiseTexture1.Width);
			attraidiesEffect.Parameters["effectSize"].SetValue(baseRectangle.Size());

			attraidiesEffect.Parameters["effectColor1"].SetValue(UsefulFunctions.ShiftColor(color, effectTimer / 25f).ToVector4());
			attraidiesEffect.Parameters["effectColor2"].SetValue(UsefulFunctions.ShiftColor(color, effectTimer / 25f).ToVector4());
			attraidiesEffect.Parameters["ringProgress"].SetValue(0.1f);
			attraidiesEffect.Parameters["fadePercent"].SetValue(0);
			attraidiesEffect.Parameters["scaleFactor"].SetValue(.5f * 150);
			attraidiesEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.05f);
			attraidiesEffect.Parameters["colorSplitAngle"].SetValue(MathHelper.TwoPi);

			//Apply the shader
			attraidiesEffect.CurrentTechnique.Passes[0].Apply();

			Main.EntitySpriteDraw(tsorcRevamp.tNoiseTexture1, Projectile.Center - Main.screenPosition, baseRectangle, Color.White, MathHelper.TwoPi, baseOrigin, 1, SpriteEffects.None, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
}