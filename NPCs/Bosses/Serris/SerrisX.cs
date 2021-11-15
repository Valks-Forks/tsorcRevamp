using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Items;

namespace tsorcRevamp.NPCs.Bosses.Serris
{
	[AutoloadBossHead]
	class SerrisX : ModNPC
	{
		public override void SetDefaults()
		{
			Main.npcFrameCount[npc.type] = 16;
			drawOffsetY = 10;
			npc.npcSlots = 5;
			npc.width = 70;
			npc.height = 70;
			npc.aiStyle = 2;
			npc.timeLeft = 22500;
			npc.damage = 150;
			npc.defense = 9999;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.lifeMax = 10000;
			npc.scale = 1;
			npc.knockBackResist = 0;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.boss = true;
			npc.value = 500000;

			npc.buffImmune[BuffID.Confused] = true;
			bossBag = ModContent.ItemType<Items.BossBags.SerrisBag>();

			//If one already exists, don't add text to the others despawnhandler (so it doesn't show duplicate messages if you die)
			if (NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Serris.SerrisHead>()) || NPC.CountNPCS(ModContent.NPCType<NPCs.Bosses.Serris.SerrisX>()) > 1)
			{
				despawnHandler = new NPCDespawnHandler(DustID.Firework_Blue);
			}
			else
			{
				despawnHandler = new NPCDespawnHandler("Serris returns to the depths of its temple...", Color.Cyan, DustID.Firework_Blue);
			}
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Serris-X");
		}

		int plasmaOrbDamage = 70;
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.damage = (int)(npc.damage * 1.3 / tsorcRevampGlobalNPC.expertScale);
			npc.lifeMax = npc.lifeMax / 2;
			plasmaOrbDamage = plasmaOrbDamage / 2;
		}

		bool immuneFlash = true;
		bool TimeLock = false;
		//Cooldown between projectiles in frames
		int projCooldown = 0;
		bool projRotate = false;
		bool extraProjs = false;
		NPCDespawnHandler despawnHandler;
		int attack = 0;
		int secondaryCooldown = 900;
		float attackCounter = 0;
		int slowdownCounter = 100;
		public override void AI()
		{
			despawnHandler.TargetAndDespawn(npc.whoAmI);

			//Health stuff. This would go into OnHitByWhatever or ModifyHitByWhatever, but surprise: Those are fucky in multiplayer! Isn't learning fun.
			if((npc.life % 1000) != 0 && npc.life > 1)
            {
				npc.life -= npc.life % 1000;
				if(npc.life <= 0)
                {
					npc.life = 1;
                }

				TimeLock = false;
				npc.ai[0] = 2;
				Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0);
				npc.netUpdate = true;
			}

			if(attack == 0) {
				if (secondaryCooldown > 0) {
					secondaryCooldown--;
				}
				else
                {
					attack = 1;
					secondaryCooldown = 300;
				}
				if (projCooldown > 0)
				{
					projCooldown--;
				}
				else
				{
					projCooldown = 60;
					float speed = 5f;
					Vector2 vector8 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height / 2));
					float distanceFactor = Vector2.Distance(vector8, Main.player[npc.target].position) / speed;
					float speedX = ((Main.player[npc.target].position.X + (Main.player[npc.target].width * 0.5f)) - vector8.X) / distanceFactor;
					float speedY = ((Main.player[npc.target].position.Y + (Main.player[npc.target].height * 0.5f)) - vector8.Y) / distanceFactor;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(vector8.X, vector8.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
					}
					if (extraProjs)
					{
						if (projRotate )
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(vector8.X, vector8.Y, speed, speed, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X, vector8.Y, -speed, speed, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X, vector8.Y, speed, -speed, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X, vector8.Y, -speed, -speed, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
							}
							projRotate = false;
						}
						else
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(vector8.X, vector8.Y, speed, 0, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X, vector8.Y, -speed, 0, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X, vector8.Y, 0, speed, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X, vector8.Y, 0, -speed, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
							}
							projRotate = true;
						}
						extraProjs = false;
					}
					else
					{
						extraProjs = true;
					}
				}
			} 
			if(attack == 1)
            {				
				if (projCooldown > 0 || secondaryCooldown > 0)
				{					
					for (int j = 0; j < 5; j++)
					{
						Vector2 dir = Main.rand.NextVector2CircularEdge(250, 250);
						Vector2 dustPos = npc.Center + dir;
						Vector2 dustVel = UsefulFunctions.GenerateTargetingVector(dustPos, npc.Center, 12);
						Dust.NewDustPerfect(dustPos, DustID.Firework_Blue, dustVel, 200).noGravity = true;
					}
					projCooldown--;
					secondaryCooldown--;
				}
				else
                {
					npc.velocity.X *= slowdownCounter/100;
					npc.velocity.Y *= slowdownCounter/100;

					if(slowdownCounter > 0)
                    {
						slowdownCounter--;
						//While it slows down, add some fun in
						if (Main.rand.Next(2) == 0)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(npc.Center.X, npc.Center.Y, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10), ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
							}
						}
					}

					if(slowdownCounter == 0 && ((Math.Abs(npc.velocity.X) < .01) && Math.Abs(npc.velocity.Y) < .01))
                    {
						float speed = 10f;
						int spread = 30;
						Vector2 vector8 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height / 2));
						float distanceFactor = Vector2.Distance(vector8, Main.player[npc.target].position) / speed;
						float speedX = ((Main.player[npc.target].position.X + (Main.player[npc.target].width * 0.5f)) - vector8.X) / distanceFactor;
						float speedY = ((Main.player[npc.target].position.Y + (Main.player[npc.target].height * 0.5f)) - vector8.Y) / distanceFactor;

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(vector8.X, vector8.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
						}
						if (projRotate)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(vector8.X - spread, vector8.Y - spread, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X + spread, vector8.Y - spread, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X - spread, vector8.Y + spread, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X + spread, vector8.Y + spread, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
							}
							projRotate = false;
						}
						else
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(vector8.X - spread, vector8.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X + spread, vector8.Y, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X, vector8.Y - spread, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
								Projectile.NewProjectile(vector8.X, vector8.Y + spread, speedX, speedY, ModContent.ProjectileType<Projectiles.Enemy.EnemyPlasmaOrb>(), plasmaOrbDamage, 0f, Main.myPlayer);
							}
						projRotate = true;
						}
						

						attackCounter++;
						//Every 3 shots, pause for 4 seconds
						if((attackCounter % 5) == 0)
                        {
							secondaryCooldown = 240;
							slowdownCounter = 100;
						} else
                        {
							secondaryCooldown = 20;
                        }
						//After doing this 3 times, change phases
						if (attackCounter >= 15)
						{
							attackCounter = 0;
							attack = 0;
							secondaryCooldown = 900;
						}
					}
				}
			}
			npc.ai[0]++;
			npc.position += npc.velocity * 1.5f;
			if (npc.ai[0] <= 1 || npc.ai[0] >= 300)
			{
				immuneFlash = false;
				npc.dontTakeDamage = false;
				TimeLock = true;
			}
			else if (npc.ai[0] >= 2)
			{
				immuneFlash = true;
				npc.dontTakeDamage = true;
			}
			if (TimeLock)
			{
				npc.ai[0] = 0;
			}
		}

		
       
		public override bool CheckActive()
		{
			return false;
		}

        public override bool PreNPCLoot()
        {
			Vector2 vector8 = new Vector2(npc.position.X + (npc.width * 0.5f), npc.position.Y + (npc.height / 2));
			Gore.NewGore(vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Serris-X Gore 1"), 1f);
			Gore.NewGore(vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Serris-X Gore 2"), 1f);
			Gore.NewGore(vector8, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), mod.GetGoreSlot("Gores/Serris-X Gore 3"), 1f);
			for (int num36 = 0; num36 < 70; num36++)
			{
				int dust = Dust.NewDust(npc.position, (int)(npc.width), (int)(npc.height), DustID.Firework_Blue, Main.rand.Next(-15, 15), Main.rand.Next(-15, 15), 100, new Color(), 9f);
				Main.dust[dust].noGravity = true;
			}

			if (Main.player[npc.target].active)
			{
				npc.Center = Main.player[npc.target].Center;
			}
			return true;
        }


        public override void NPCLoot()
		{		
			Main.NewText("Serris falls...", Color.Cyan);

			if (!(NPC.AnyNPCs(ModContent.NPCType<NPCs.Bosses.Serris.SerrisHead>()) || (NPC.CountNPCS(ModContent.NPCType<NPCs.Bosses.Serris.SerrisX>()) > 1))) {

				if (Main.expertMode)
				{
					npc.DropBossBags();
				}
				else
				{
					Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Potions.DemonDrugPotion>(), 3 + Main.rand.Next(4));
					Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Potions.ArmorDrugPotion>(), 3 + Main.rand.Next(4));
					Item.NewItem(npc.getRect(), ModContent.ItemType<Items.GuardianSoul>(), 1);
					Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Magic.BarrierTome>(), 1);
					if (!tsorcRevampWorld.Slain.ContainsKey(npc.type))
					{
						Item.NewItem(npc.getRect(), ModContent.ItemType<Items.DarkSoul>(), 50000);
					}
				}
			}
		}
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.GreaterHealingPotion;
		}
		public override void FindFrame(int currentFrame)
		{
			int num = 1;
			if (!Main.dedServ)
			{
				num = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
			}
			npc.frameCounter += 1.0;
			if (immuneFlash)
			{
				if (npc.frameCounter >= 0 && npc.frameCounter < 5)
				{
					npc.frame.Y = num * 8;
				}
				if (npc.frameCounter >= 5 && npc.frameCounter < 10)
				{
					npc.frame.Y = num * 9;
				}
				if (npc.frameCounter >= 10 && npc.frameCounter < 15)
				{
					npc.frame.Y = num * 10;
				}
				if (npc.frameCounter >= 15 && npc.frameCounter < 20)
				{
					npc.frame.Y = num * 11;
				}
				if (npc.frameCounter >= 20 && npc.frameCounter < 25)
				{
					npc.frame.Y = num * 12;
				}
				if (npc.frameCounter >= 25 && npc.frameCounter < 30)
				{
					npc.frame.Y = num * 13;
				}
				if (npc.frameCounter >= 30 && npc.frameCounter < 35)
				{
					npc.frame.Y = num * 14;
				}
				if (npc.frameCounter >= 35 && npc.frameCounter < 40)
				{
					npc.frame.Y = num * 15;
				}
				if (npc.frameCounter >= 40)
				{
					npc.frameCounter = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 0 && npc.frameCounter < 5)
				{
					npc.frame.Y = 0;
				}
				if (npc.frameCounter >= 5 && npc.frameCounter < 10)
				{
					npc.frame.Y = num;
				}
				if (npc.frameCounter >= 10 && npc.frameCounter < 15)
				{
					npc.frame.Y = num * 2;
				}
				if (npc.frameCounter >= 15 && npc.frameCounter < 20)
				{
					npc.frame.Y = num * 3;
				}
				if (npc.frameCounter >= 20 && npc.frameCounter < 25)
				{
					npc.frame.Y = num * 4;
				}
				if (npc.frameCounter >= 25 && npc.frameCounter < 30)
				{
					npc.frame.Y = num * 5;
				}
				if (npc.frameCounter >= 30 && npc.frameCounter < 35)
				{
					npc.frame.Y = num * 6;
				}
				if (npc.frameCounter >= 35 && npc.frameCounter < 40)
				{
					npc.frame.Y = num * 7;
				}
				if (npc.frameCounter >= 45)
				{
					npc.frameCounter = 0;
				}
			}
		}
	}
}