using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Projectiles;
using tsorcRevamp.Projectiles.Enemy.DarkCloud;
using tsorcRevamp.Buffs.Debuffs;

namespace tsorcRevamp.NPCs.Bosses.SuperHardMode;

[AutoloadBossHead]
class DarkCloud : ModNPC
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailCacheLength[NPC.type] = (int)TRAIL_LENGTH;    //The length of old position to be recorded
        NPCID.Sets.TrailingMode[NPC.type] = 1;
    }


    public override void SetDefaults()
    {
        NPC.npcSlots = 10;
        Main.npcFrameCount[NPC.type] = 16;
        AnimationType = 28;
        NPC.aiStyle = 3;
        NPC.height = 40;
        NPC.width = 20;
        Music = 12;
        NPC.damage = 200;
        NPC.defense = 80;
        NPC.lifeMax = 300000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 1500000;
        NPC.knockBackResist = 0f;
        NPC.boss = true;

        NPC.buffImmune[BuffID.Poisoned] = true;
        NPC.buffImmune[BuffID.OnFire] = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.buffImmune[BuffID.CursedInferno] = true;
        despawnHandler = new NPCDespawnHandler("You are subsumed by your shadow...", Color.Blue, DustID.ShadowbeamStaff);
    }

    #region Damage variables
    const float TRAIL_LENGTH = 12;

    public static int meteorDamage = 17;
    public static int deathBallDamage = 75;
    public static int poisonStrikeDamage = 46;
    public static int holdBallDamage = 35;
    public static int dragoonLanceDamage = 68;
    public static int armageddonDamage = 65;
    public static int gravityBallDamage = 35;
    public static int crazedPurpleCrushDamage = 40;
    public static int shadowShotDamage = 40;
    public static int iceStormDamage = 33;
    public static int darkArrowDamage = 45;
    public static int stormWaveDamage = 95;

    public static int divineSparkDamage = 75;
    public static int darkFlowDamage = 50;
    public static int antiMatDamage = 100;
    public static int darkSlashDamage = 25; //This one gets x16'd
    public static int swordDamage = 50;
    public static int freezeBoltDamage = 60;
    public static int confinedBlastDamage = 200; //Very high because it isn't compensating for doubling/quadrupling, and is very easy to dodge
    public static int arrowRainDamage = 50;
    #endregion

    public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
    {
        NPC.damage = 200;
    }

    #region First Phase Vars
    float comboDamage = 0;
    bool breakCombo = false;
    float customAi1;
    int boredTimer = 0;
    int tBored = 1;//increasing this increases how long it take for the NP to get bored
    int boredResetT = 0;
    int bReset = 50;//increasing this will increase how long an NPC "gives up" before coming back to try again.
    int chargeDamage = 0;
    bool chargeDamageFlag = false;

    float customspawn1;
    float customspawn2;
    float customspawn3;
    #endregion

    //If this is set to anything but -1, the boss will *only* use that attack ID
    readonly int testAttack = -1;
    bool firstPhase = true;
    bool changingPhases = false;

    //The next warp point in the current attack. It gets calculated before it's used so it has time to get synced first
    Vector2 nextWarpPoint;

    //The first warp point of the *next* attack. It is only used once per attack, at the start. Whenever it's used, a new one is calculated immediately to give it time to sync.
    Vector2 preSelectedWarpPoint;

    float phaseChangeCounter = 0;
    DarkCloudMove CurrentMove;
    List<DarkCloudMove> ActiveMoveList;
    List<DarkCloudMove> DefaultList;

    public int NextAttackMode
    {
        get => (int)NPC.ai[0];
        set => NPC.ai[0] = value;
    }
    public float AttackModeCounter
    {
        get => NPC.ai[1];
        set => NPC.ai[1] = value;
    }
    public float NextConfinedBlastsAngle
    {
        get => NPC.ai[2];
        set => NPC.ai[2] = value;
    }
    public int AttackModeTally
    {
        get => (int)NPC.ai[3];
        set => NPC.ai[3] = value;
    }
    public Player Target
    {
        get
        {
            if(NPC.target >= 0 && NPC.target < Main.maxPlayers)
            {
                return Main.player[NPC.target];
            }
            else
            {
                return null;
            }
        }
    }

    NPCDespawnHandler despawnHandler;
    public override void AI()
    {
        //If we're about to despawn, and it's not first phase, then clean up by deactivating the pyramid and clearing any targeting lasers
        if (despawnHandler.TargetAndDespawn(NPC.whoAmI) && !firstPhase)
        {
            if (Main.tile[5810, 1670] != null)
            {
                if (Main.tile[5810, 1670].HasTile && Main.tile[5810, 1670].IsActuated)
                {
                    DeactuatePyramid();
                }
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<Projectiles.GenericLaser>())
                {
                    Main.projectile[i].Kill();
                }
            }
        }

        Lighting.AddLight(NPC.Center, Color.Blue.ToVector3());
        UsefulFunctions.DustRing(NPC.Center, 64, DustID.ShadowbeamStaff);

        //Force an update 3 times a second. Terraria gets a bit lazy about it, and this consistency is required to prevent rubberbanding on certain high-intensity attacks
        if (Main.GameUpdateCount % 20 == 0)
        {
            NPC.netUpdate = true;
        }


        //If it's the first phase
        if (firstPhase)
        {
            //Check if it's either low on health or has already begun the phase change process
            if (changingPhases || ((NPC.life < (9 * NPC.lifeMax / 10))))
            {
                ChangePhases();
            }
            //If not, then proceed to its 'classic' first phase
            else
            {
                FirstPhase();
            }
        }

        //If it's not in the first phase, move according to the pattern of the current attack. If it's not a multiplayer client, then also run its attacks.
        //These are split up to keep the code readable.
        else
        {
            if (CurrentMove != null)
            {
                CurrentMove.Move();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    CurrentMove.Attack();
                }
            }
            else
            {
                CurrentMove = new DarkCloudMove(DragoonLanceMove, DragoonLanceAttack, DarkCloudAttackID.DragoonLance, "Dragoon Lance");
            }
            AttackModeCounter++;
        }

        if (AttackModeCounter == 5)
        {
            PrecalculateFirstTeleport();
        }
    }


    //Randomly pick a new unused attack and reset attack variables
    void ChangeAttacks()
    {
        if (testAttack == -1)
        {
            for (int i = 0; i < ActiveMoveList.Count; i++)
            {
                if (ActiveMoveList[i].ID == NextAttackMode)
                {
                    //Set the current move using the previous, stored attack mode now that it's had time to sync
                    CurrentMove = ActiveMoveList[i];

                    //Remove the chosen attack from the list so it can't be picked again until all other attacks are used up
                    ActiveMoveList.RemoveAt(i);
                    break;
                }
                if (i == (ActiveMoveList.Count - 1) && Main.netMode != NetmodeID.Server)
                {
                    Main.NewText("Move failed to set! NextAttackMode " + NextAttackMode + "ActiveMoveList.Count" + ActiveMoveList.Count);
                }
            }

            //If there's no moves left in the list, refill it   
            if (ActiveMoveList.Count == 0)
            {
                InitializeMoves();
            }

            //Pick the next attack mode from the ones that remain, and store it in ai[0] (NextAttackMode) so it can sync
            NextAttackMode = ActiveMoveList[Main.rand.Next(ActiveMoveList.Count)].ID;
        }
        else
        {
            CurrentMove = ActiveMoveList[testAttack];
            NextAttackMode = testAttack;
        }

        //Reset variables
        NPC.velocity = Vector2.Zero;
        AttackModeCounter = -1;
        AttackModeTally = 0;
        nextWarpPoint = Vector2.Zero;
        InstantNetUpdate();
    }

    //int NextWarpEntropy;
    //bool sendEntropy = true;
    public override void SendExtraAI(BinaryWriter writer)
    {
        //Send the list of remaining moves
        if (ActiveMoveList == null)
        {
            writer.Write(0);
        }
        else
        {
            writer.Write(ActiveMoveList.Count);
            for (int i = 0; i < ActiveMoveList.Count; i++)
            {
                writer.Write(ActiveMoveList[i].ID);
            }
        }


        //A seed value that clients can use whenever they'd like to pick the next attack.
        //Would allow all clients to "randomly" roll the same attack right when it happens, instead of needing to do it early.
        //writer.Write(sendEntropy);
        //if (sendEntropy)
        // {
        //     NextWarpEntropy = Main.rand.Next();
        //     writer.Write(NextWarpEntropy);
        // }

        //Send the next point to teleport to during this attack, and the first point for the next attack
        writer.WriteVector2(nextWarpPoint);
        writer.WriteVector2(preSelectedWarpPoint);
        ;

        if (CurrentMove == null)
        {
            writer.Write(-1);
        }
        else
        {
            writer.Write(CurrentMove.ID);
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        //Recieve the list of remaining moves
        int moveCount = reader.ReadInt32();
        List<int> validMoves = new List<int>();
        for (int i = 0; i < moveCount; i++)
        {
            int move = reader.ReadInt32();
            validMoves.Add(move);
        }
        InitializeMoves(validMoves);

        //bool recievedEntropy = reader.ReadBoolean();
        //if (recievedEntropy)
        //{
        //    //A seed value that clients can use whenever they'd like to pick the next attack.
        //    NextWarpEntropy = reader.ReadInt32();
        //}

        //Recieve the next point to teleport to during this attack, and the first point for the next attack
        nextWarpPoint = reader.ReadVector2();
        preSelectedWarpPoint = reader.ReadVector2();


        int readMoveID = reader.ReadInt32();
        if (readMoveID != -1)
        {
            for (int i = 0; i < DefaultList.Count; i++)
            {
                if (DefaultList[i].ID == readMoveID)
                {
                    CurrentMove = DefaultList[i];
                }
            }
        }
    }

    //These describe how the boss should move, and other things that should be done on the server and every client to keep it deterministic
    #region Movements

    //A few moves use teleports that need to be calculated in advance so their first warp can be pre-synced. That's done here.
    void PrecalculateFirstTeleport()
    {
        if (NextAttackMode == DarkCloudAttackID.DivineSpark)
        {
            preSelectedWarpPoint = DivineSparkTeleport();
        }
        if (NextAttackMode == DarkCloudAttackID.ArrowRain)
        {
            preSelectedWarpPoint = ArrowRainTeleport();
        }
        if (NextAttackMode == DarkCloudAttackID.AntiMat)
        {
            preSelectedWarpPoint = Main.rand.NextVector2CircularEdge(700, 700);
        }
        if (NextAttackMode == DarkCloudAttackID.TeleportingSlashes)
        {
            preSelectedWarpPoint = Target.Center + Main.rand.NextVector2CircularEdge(slashesWarpRadius, slashesWarpRadius);
        }
        InstantNetUpdate();
    }

    void DragoonLanceMove()
    {
        NPC.position.Y = Main.player[NPC.target].position.Y + 400;
        if (AttackModeCounter == 0)
        {
            DarkCloudParticleEffect(-2);
            NPC.position = Main.player[NPC.target].position + (new Vector2(-800, 400));
            DarkCloudParticleEffect(6);
        }
        if (AttackModeCounter <= 60)
        {
            DarkCloudParticleEffect(-2, 8 * (AttackModeCounter / 60));
        }
        if (AttackModeCounter == 60)
        {
            //Burst of particles
            DarkCloudParticleEffect(18, 30);
        }
        if (AttackModeCounter >= 60 && AttackModeCounter < 180)
        {
            NPC.velocity = new Vector2(17, 0);
        }
        if (AttackModeCounter == 180)
        {
            DarkCloudParticleEffect(-2);
            NPC.position = Main.player[NPC.target].position + (new Vector2(800, 400));
            DarkCloudParticleEffect(6);
        }
        if (AttackModeCounter >= 180 && AttackModeCounter < 300)
        {
            NPC.velocity = new Vector2(-17, 0);
        }
        if (AttackModeCounter == 300)
        {
            DarkCloudParticleEffect(-2);
            NPC.position = Main.player[NPC.target].position + (new Vector2(-800, 400));
            DarkCloudParticleEffect(6);
        }
        if (AttackModeCounter >= 300 && AttackModeCounter < 420)
        {
            NPC.velocity = new Vector2(17, 0);
        }
        if (AttackModeCounter == 420)
        {
            ChangeAttacks();
        }
    }

    float initialTargetRotation;
    bool counterClockwise = false;
    //How long each cycle takes
    const int turnLength = 60;
    //How long dark cloud telegraphs its shot
    const int chargeTime = 20;
    //Length in degrees of the arc on either side of the player
    const int arcLength = 60;


    void DivineSparkMove()
    {
        //If it's the first attack wait and charge for a moment
        if (AttackModeTally <= 0)
        {

            if (AttackModeCounter == chargeTime)
            {
                AttackModeCounter--;
                if (AttackModeTally == 0)
                {
                    AttackModeTally = -200;
                }
                else
                {
                    AttackModeTally++;
                }
                if (AttackModeTally == -1)
                {
                    AttackModeTally = 1;
                }

                float factor = (AttackModeTally + 200f) / 200f;
                DarkCloudParticleEffect(-18 * factor, 200 * factor, AttackModeTally * 5);
            }
        }

        
        if (AttackModeCounter % turnLength == 0)
        {
            DarkCloudParticleEffect(-2);
            if (AttackModeCounter > 70)
            {
                //Check that the previously chosen warp point is still valid, since players could have moved dramatically since the last one was chosen.
                //If not, then reflect it, but only by 170. Keep doing that until either a spot is found, or it runs out of tries.
                //This code does not add any randomness, so every client will run through it the same.
                bool valid = false;
                int triesLeft = 50;
                do
                {
                    if (Collision.CanHit(Target.Center + nextWarpPoint, 1, 1, Target.Center, 1, 1) || Collision.CanHitLine(Target.Center + nextWarpPoint, 1, 1, Target.Center, 1, 1))
                    {
                        valid = true;
                    }
                    else
                    {
                        nextWarpPoint = nextWarpPoint.RotatedBy(MathHelper.ToRadians(170));
                    }
                    triesLeft--;
                    //Retry at maximum 150 times, if no 'fair' spot exists then ignore the rule and continue
                    if (triesLeft == 0)
                    {
                        break;
                    }
                }
                while (!valid);

                NPC.Center = Target.Center + nextWarpPoint;
            }
            else
            {
                NPC.Center = Target.Center + preSelectedWarpPoint;
            }

            nextWarpPoint = DivineSparkTeleport();
            DarkCloudParticleEffect(6);
        }
        
        if (AttackModeCounter % turnLength <= 15)
        {
            initialTargetRotation = (Target.Center - NPC.Center).ToRotation();
            if (NPC.Center.Y > Target.Center.Y)
            {
                if (NPC.Center.X < Target.Center.X)
                {
                    initialTargetRotation += MathHelper.ToRadians(arcLength);
                    counterClockwise = true;
                }
                else
                {
                    initialTargetRotation -= MathHelper.ToRadians(arcLength);
                    counterClockwise = false;
                }
            }
            else
            {
                if (NPC.Center.X < Target.Center.X)
                {
                    initialTargetRotation -= MathHelper.ToRadians(arcLength);
                    counterClockwise = false;
                }
                else
                {
                    initialTargetRotation += MathHelper.ToRadians(arcLength);
                    counterClockwise = true;
                }
            }
        }


        if (AttackModeCounter == turnLength * 5)
        {
            ChangeAttacks();
        }
    }

    Vector2 DivineSparkTeleport()
    {
        //This ensures the boss will not appear deep in a wall, making it impossible to dodge the laser
        Vector2 warp;
        bool valid;
        int triesLeft = 150;
        do
        {
            valid = false;
            float angle = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
            if (Main.rand.NextBool())
            {
                angle += MathHelper.PiOver2;
            }
            else
            {
                angle -= MathHelper.PiOver2;
            }
            warp = new Vector2(550, 0).RotatedBy(angle);

            if (Collision.CanHit(Target.Center + warp, 1, 1, Target.Center, 1, 1) || Collision.CanHitLine(Target.Center + warp, 1, 1, Target.Center, 1, 1))
            {
                valid = true;
            }

            triesLeft--;
            //Retry at maximum 150 times, if no 'fair' spot exists then ignore the rule and continue
            if (triesLeft == 0)
            {
                break;
            }
        } while (!valid);

        return warp;
    }

    List<Player> targetPlayers;
    float pullSpeed = 0.2f;
    Vector2 gravCounter = new Vector2(0, -.3f); //Applies a force countering gravity
    Vector2 shockwaveCounter = new Vector2(0, -5.2f);
    void DarkFlowMove()
    {
        //Nope lol
        NukeGrapples();

        //Make sure it stays still
        NPC.velocity = Vector2.Zero;

        //At the start of the attack, teleport to the arena center, and make a list of every player within 5000 units (to avoid pulling players who are on the other end of the world)
        if (targetPlayers == null || AttackModeCounter == 0)
        {
            TeleportToArenaCenter();
            targetPlayers = new List<Player>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Vector2.Distance(Main.player[i].Center, NPC.Center) < 5000)
                {
                    targetPlayers.Add(Main.player[i]);
                }
            }
        }

        //The attack ramps up over 5 seconds. This helps it keep track of that and keep code clean
        float strengthFactor = 1;
        int damageRadius = 220;
        if (AttackModeCounter < 300)
        {
            strengthFactor = (AttackModeCounter / 300);
        }

        //Draw some dusts
        //"Black hole" effect dusts
        for (int i = 0; i < 50; i++)
        {
            Vector2 dustPos = Main.rand.NextVector2CircularEdge(damageRadius * strengthFactor, damageRadius * strengthFactor);
            Vector2 dustVel = new Vector2(15, 0).RotatedBy(dustPos.ToRotation() + MathHelper.Pi);
            dustPos += NPC.Center;

            Dust.NewDustPerfect(dustPos, 109, dustVel, Scale: 2).noGravity = true;
        }

        //Dusts indicating the damage radius of the attack
        for (int j = 0; j < 20; j++)
        {
            Vector2 dir = Main.rand.NextVector2CircularEdge(damageRadius * strengthFactor, damageRadius * strengthFactor);
            Vector2 dustPos = NPC.Center + dir;

            Vector2 dustVel = new Vector2(2, 0).RotatedBy(dir.ToRotation() + MathHelper.Pi / 2);
            Dust.NewDustPerfect(dustPos, DustID.ShadowbeamStaff, dustVel, 200).noGravity = true;

        }

        //Spawn a ring of dust at the hard pull radius
        UsefulFunctions.DustRing(NPC.Center, 1900, DustID.ShadowbeamStaff, 50);

        //The actual attack
        //For each player in the list, check if they're outside past the attack's hard pull radius. If so, pull them back into it hard.
        //Otherwise, pull them in normally
        float distance;
        foreach (Player p in targetPlayers)
        {
            //Counter the shockwave effect. Sorry!
            if (p.GetModPlayer<tsorcRevampPlayer>().Shockwave && p.controlDown)
            {
                p.velocity += shockwaveCounter;
            }
            distance = Vector2.Distance(p.Center, NPC.Center);
            p.velocity += gravCounter;
            if (distance < 1900)
            {
                p.velocity += UsefulFunctions.GenerateTargetingVector(p.Center, NPC.Center, pullSpeed * strengthFactor).RotatedBy(MathHelper.ToRadians(25));
            }
            else
            {
                p.velocity += UsefulFunctions.GenerateTargetingVector(p.Center, NPC.Center, pullSpeed * 10 * strengthFactor);
            }

            //If they're within the dust ring in the center, then damage them rapidly. Calculate the damage such that it increases to counter player defense or damage reduction.
            if (distance < damageRadius * strengthFactor)
            {
                float damage = 99;
                damage /= (1 - p.endurance);
                if (Main.expertMode)
                {
                    damage += (int)Math.Ceiling(p.statDefense * 0.75);
                }
                else
                {
                    damage += (int)Math.Ceiling(p.statDefense * 0.5);
                }
                //player.Hurt() lets you cause damage whenever and however you'd like
                //This lets us bypass the fact that all hitboxes are square, and simply cause damage if the player is within a the dust ring radius
                //https://en.wikipedia.org/wiki/Spaghettification
                p.immuneTime = 0;
                p.Hurt(Terraria.DataStructures.PlayerDeathReason.ByCustomReason(p.name + " was spaghettified."), (int)damage, 1);
            }
        }

        //At the end of the attack, change attacks and spawn a burst of dust
        if (AttackModeCounter >= 1200)
        {
            for (int i = 0; i < 120; i++)
            {
                Vector2 offset = Main.rand.NextVector2CircularEdge(256, 256);
                Vector2 velocity = new Vector2(15, 0).RotatedBy(offset.ToRotation()) * Main.rand.NextFloat(2);
                Dust.NewDustPerfect(NPC.Center + offset, DustID.ShadowbeamStaff, velocity, Scale: 2).noGravity = true;
            }
        }
        if (AttackModeCounter == 1210)
        {
            ChangeAttacks();
        }
    }

    //Melee attack. The main, big one.
    //This attack is an enormous mess ngl lol. I tried to use a list of timings for the sub-attacks specifying what should happen when, which was a mistake.
    //Should have really used states instead (like I did for the main attacks), but I realized that too late.
    //May re-do it at some point anyway to allow the sub-attacks happen in a random order, but on the other hand the fact it's choreographed means the attacks can happen faster (since player needs less time to react).
    Vector2 targetPoint;
    Vector2 slamVelocity;
    bool hitGround = false;
    bool dashLeft = true;
    int backstabOffset = 120;
    void UltimaWeaponMove()
    {

        //Inflict debuff
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            if (Main.player[i].active)
            {
                Main.player[i].AddBuff(ModContent.BuffType<WeightOfShadow>(), 60);
            }
        }

        //Delete all grapples. This runs every tick that this attack is in use.
        for (int p = 0; p < 1000; p++)
        {
            if (Main.projectile[p].active && Main.projectile[p].aiStyle == 7)
            {
                Main.projectile[p].Kill();
            }
        }

        //Initialize things
        if (AttackModeCounter == 0)
        {
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.Center = Target.Center + new Vector2(-500, 0);
        }

        //Hold AttackModeCounter at 1 and keep refreshing the debuff until the target player lands, delaying the start of the attack phase
        if (Target.velocity.Y != 0 && AttackModeCounter == 2)
        {
            AttackModeCounter = 1;
        }

        //Once the attack properly begins, refresh the debuff, teleport in front of the player, and prepare to dash
        if (AttackModeCounter == 3)
        {
            int distance = 500;
            if (Target.direction == -1)
            {
                distance *= -1;
                dashLeft = false;
            }
            else
            {
                dashLeft = true;
            }
            Vector2 warp = Target.Center;
            warp.X += distance;
            NPC.Center = warp;

            DarkCloudParticleEffect(5);
        }

        //Prevent it from moving along the y-axis for the duration of the attack
        if (AttackModeCounter >= 3 && AttackModeCounter <= 210)
        {
            NPC.velocity.Y = 0;
        }

        //Charging particle effect
        if (AttackModeCounter >= 3 && AttackModeCounter <= 60)
        {
            ChargingParticleEffect(AttackModeCounter - 3, 57);
        }

        //Dash at the player. Once past them, slow down and chill for a moment.
        if (AttackModeCounter >= 60 && AttackModeCounter <= 150)
        {
            if ((NPC.Center.X > Target.Center.X) && dashLeft)
            {
                NPC.velocity = new Vector2(-30, 0);
                NPC.noTileCollide = true;
                NPC.noGravity = true;
            }
            else if ((NPC.Center.X < Target.Center.X) && !dashLeft)
            {
                NPC.velocity = new Vector2(30, 0);
                NPC.noTileCollide = true;
                NPC.noGravity = true;
            }
            else
            {
                if (Math.Abs(NPC.velocity.X) < 1)
                {
                    NPC.noTileCollide = false;
                    NPC.noGravity = false;
                }
            }

            //Make the NPC face the direction it is moving (it defaults toward its target)
            if (NPC.velocity.X > 0)
            {
                NPC.direction = 1;
            }
            else
            {
                NPC.direction = -1;
            }
        }

        //Charge up effect again
        if (AttackModeCounter > 120 && AttackModeCounter < 180)
        {
            ChargingParticleEffect((int)AttackModeCounter - 120, 60);
        }

        //Pick and store a point 505 units above the player (the extra 5 is to reduce how often it clips into the ground mid-slam, because terraria's (XNA's?) collision is hot garbage)
        if (AttackModeCounter == 180)
        {
            targetPoint = Target.Center;
            targetPoint.Y -= 505;
        }

        //Leap toward the chosen point, summoning the sword and charging up dust
        if (AttackModeCounter > 180 && AttackModeCounter < 240)
        {
            ChargingParticleEffect((int)AttackModeCounter - 180, 60);
            NPC.noTileCollide = true;

            //If not close to the chosen point, accelerate toward it. Within 200 units is close enough.
            if (Vector2.DistanceSquared(NPC.Center, targetPoint) > 200)
            {
                NPC.velocity = UsefulFunctions.GenerateTargetingVector(NPC.Center, targetPoint, 30 - ((AttackModeCounter - 180) / 2));
            }
            else
            {
                NPC.velocity = Vector2.Zero;
            }
        }

        //Slam down directly toward the player
        if (AttackModeCounter >= 240 && AttackModeCounter < 300)
        {
            NPC.noTileCollide = false;

            //On hitting ground after slam
            if (OnGround())
            {
                //If it's the first frame we hit the ground, do some stuff. If not, do nothing.
                if (!hitGround)
                {
                    for (int i = 0; i < 60; i++)
                    {
                        Vector2 offset = Main.rand.NextVector2CircularEdge(5, 5);
                        if (Math.Abs(offset.Y) < 2.5f)
                        {
                            Vector2 velocity = new Vector2(7, 0).RotatedBy(offset.ToRotation()) * Main.rand.NextFloat(2);
                            Dust.NewDustPerfect(NPC.Center + offset, DustID.ShadowbeamStaff, velocity * 5, Scale: 5).noGravity = true;
                        }
                    }

                    NPC.velocity = Vector2.Zero;
                    NPC.noGravity = false;
                    hitGround = true;
                }
            }

            //In air slamming
            else
            {
                NPC.noGravity = true;
                ChargingParticleEffect((int)AttackModeCounter - 240, 20);

                slamVelocity = UsefulFunctions.GenerateTargetingVector(NPC.Center, Target.Center, 20);

                //Do not change X velocity if it would cause dark cloud to change directions mid-slam
                if ((slamVelocity.X > 0 && NPC.velocity.X >= 0) || (slamVelocity.X < 0 && NPC.velocity.X <= 0))
                {
                    //Do not change x velocity if it would cause dark cloud to slow down
                    //These checks are to allow the player to dash under it
                    if (Math.Abs(slamVelocity.X) > Math.Abs(NPC.velocity.X))
                    {
                        NPC.velocity = slamVelocity;
                    }
                }

                NPC.velocity.Y = (AttackModeCounter - 240) / 1.5f;
                if (NPC.velocity.Y > 35)
                {
                    NPC.velocity.Y = 35;
                }
            }

            if (NPC.velocity.X > 0)
            {
                NPC.direction = 1;
            }
            else
            {
                NPC.direction = -1;
            }

        }

        //Swing, firing off a crescent shaped wave of shadow energy from the sword
        //Also, reset some variables
        if (AttackModeCounter == 300)
        {
            slamVelocity = Vector2.Zero;
            hitGround = false;
        }

        //Set its velocity to 0 for the next few attacks
        if (AttackModeCounter >= 360 && AttackModeCounter < 600)
        {
            NPC.velocity = Vector2.Zero;
        }

        //Telegraph teleport
        if (AttackModeCounter > 330 && AttackModeCounter < 360)
        {
            Vector2 warp = Target.Center;
            warp.X += backstabOffset * Target.direction * -1;
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(warp + Main.rand.NextVector2Circular(30, 60), DustID.ShadowbeamStaff, Main.rand.NextVector2CircularEdge(3, 3));
            }
        }

        //Teleport behind the player
        if (AttackModeCounter == 360)
        {
            Vector2 warp = Target.Center;
            warp.X += backstabOffset * Target.direction * -1;
            NPC.Center = warp;

            DarkCloudParticleEffect(5);
        }

        //Telegraph teleport
        if (AttackModeCounter > 420 && AttackModeCounter < 480)
        {
            Vector2 warp = Target.Center;
            warp.X += backstabOffset * Target.direction * -1;
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustPerfect(warp + Main.rand.NextVector2Circular(30, 60), DustID.ShadowbeamStaff, Main.rand.NextVector2CircularEdge(3, 3));
            }
        }

        //Teleport behind the player again
        if (AttackModeCounter == 480)
        {
            Vector2 warp = Target.Center;
            warp.X += backstabOffset * Target.direction * -1;
            NPC.Center = warp;

            DarkCloudParticleEffect(5);
        }


        //Teleport above player
        if (AttackModeCounter == 600)
        {
            Vector2 warp = Target.Center;
            warp.X -= 0.1f;
            warp.Y -= 500;
            NPC.Center = warp;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.velocity = Vector2.Zero;
            //Do not get hit by this.
            NPC.damage = 600;
        }

        if (AttackModeCounter >= 600 && AttackModeCounter < 630)
        {
            float count = AttackModeCounter - 600;
            DarkCloudParticleEffect(-5, count * 4, 42 - count);
        }

        //Similar to first slam, but fall straight down instead.
        if (AttackModeCounter >= 630 && AttackModeCounter < 730)
        {
            NPC.noTileCollide = false;

            //Mostly the same code as before, with a few tweaks
            if (OnGround())
            {
                if (!hitGround)
                {
                    for (int i = 1; i < 200; i++)
                    {
                        Vector2 offset = Main.rand.NextVector2CircularEdge(i * 2, i * 2);
                        while (Math.Abs(Math.Sin(offset.ToRotation())) > 0.5f)
                        {
                            offset = Main.rand.NextVector2CircularEdge(i, i);
                        }

                        Vector2 velocity = new Vector2(7, 0).RotatedBy(offset.ToRotation()) * Main.rand.NextFloat(2);
                        Dust.NewDustPerfect(NPC.Center + offset, DustID.ShadowbeamStaff, velocity * 5, Scale: 5).noGravity = true;
                    }

                    //Fire shockwave projectiles. This sorta has to be done here, not in Attack() like the other projectiles, because it's not time-based. When the NPC hits the ground depends on how far up it is.
                    //Just another way this attack is kinda sloppy and breaks the way I tried to set this boss up lol
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float projSpeed = 15;
                        for (int i = 0; i < 50; i++)
                        {
                            Vector2 velocity = new Vector2(projSpeed, 0).RotatedByRandom(MathHelper.ToRadians(45));
                            if (Main.rand.NextBool() == true)
                            {
                                velocity.X *= -1;
                            }
                            if (Main.rand.NextBool() == true)
                            {
                                velocity.Y *= -1;
                            }
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<DarkWave>(), DarkCloud.darkSlashDamage, 0.5f, Main.myPlayer);
                        }
                    }


                    NPC.velocity = Vector2.Zero;
                    NPC.damage = 200;
                    NPC.noGravity = false;
                    hitGround = true;
                }
            }
            else
            {
                NPC.noGravity = true;
                DarkCloudParticleEffect(5, 120, 12);
                NPC.velocity.Y = (AttackModeCounter - 540) / 2f;
                if (NPC.velocity.Y > 11)
                {
                    NPC.velocity.Y = 11;
                }
            }
        }

        //Reset variables
        if (AttackModeCounter == 770)
        {
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.velocity = new Vector2(0, -22);
            hitGround = false;
            slamVelocity = Vector2.Zero;
        }

        //End the attack phase
        if (AttackModeCounter == 790)
        {
            ChangeAttacks();
        }
    }


    float confinedBlastsRadius = 500;
    float currentBlastAngle = 0;
    void ConfinedBlastsMove()
    {
        //Honestly, this attack exists at least partially just to give melee players an opening to tear the boss apart
        //I think it turned out pretty well, though!
        //Creates a safe region around the NPC, outside of which the player gets pulled in and eventually takes damage
        //Telegraph a series of blasts in different directions. Then fire them one by one with a 1 second delay

        if (AttackModeCounter == 0)
        {
            TeleportToArenaCenter();
        }

        if (targetPlayers == null)
        {
            targetPlayers = new List<Player>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Vector2.Distance(Main.player[i].Center, NPC.Center) < 5000)
                {
                    targetPlayers.Add(Main.player[i]);
                }
            }
        }

        //Make sure it stays still
        NPC.velocity = Vector2.Zero;

        //Scales from 0 to 1 as the attack ramps up. Certain effects are tied to this.
        float intensity = (AttackModeCounter / 300);
        if (AttackModeCounter > 300)
        {
            intensity = 1;
        }

        //Draw some dust at the attack edge
        DarkCloudParticleEffect(-2 * (AttackModeCounter / 300), 50, confinedBlastsRadius + 2000 * (1 - intensity));

        //Nuke grapples
        NukeGrapples();


        float radius = Main.rand.Next((int)(confinedBlastsRadius + 2000 * (1 - intensity)), (int)(2000 + 2000 * (1 - intensity)));
        DarkCloudParticleEffect(-10, 100, radius);


        //For each player in the list, check if they're out of the attack range. If so, pull them into it hard.
        float distance;
        foreach (Player p in targetPlayers)
        {
            distance = Vector2.Distance(p.Center, NPC.Center);
            if (distance > confinedBlastsRadius)
            {
                p.velocity += UsefulFunctions.GenerateTargetingVector(p.Center, NPC.Center, pullSpeed * 5 * (AttackModeCounter / 300));

                //If more than 5 seconds have passed, damage them when outside the range
                if (AttackModeCounter > 300)
                {
                    float damage = 5;
                    damage *= (1 - p.endurance);
                    if (Main.expertMode)
                    {
                        damage += (int)Math.Ceiling(p.statDefense * 0.75f);
                    }
                    else
                    {
                        damage += (int)Math.Ceiling(p.statDefense * 0.5f);
                    }

                    p.immuneTime = 0;
                    p.Hurt(Terraria.DataStructures.PlayerDeathReason.ByCustomReason(p.name + " was torn apart by tidal forces."), (int)damage, 1);
                }
            }
        }

        if (AttackModeCounter > 300)
        {
            //Every second, perform the attack
            if ((AttackModeCounter - 300) % 60 == 0)
            {
                currentBlastAngle = NextConfinedBlastsAngle;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NextConfinedBlastsAngle = Main.rand.NextFloat(0, MathHelper.Pi);
                }
                InstantNetUpdate();
            }

            //Spawn dust telegraphing next attack
            for (int i = 0; i < 20; i++)
            {
                //Spawn dust within 45 degrees (Pi / 4) around the chosen angle 
                float dustAngle = currentBlastAngle + Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                Vector2 dustPos = new Vector2(Main.rand.NextFloat(0, confinedBlastsRadius), 0).RotatedBy(dustAngle);
                Dust thisDust = Dust.NewDustPerfect(NPC.Center + dustPos, DustID.VenomStaff, Scale: 1.5f);
                thisDust.noLight = true;
                thisDust.noGravity = true;

                thisDust = Dust.NewDustPerfect(NPC.Center - dustPos, DustID.VenomStaff, Scale: 1.5f);
                thisDust.noLight = true;
                thisDust.noGravity = true;
            }

            if ((AttackModeCounter - 300) % 60 == 59)
            {
                for (int i = 0; i < 50; i++)
                {
                    float dustAngle = currentBlastAngle + Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                    Vector2 dustVel = new Vector2(99, 0).RotatedBy(dustAngle);
                    Dust.NewDustPerfect(NPC.Center, DustID.ShadowbeamStaff, dustVel, Scale: 5).noLight = true;
                    Dust.NewDustPerfect(NPC.Center, DustID.ShadowbeamStaff, -dustVel, Scale: 5).noLight = true;
                }

                //In-between each attack, nuke all dust that was telegraphing the previous one
                //Hacky way to do this, but it works
                for (int i = 0; i < Main.maxDust; i++)
                {
                    if (Main.dust[i].type == DustID.VenomStaff)
                    {
                        Main.dust[i].active = false;
                        Main.dust[i].scale = 0;
                    }
                }
            }
        }

        //At the end of the attack, change attacks and spawn a burst of dust
        if (AttackModeCounter >= 800)
        {
            for (int i = 0; i < 120; i++)
            {
                Vector2 offset = Main.rand.NextVector2CircularEdge(256, 256);
                Vector2 velocity = new Vector2(15, 0).RotatedBy(offset.ToRotation()) * Main.rand.NextFloat(2);
                Dust.NewDustPerfect(NPC.Center, DustID.ShadowbeamStaff, velocity, Scale: 2).noGravity = true;
            }
        }

        //Wait a second and a half to change attacks
        //Give the player time to move to a normal safe distance
        if (AttackModeCounter == 890)
        {
            ChangeAttacks();
        }
    }
    void FreezeBoltsMove()
    {
        if (AttackModeTally == 0)
        {
            if (AttackModeCounter == 0)
            {
                TeleportToArenaCenter();
            }

            if (AttackModeCounter >= 0 && AttackModeCounter <= 40)
            {
                IceChargingParticleEffect(AttackModeCounter, 40);
            }
            if (AttackModeCounter > 150)
            {
                count++;
                AttackModeCounter = 0;
            }
            if (count == 3)
            {
                AttackModeCounter = 0;
                AttackModeTally = 1;
                return;
            }
        }

        if (AttackModeTally == 1)
        {
            if (AttackModeCounter >= 80 && AttackModeCounter <= 120)
            {
                IceChargingParticleEffect(AttackModeCounter - 80, 40);
            }
            if (AttackModeCounter == 500)
            {
                AttackModeCounter = 0;
                AttackModeTally = 2;
                return;
            }
        }

        if (AttackModeTally == 2)
        {
            if (AttackModeCounter > 500)
            {
                //Clean up
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<DarkFreezeBolt>())
                    {
                        Main.projectile[i].Kill();
                    }
                }
                count = 0;
                ChangeAttacks();
            }
        }
    }
    Vector2 arrowRainTargetingVector = Vector2.Zero;
    void ArrowRainMove()
    {
        //Right now, this uses a shitty vague approximation of actual ballistic projectile aiming code. It *works*, but could be so much better
        //Will probably see if I can get the actual thing working later

        //Teleport somewhere further from the player, and then fire a shotgun barrage of Arrow of Bard's at them
        if (AttackModeCounter % 80 == 0)
        {
            DarkCloudParticleEffect(-2);

            if (AttackModeCounter > 70)
            {
                NPC.Center = nextWarpPoint + Target.Center;
            }
            else
            {
                NPC.Center = preSelectedWarpPoint + Target.Center;
            }

            //Pick the next warp point immediately after warping, to give it time to sync
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                nextWarpPoint = ArrowRainTeleport();
            }
            DarkCloudParticleEffect(6);
            AttackModeTally++;

            //This is generated client-side here because it's also needed for the draw code, to make it hold the bow at the right angle
            if (AttackModeTally % 2 == 0)
            {
                arrowRainTargetingVector = UsefulFunctions.BallisticTrajectory(NPC.Center, Target.Center, 11, 0.05f, false);
            }
            else
            {
                arrowRainTargetingVector = UsefulFunctions.BallisticTrajectory(NPC.Center, Target.Center, 11, 0.05f, true);
            }
        }

        if (AttackModeCounter == 1200)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<Projectiles.Enemy.DarkCloud.EnemyArrowOfDarkCloud>())
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Dust.NewDust(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, DustID.FireworkFountain_Green, Main.projectile[i].velocity.X * 0.8f, Main.projectile[i].velocity.Y * 0.8f);
                        Dust.NewDust(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, DustID.ShadowbeamStaff, Main.projectile[i].velocity.X * 0.8f, Main.projectile[i].velocity.Y * 0.8f);
                    }
                    Main.projectile[i].Kill();
                }
            }
            ChangeAttacks();
        }
    }
    Vector2 ArrowRainTeleport()
    {
        Vector2 warp = Vector2.Zero;
        do
        {
            warp.X += Main.rand.Next(-900, 900);
            warp.Y += Main.rand.Next(-400, 400);
        } while (Vector2.Distance(warp + Target.Center, Target.Center) < 500);
        return warp;
    }
    void AntiMatMove()
    {
        //Line up an Anti-Mat with a targeting laser, and spawn a handful of reflections around the player. After a delay, they open fire one by one.
        if (AttackModeCounter % 300 == 0)
        {
            DarkCloudParticleEffect(-2);

            //The first warp should be to the pre-selected point
            if (AttackModeCounter > 270)
            {
                NPC.Center = Target.Center + nextWarpPoint;
            }
            else
            {
                NPC.Center = Target.Center + preSelectedWarpPoint;
            }

            //They'll recieve it from the server.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                nextWarpPoint = Main.rand.NextVector2CircularEdge(700, 700);
            }
            DarkCloudParticleEffect(6);
        }

        if (AttackModeCounter == 1200)
        {
            ChangeAttacks();
        }
    }

    float slashesWarpRadius = 750;
    void TeleportingSlashesMove()
    {
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.aiStyle = 0;
        NPC.velocity.Y += 0.09f;
        NPC.velocity.X *= 1.07f;
        if (Target.Center.X > NPC.Center.X)
        {
            NPC.direction = 1;
        }
        else
        {
            NPC.direction = -1;
        }

        //Skip the first one to give players time to react
        if (AttackModeCounter % 80 == 0 && AttackModeCounter != 80)
        {
            DarkCloudParticleEffect(-2);
            if (AttackModeCounter > 70)
            {
                NPC.Center = nextWarpPoint;
            }
            else
            {
                NPC.Center = preSelectedWarpPoint;
            }
            nextWarpPoint = Target.Center + Main.rand.NextVector2CircularEdge(slashesWarpRadius, slashesWarpRadius);
            DarkCloudParticleEffect(6);
            NPC.velocity = UsefulFunctions.GenerateTargetingVector(NPC.Center, Target.Center, 17);
        }

        for (int i = 0; i < 50; i++)
        {
            Dust.NewDustPerfect(nextWarpPoint + Main.rand.NextVector2Circular(30, 60), DustID.ShadowbeamStaff, Main.rand.NextVector2CircularEdge(3, 3));
        }

        //Perform a rapid chain of dashes toward the player, while swinging Ultima Weapon
        //At the end, dash above the player, swing the weapon around once, then it slam it down at high speed requiring the player to dodge just as the slam starts
        if (AttackModeCounter == 640)
        {
            //Clean up
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type == ModContent.NPCType<DarkCloudMirror>())
                {
                    Main.npc[i].active = false;
                }
            }
            ChangeAttacks();
        }
    }
    void BulletPortalsMove()
    {
        //Scrapped: Too similar to teleporting slashes to dodge. Might rework this later, do kinda want to give it some attack based on the Expulsor Cannon
        //Dark cloud opens a black hole in front of itself, then begins firing the quardro cannon into it (with a tighter spread than the players)
        //A second later white holes begin to open up randomly at close range around the player one by one, firing bursts of bullets toward them
    }
    #endregion

    //These describe projectiles the boss should shoot, and other things that should *not* be done for every multiplayer client
    #region Attacks
    void DragoonLanceAttack()
    {
        if (AttackModeCounter >= 60 && AttackModeCounter < 180 && ((AttackModeCounter) % 5 == 0))
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-0.5f, -0.5f), ModContent.ProjectileType<DarkCloudDragoonLance>(), dragoonLanceDamage, 0.5f, Main.myPlayer, 20);
            DarkCloudParticleEffect(5, 15);
        }
        if (AttackModeCounter >= 180 && AttackModeCounter < 300 && ((AttackModeCounter - 2) % 5 == 0))
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-0.5f, -0.5f), ModContent.ProjectileType<DarkCloudDragoonLance>(), dragoonLanceDamage, 0.5f, Main.myPlayer, 20);
            DarkCloudParticleEffect(5, 15);
        }
        if (AttackModeCounter >= 300 && AttackModeCounter < 420 && ((AttackModeCounter) % 5 == 0))
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-0.5f, -0.5f), ModContent.ProjectileType<DarkCloudDragoonLance>(), dragoonLanceDamage, 0.5f, Main.myPlayer, 20);
            DarkCloudParticleEffect(5, 15);
        }
    }

    void DivineSparkAttack()
    {
        //Spawn the targeting lasers one by one
        if ((AttackModeCounter % turnLength) % Math.Round((chargeTime * 0.8) / 5) == 0 && AttackModeCounter % turnLength <= (chargeTime * 0.8))
        {
            Vector2 laserVector;
            if (counterClockwise)
            {
                laserVector = new Vector2(1, 0).RotatedBy(initialTargetRotation - MathHelper.ToRadians(arcLength * (AttackModeCounter % turnLength) / (turnLength / 4f)));
            }
            else
            {
                laserVector = new Vector2(1, 0).RotatedBy(initialTargetRotation + MathHelper.ToRadians(arcLength * (AttackModeCounter % turnLength) / (turnLength / 4f)));
            }

            float telegraphTimer = (int)(turnLength - (AttackModeCounter % turnLength));
            if (AttackModeCounter < turnLength)
            {
                telegraphTimer += 200;
            }
            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, laserVector, ModContent.ProjectileType<DarkDivineSpark>(), 0, 0.5f, Main.myPlayer, telegraphTimer, NPC.whoAmI);
        }

        //Spawn the big laser
        if (AttackModeCounter % turnLength == chargeTime)
        {
            int direction = 0;
            if (counterClockwise)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(1, 0).RotatedBy(initialTargetRotation), ModContent.ProjectileType<DarkDivineSpark>(), divineSparkDamage, 0.5f, Main.myPlayer, direction * 999, NPC.whoAmI);
        }
    }

    int darkFlowRadius = 2500;
    void DarkFlowAttack()
    {
        if (AttackModeCounter % 4 == 0)
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2CircularEdge(darkFlowRadius, darkFlowRadius), Vector2.Zero, ModContent.ProjectileType<DarkFlow>(), darkFlowDamage, 0.5f, Main.myPlayer, NPC.whoAmI, 1200 - AttackModeCounter);
        }
    }

    void DivineSparkThirdsAttack()
    {

    }

    void UltimaWeaponAttack()
    {
        if (AttackModeCounter == 3)
        {
            //Spawn the sword.
            //That's it. The rest of it happens within the sword's ai.
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<DarkUltimaWeapon>(), 0, NPC.whoAmI);
        }
    }

    void ConfinedBlastsAttack()
    {
        if (AttackModeCounter > 300 && (AttackModeCounter - 300) % 60 == 59)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active)
                {
                    Vector2 diff = NPC.Center - p.Center;
                    float posAngle = diff.ToRotation();

                    //Deeply cursed conversions from XNA FuckoUnits to normal radians, because working with the former was giving me a headache
                    posAngle += MathHelper.Pi;
                    posAngle = MathHelper.TwoPi - posAngle;
                    float workingAngle = MathHelper.Pi - currentBlastAngle;

                    //Mirror the player angle to the top half of the circle if it's on the bottom
                    //This means we don't need to check both blast zones, since they're mirrored too
                    if (posAngle > MathHelper.Pi)
                    {
                        posAngle -= MathHelper.Pi;
                    }

                    //Take the difference.
                    //Each blast zone is actually split in two down the middle: since left and right don't make sense on a circle they can be called the "clockwise" half, and "counter clockwise" half
                    //diff < pi / 4 means they're in the counter-clockwise half of one of them. diff > 3pi / 4 means they're in the clockwise half of the other.
                    float posDiff = Math.Abs(posAngle - workingAngle);
                    if (posDiff < MathHelper.PiOver4 || posDiff > (MathHelper.Pi - MathHelper.PiOver4))
                    {
                        if (Vector2.DistanceSquared(NPC.Center, p.Center) < confinedBlastsRadius * confinedBlastsRadius)
                        {
                            float damage = confinedBlastDamage;
                            damage *= (1 - p.endurance);
                            if (Main.expertMode)
                            {
                                damage += (int)Math.Ceiling(p.statDefense * 0.75f);
                            }
                            else
                            {
                                damage += (int)Math.Ceiling(p.statDefense * 0.5f);
                            }

                            p.Hurt(Terraria.DataStructures.PlayerDeathReason.ByCustomReason(p.name + " was shattered."), (int)damage, 1);
                        }
                    }
                }
            }
        }
    }

    float attackAngle;
    float count = 0;
    void FreezeBoltsAttack()
    {
        if (AttackModeTally == 0)
        {
            if ((AttackModeCounter - 40) % 20 == 0 && AttackModeCounter < 80 && count < 2)
            {
                float boltCount = 16;
                int speed = 10;
                for (float i = 0; i < boltCount; i++)
                {
                    Vector2 attackVel = new Vector2(speed, 0);
                    float tally = (AttackModeCounter - 40) / 20;
                    attackVel = attackVel.RotatedBy(MathHelper.TwoPi * (i / boltCount) + MathHelper.ToRadians(15 * tally));
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, attackVel, ModContent.ProjectileType<DarkFreezeBolt>(), freezeBoltDamage, 0.5f, Main.myPlayer);
                }
            }
        }

        if (AttackModeTally == 1)
        {
            float boltCount = 100;
            int speed = 8;
            //Store the angle 120 degrees counter clockwise of the target's current position
            if (AttackModeCounter == 120)
            {
                attackAngle = Target.Center.ToRotation() + MathHelper.ToRadians(120);
            }
            if ((AttackModeCounter >= 120 && AttackModeCounter < 360) && AttackModeCounter % 2 == 0)
            {
                attackAngle += MathHelper.ToRadians((180f / boltCount) * ((AttackModeCounter - 120) / 120f));
                Vector2 attackVel = new Vector2(speed, 0).RotatedBy(attackAngle);
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, attackVel.RotatedBy(i * MathHelper.TwoPi / 5), ModContent.ProjectileType<DarkFreezeBolt>(), freezeBoltDamage, 0.5f, Main.myPlayer);
                }
            }
        }

        if (AttackModeTally == 2)
        {
            if (AttackModeCounter >= 120 && AttackModeCounter < 390)
            {
                int speed = 11;
                if ((AttackModeCounter - 120) % 60 == 0)
                {
                    count = 0;
                    attackAngle = (Target.Center - NPC.Center).ToRotation() - MathHelper.ToRadians(45);
                }
                int step = (int)AttackModeCounter - 120;
                //Integer division is evil, but occasionally useful evil
                step -= (60 * (step / 60));

                if ((step % 5 == 0) && step < 25)
                {
                    count++;
                    Vector2 attackVel = new Vector2(speed, 0).RotatedBy(attackAngle + MathHelper.ToRadians(15 * count));
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, attackVel.RotatedBy(-MathHelper.PiOver4), ModContent.ProjectileType<DarkFreezeBolt>(), freezeBoltDamage, 0.5f, Main.myPlayer);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, attackVel, ModContent.ProjectileType<DarkFreezeBolt>(), freezeBoltDamage, 0.5f, Main.myPlayer);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, attackVel.RotatedBy(MathHelper.PiOver4), ModContent.ProjectileType<DarkFreezeBolt>(), freezeBoltDamage, 0.5f, Main.myPlayer);

                }
            }
        }
    }

    void ArrowRainAttack()
    {
        if (AttackModeCounter == 0 || AttackModeCounter % 40 == 20)
        {
            if (AttackModeTally % 2 == 0)
            {
                for (int i = 0; i < 13; i++)
                {
                    Vector2 offset = (i - 7) * new Vector2(1.05f, 1.05f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (arrowRainTargetingVector + offset), ModContent.ProjectileType<EnemyArrowOfDarkCloud>(), arrowRainDamage, 0.5f, Main.myPlayer);
                }
            }
            else
            {
                for (int i = 0; i < 13; i++)
                {
                    Vector2 velocity = UsefulFunctions.BallisticTrajectory(NPC.Center, Target.Center, 5 + i, 0.05f, true, false);
                    if (velocity != Vector2.Zero)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<EnemyArrowOfDarkCloud>(), arrowRainDamage, 0.5f, Main.myPlayer);
                    }
                }
            }
            //Vector2 projVelocity = UsefulFunctions.BallisticTrajectory(npc.Center, Target.Center, 10, 0.05f, true);

        }
    }

    void AntiMatAttack()
    {
        if (AttackModeCounter % 300 == 15)
        {
            for (int i = 0; i < 7; i++)
            {
                Vector2 pos = Main.rand.NextVector2CircularEdge(700, 700) + Target.Center;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X, (int)pos.Y, ModContent.NPCType<DarkCloudMirror>(), 0, DarkCloudAttackID.AntiMat, 60 + Main.rand.NextFloat(150));
            }
        }


        if (AttackModeCounter % 300 == 15)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Enemy.DarkCloud.AntimatTargeting>(), 0, 0.5f, Main.myPlayer, i, NPC.whoAmI);
            }
        }

        if (AttackModeCounter % 300 == 299)
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, UsefulFunctions.GenerateTargetingVector(NPC.Center, Target.Center, 14).RotatedBy(MathHelper.ToRadians(10)), ModContent.ProjectileType<DarkAntiMatRound>(), antiMatDamage, 0.5f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, UsefulFunctions.GenerateTargetingVector(NPC.Center, Target.Center, 16), ModContent.ProjectileType<DarkAntiMatRound>(), antiMatDamage, 0.5f, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, UsefulFunctions.GenerateTargetingVector(NPC.Center, Target.Center, 14).RotatedBy(MathHelper.ToRadians(-10)), ModContent.ProjectileType<DarkAntiMatRound>(), antiMatDamage, 0.5f, Main.myPlayer);
        }
    }

    void TeleportingSlashesAttack()
    {
        if (AttackModeCounter == 0)
        {
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DarkUltimaWeapon>(), ai0: NPC.whoAmI, ai2: DarkCloudAttackID.TeleportingSlashes);

            Vector2 spawnPoint = Target.Center + Main.rand.NextVector2CircularEdge(slashesWarpRadius, slashesWarpRadius);
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPoint.X, (int)spawnPoint.Y, ModContent.NPCType<DarkCloudMirror>(), ai0: DarkCloudAttackID.TeleportingSlashes);
        }
        if (AttackModeCounter % 20 == 0)
        {
            Vector2 spawnPoint = Target.Center + Main.rand.NextVector2CircularEdge(slashesWarpRadius, slashesWarpRadius);
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPoint.X, (int)spawnPoint.Y, ModContent.NPCType<DarkCloudMirror>(), ai0: DarkCloudAttackID.TeleportingSlashes);
        }
    }

    void BulletPortalsAttack()
    {

    }
    #endregion

    //Change from the first classic phase to the actual fight
    void ChangePhases()
    {
        if (!changingPhases)
        {
            InitializeMoves();
            if (testAttack == -1)
            {
                NextAttackMode = ActiveMoveList[Main.rand.Next(ActiveMoveList.Count)].ID;
                ChangeAttacks();
            }
            else
            {
                CurrentMove = ActiveMoveList[testAttack];
                NextAttackMode = testAttack;
            }

            PrecalculateFirstTeleport();
            changingPhases = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.aiStyle = 0;
        }

        for (int i = 0; i < 5; i++)
        {
            Vector2 dustPos = Main.LocalPlayer.Center;
            dustPos.X -= 1500;
            dustPos.Y -= 500;

            Dust newDust = Dust.NewDustDirect(dustPos, 3000, 1500, DustID.ShadowbeamStaff, 0, 0, Scale: 2);
            newDust.velocity = UsefulFunctions.GenerateTargetingVector(newDust.position, NPC.position, 40);
        }
        if (phaseChangeCounter <= 180)
        {
            NPC.velocity = Vector2.Zero;
            DarkCloudParticleEffect(-3, (float)30 * (float)(phaseChangeCounter / 180));
        }
        if (phaseChangeCounter == 180)
        {
            DarkCloudParticleEffect(12, 90);
        }
        if (phaseChangeCounter < 210)
        {
            for (int i = 0; i < 5; i++)
            {
                float intensity = (AttackModeCounter / 210);

                float radius = Main.rand.Next((int)(1 + 2000 * (1 - intensity)), (int)(2000 + 2000 * (1 - intensity)));
                DarkCloudParticleEffect(-10, 100, radius);
            }
        }
        if (phaseChangeCounter == 210)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                ActuatePyramid();
            }
            NPC.noTileCollide = true;
            NPC.velocity = new Vector2(0, -22);

            for (int i = 0; i < 500; i++)
            {
                Vector2 dustPos = Target.Center;
                dustPos.X -= 1500;
                dustPos.Y -= 500;
                Dust.NewDust(dustPos, 3000, 1500, DustID.ShadowbeamStaff, Main.rand.Next(-500, 500), Main.rand.Next(-500, 500), Scale: 5);
            }
        }
        if (phaseChangeCounter == 240)
        {
            NPC.velocity = Vector2.Zero;
            NPC.lifeMax = 500000 * Main.CurrentFrameFlags.ActivePlayersCount;
            NPC.life = NPC.lifeMax;
            changingPhases = false;
            NPC.dontTakeDamage = false;
            firstPhase = false;
        }
        phaseChangeCounter++;
    }

    #region Teleport Functions
    void DashToAroundPlayer()
    {
        //TODO: Implement
    }

    void TeleportToArenaCenter()
    {
        DarkCloudParticleEffect(-2);
        if (ModContent.GetInstance<tsorcRevampConfig>().AdventureMode)
        {
            NPC.Center = new Vector2(5827.5f, 1698) * 16;
        }
        else
        {
            Vector2 warpPoint = Target.Center;
            warpPoint.Y -= 600;
            NPC.Center = warpPoint;
        }
        DarkCloudParticleEffect(6);
        InstantNetUpdate();
    }
    #endregion

    //The dust ring particle effect the boss uses
    void DarkCloudParticleEffect(float dustSpeed, float dustAmount = 50, float radius = 64)
    {
        for (int i = 0; i < dustAmount; i++)
        {
            Vector2 offset = Main.rand.NextVector2CircularEdge(radius, radius);
            Vector2 velocity = new Vector2(dustSpeed, 0).RotatedBy(offset.ToRotation()) * Main.rand.NextFloat(2);
            Dust.NewDustPerfect(NPC.Center + offset, DustID.ShadowbeamStaff, velocity, Scale: 2).noGravity = true;
        }
    }

    //A charging effect that focuses in on dark cloud and grows in intensity as time goes on
    void ChargingParticleEffect(float progress, float maxProgress)
    {
        float count = (progress / maxProgress) * 30;
        DarkCloudParticleEffect(-5, count * 4, 42 - count);
    }

    //Same as above, but mixes in freeze bolt particles
    void IceChargingParticleEffect(float progress, float maxProgress)
    {
        ChargingParticleEffect(progress, maxProgress);

        float count = (progress / maxProgress) * 30;
        for (int i = 0; i < count * 4; i++)
        {
            Vector2 offset = Main.rand.NextVector2CircularEdge(35 - count, 35 - count);
            Vector2 velocity = new Vector2(-5, 0).RotatedBy(offset.ToRotation()) * Main.rand.NextFloat(2);
            Dust.NewDustPerfect(NPC.Center + offset, DustID.MagicMirror, velocity, Scale: 2).noGravity = true;
        }
    }

    //Nuke all grapples
    //Yes, this will also delete them for any players not fighting this boss
    //No, I don't care. It's not a big deal, and this runs for 1000 projectiles every frame. It needs to be fast.
    void NukeGrapples()
    {
        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            if (Main.projectile[i].aiStyle == 7)
            {
                Main.projectile[i].Kill();
            }
        }
    }

    //Tells the game to sync the NPC's data *now* instead of waiting until the end of AI() like npc.netUpdate = true;
    void InstantNetUpdate()
    {
        if (Main.netMode == NetmodeID.Server)
        {
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
        }
    }

    static Texture2D darkCloudTexture = (Texture2D)ModContent.Request<Texture2D>("tsorcRevamp/NPCs/Bosses/SuperHardMode/DarkCloud");
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (darkCloudTexture == null || darkCloudTexture.IsDisposed)
        {
            darkCloudTexture = (Texture2D)ModContent.Request<Texture2D>("tsorcRevamp/NPCs/Bosses/SuperHardMode/DarkCloud");
        }
        Rectangle sourceRectangle = new Rectangle(0, 0, darkCloudTexture.Width, darkCloudTexture.Height / Main.npcFrameCount[NPC.type]);
        Vector2 origin = sourceRectangle.Size() / 2f;
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (NPC.spriteDirection == 1)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
        for (float i = TRAIL_LENGTH - 1; i >= 0; i--)
        {
            Main.spriteBatch.Draw(darkCloudTexture, NPC.oldPos[(int)i] - Main.screenPosition + new Vector2(12, 16), sourceRectangle, drawColor * ((TRAIL_LENGTH - i) / TRAIL_LENGTH), NPC.rotation, origin, NPC.scale, spriteEffects, 0);
        }

        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!firstPhase && CurrentMove != null && CurrentMove.Draw != null)
        {
            CurrentMove.Draw(spriteBatch, drawColor);
        }
    }

    #region Draw Functions
    static Texture2D darkSparkTexture = (Texture2D)ModContent.Request<Texture2D>("tsorcRevamp/Projectiles/Enemy/DarkCloud/DarkCloudSpark");
    public void DivineSparkDraw(SpriteBatch spriteBatch, Color drawColor)
    {
        if (darkSparkTexture == null || darkSparkTexture.IsDisposed)
        {
            darkSparkTexture = (Texture2D)ModContent.Request<Texture2D>("tsorcRevamp/Projectiles/Enemy/DarkCloud/DarkCloudSpark");
        }
        float targetPoint;
        if ((AttackModeCounter % turnLength) > chargeTime && (AttackModeCounter % turnLength) < turnLength)
        {
            if (counterClockwise)
            {
                //The -4 fixes a rounding error and makes sure the sprite is aligned. Hacky, but w/e.
                targetPoint = initialTargetRotation - MathHelper.ToRadians(4 * (int)((AttackModeCounter % turnLength) - chargeTime) - 4);
            }
            else
            {
                targetPoint = initialTargetRotation + MathHelper.ToRadians(4 * (int)((AttackModeCounter % turnLength) - chargeTime) - 4);
            }
        }
        else
        {
            targetPoint = initialTargetRotation;
            Vector2 startPos = new Vector2(0, 160).RotatedBy(initialTargetRotation + MathHelper.ToRadians(-90));
            if (!Main.gamePaused)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 thisPos = NPC.Center + startPos + Main.rand.NextVector2Circular(50, 50);
                    Vector2 thisVel = UsefulFunctions.GenerateTargetingVector(thisPos, NPC.Center + Main.rand.NextVector2Circular(10, 10), 8);
                    Dust.NewDustPerfect(thisPos, DustID.FireworkFountain_Blue, thisVel).noGravity = true;
                }
                DarkCloudParticleEffect(-2, 1);
            }
        }
        //targetPoint = initialTargetRotation;
        Rectangle sourceRectangle = new Rectangle(0, 0, darkSparkTexture.Width, darkSparkTexture.Height);
        Vector2 origin = new Vector2(0, sourceRectangle.Height / 2);
        Main.spriteBatch.Draw(darkSparkTexture, NPC.Center - Main.screenPosition, sourceRectangle, drawColor, targetPoint, origin, NPC.scale, SpriteEffects.None, 0);
    }

    static Texture2D antimatTexture = (Texture2D)ModContent.Request<Texture2D>(ModContent.GetModItem(ModContent.ItemType<Items.Weapons.Ranged.Guns.AntimatRifle>()).Texture);
    public void AntiMatDraw(SpriteBatch spriteBatch, Color drawColor)
    {
        if (antimatTexture == null || darkSparkTexture.IsDisposed)
        {
            antimatTexture = (Texture2D)ModContent.Request<Texture2D>(ModContent.GetModItem(ModContent.ItemType<Items.Weapons.Ranged.Guns.AntimatRifle>()).Texture);
        }
        float targetPoint = UsefulFunctions.GenerateTargetingVector(NPC.Center, Target.Center, 1).ToRotation();
        if (!Main.gamePaused && (AttackModeCounter % 3 == 0))
        {
            Vector2 thisPos = NPC.Center + new Vector2(0, 128).RotatedBy(targetPoint - MathHelper.PiOver2) + Main.rand.NextVector2Circular(32, 32);
            Vector2 thisVel = UsefulFunctions.GenerateTargetingVector(thisPos, NPC.Center + Main.rand.NextVector2Circular(10, 10), 8);
            Dust.NewDustPerfect(thisPos, DustID.FireworkFountain_Red, thisVel, 100, default, 0.5f).noGravity = true;
        }


        Rectangle sourceRectangle = new Rectangle(0, 0, antimatTexture.Width, antimatTexture.Height);
        Vector2 origin = new Vector2(0, sourceRectangle.Height / 2);
        SpriteEffects theseEffects = (NPC.Center.X < Target.Center.X) ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Main.spriteBatch.Draw(antimatTexture, NPC.Center - Main.screenPosition, sourceRectangle, drawColor, targetPoint, origin, NPC.scale, theseEffects, 0f);
    }

    static Texture2D cernosTexture = (Texture2D)ModContent.Request<Texture2D>(ModContent.GetModItem(ModContent.ItemType<Items.Weapons.Ranged.Bows.CernosPrime>()).Texture);
    public void ArrowRainDraw(SpriteBatch spriteBatch, Color drawColor)
    {
        if (cernosTexture == null || darkSparkTexture.IsDisposed)
        {
            cernosTexture = (Texture2D)ModContent.Request<Texture2D>(ModContent.GetModItem(ModContent.ItemType<Items.Weapons.Ranged.Bows.CernosPrime>()).Texture);
        }
        float targetPoint = arrowRainTargetingVector.ToRotation();
        if (!Main.gamePaused && (AttackModeCounter % 80 == 20))
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 thisVel = arrowRainTargetingVector + Main.rand.NextVector2Circular(10, 10);
                Dust.NewDustPerfect(NPC.Center, DustID.FireworkFountain_Green, thisVel).noGravity = true;
            }
        }

        Rectangle sourceRectangle = new Rectangle(0, 0, cernosTexture.Width, cernosTexture.Height);
        Vector2 origin = new Vector2(0, sourceRectangle.Height / 2);
        SpriteEffects theseEffects = (NPC.Center.X < Target.Center.X) ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Main.spriteBatch.Draw(cernosTexture, NPC.Center - Main.screenPosition, sourceRectangle, drawColor, targetPoint, origin, NPC.scale, theseEffects, 0f);
    }
    #endregion

    void InitializeMoves(List<int> validMoves = null)
    {
        DefaultList = new List<DarkCloudMove> {
            new DarkCloudMove(DragoonLanceMove, DragoonLanceAttack, DarkCloudAttackID.DragoonLance, "Dragoon Lance"),
            new DarkCloudMove(DivineSparkMove, DivineSparkAttack, DarkCloudAttackID.DivineSpark, "Divine Spark", DivineSparkDraw),
            new DarkCloudMove(DarkFlowMove, DarkFlowAttack, DarkCloudAttackID.DarkFlow, "Dark Flow"),
            new DarkCloudMove(UltimaWeaponMove, UltimaWeaponAttack, DarkCloudAttackID.UltimaWeapon, "Ultima Weapon"),
            new DarkCloudMove(FreezeBoltsMove, FreezeBoltsAttack, DarkCloudAttackID.FreezeBolts, "Freeze Bolts"),
            new DarkCloudMove(AntiMatMove, AntiMatAttack, DarkCloudAttackID.AntiMat, "Anti-Mat", AntiMatDraw),
            new DarkCloudMove(ConfinedBlastsMove, ConfinedBlastsAttack, DarkCloudAttackID.ConfinedBlasts, "Confined Blasts"),
            new DarkCloudMove(ArrowRainMove, ArrowRainAttack, DarkCloudAttackID.ArrowRain, "Arrow Rain", ArrowRainDraw),
            new DarkCloudMove(TeleportingSlashesMove, TeleportingSlashesAttack, DarkCloudAttackID.TeleportingSlashes, "Teleporting Slashes"),
            ///new DarkCloudMove(BulletPortalsMove, BulletPortalsAttack, DarkCloudAttackID.BulletPortals),
            ///new DarkCloudMove(DivineSparkThirdsMove, DivineSparkThirdsAttack, DarkCloudAttackID.DivineSparkThirds),
            };

        ActiveMoveList = new List<DarkCloudMove>();
        List<DarkCloudMove> TempList = DefaultList;

        if (validMoves != null)
        {
            for (int i = 0; i < TempList.Count; i++)
            {
                if (validMoves.Contains(TempList[i].ID))
                {
                    ActiveMoveList.Add(TempList[i]);
                }
            }
        }
        else
        {
            ActiveMoveList = TempList;
        }
    }

    //Useful code from old AI to check if it's on the ground.
    bool OnGround()
    {
        bool standing_on_solid_tile = false;

        int y_below_feet = (int)(NPC.position.Y + (float)NPC.height + 8f) / 16;
        int x_left_edge = (int)NPC.position.X / 16;
        int x_right_edge = (int)(NPC.position.X + (float)NPC.width) / 16;
        for (int l = x_left_edge; l <= x_right_edge; l++) // check every block under feet
        {
            Tile t = Main.tile[l, y_below_feet];
            if (t.HasTile && !t.IsActuated && Main.tileSolid[(int)t.TileType]) // tile exists and is solid
            {
                standing_on_solid_tile = true;
                break; // one is enough so stop checking
            }
        } // END traverse blocks under feet
        return standing_on_solid_tile;
    }


    //Teleport itself and the player to the center of the pyramid
    public override bool PreKill()
    {
        if (ModContent.GetInstance<tsorcRevampConfig>().AdventureMode)
        {
            Vector2 pyramidCenter = new Vector2(5828, 1750) * 16;
            NPC.Center = pyramidCenter;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Vector2.Distance(Main.player[i].Center, NPC.Center) < 10000)
                {
                    Main.player[i].Center = pyramidCenter;
                }
            }

            //if (Target != null)
            //{
                for (int i = 0; i < 500; i++)
                {
                    Vector2 dustPos = Target.Center;
                    dustPos.X -= 1500;
                    dustPos.Y -= 500;
                    Dust.NewDust(dustPos, 3000, 1500, DustID.ShadowbeamStaff, Main.rand.Next(-500, 500), Main.rand.Next(-500, 500), Scale: 9);
                }
            //}

            DarkCloudParticleEffect(-12, 120, 64);
        }
        return true;
    }

   
    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.BossBag(ModContent.ItemType<Items.BossBags.DarkCloudBag>()));
    }

    public override void OnKill()
    {

        Dust.NewDust(NPC.position, NPC.width, NPC.height, 52, 0.3f, 0.3f, 200, default(Color), 1f);
        Dust.NewDust(NPC.position, NPC.height, NPC.width, 52, 0.2f, 0.2f, 200, default(Color), 3f);
        Dust.NewDust(NPC.position, NPC.width, NPC.height, 52, 0.2f, 0.2f, 200, default(Color), 3f);
        Dust.NewDust(NPC.position, NPC.height, NPC.width, 52, 0.2f, 0.2f, 200, default(Color), 3f);
        Dust.NewDust(NPC.position, NPC.height, NPC.width, 52, 0.2f, 0.2f, 200, default(Color), 2f);
        Dust.NewDust(NPC.position, NPC.width, NPC.height, 52, 0.2f, 0.2f, 200, default(Color), 4f);
        Dust.NewDust(NPC.position, NPC.height, NPC.width, 52, 0.2f, 0.2f, 200, default(Color), 2f);
        Dust.NewDust(NPC.position, NPC.height, NPC.width, 52, 0.2f, 0.2f, 200, default(Color), 2f);
        Dust.NewDust(NPC.position, NPC.height, NPC.width, 52, 0.2f, 0.2f, 200, default(Color), 4f);

        //Clean up projectiles
        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            if (Main.projectile[i].type == ModContent.ProjectileType<Projectiles.GenericLaser>() 
                || Main.projectile[i].type == ModContent.ProjectileType<DarkFlow>() 
                || Main.projectile[i].type == ModContent.ProjectileType<Projectiles.Enemy.DarkCloud.EnemyArrowOfDarkCloud>() 
                || Main.projectile[i].type == ModContent.ProjectileType<Projectiles.Enemy.DarkCloud.DarkFreezeBolt>())
            {
                Main.projectile[i].Kill();
            }
        }
        if (!Main.expertMode)
        {
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.GuardianSoul>());
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.Humanity>(), 3);
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.Weapons.Melee.Broadswords.MoonlightGreatsword>());
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<Items.Weapons.Summon.NullSpriteStaff>());

        }
        if (Main.tile[5810, 1670] != null)
        {
            if (Main.tile[5810, 1670].HasTile && Main.tile[5810, 1670].IsActuated)
            {
                DeactuatePyramid();
            }
        }
        UsefulFunctions.BroadcastText("You have subsumed your shadow...", Color.Blue);
    }

    #region Debuffs
    public override void OnHitPlayer(Player player, int damage, bool crit)
    {
        int expertScale = 1;
        if (Main.expertMode) expertScale = 2;


        player.AddBuff(BuffID.BrokenArmor, 120 / expertScale, false); //broken armor
        player.AddBuff(BuffID.OnFire, 180 / expertScale, false); //on fire!
        player.AddBuff(ModContent.BuffType<FracturingArmor>(), 3600, false); //defense goes time on every hit

    }
    #endregion

    #region Pyramid
    public static int[,] Lanterns = new int[9, 2] { { 5824, 1715 }, { 5833, 1715 }, { 5824, 1732 }, { 5833, 1732 }, { 5824, 1749 }, { 5833, 1749 }, { 5824, 1766 }, { 5832, 1766 }, { 5828, 1766 } };
    public static int[,] Bulbs = new int[6, 2] { { 5821, 1686 }, { 5823, 1684 }, { 5826, 1682 }, { 5829, 1682 }, { 5832, 1684 }, { 5834, 1686 } };
    public static int[,] HarpyStatues = new int[2, 2] { { 5824, 1764 }, { 5831, 1764 } };
    public static void ActuatePyramid()
    {
        if (ModContent.GetInstance<tsorcRevampConfig>().AdventureMode)
        {

            //Destroy Lanterns (doing it like this prevents tiles from doing annoying things like dropping an item or spawning a boss)
            for (int i = 0; i < 9; i++)
            {
                if (Main.tile[Lanterns[i, 0], Lanterns[i, 1]].TileType == TileID.HangingLanterns)
                {
                    Main.tile[Lanterns[i, 0], Lanterns[i, 1]].ClearTile();
                    Main.tile[Lanterns[i, 0], Lanterns[i, 1] + 1].ClearTile();
                    //WorldGen.KillTile(Lanterns[i, 0], Lanterns[i, 1], noItem: true);
                    //WorldGen.KillTile(Lanterns[i, 0], Lanterns[i, 1] + 1, noItem: true);
                }
            }

            //Bulbs
            for (int i = 0; i < 6; i++)
            {
                if (Main.tile[Bulbs[i, 0], Bulbs[i, 1]].TileType == TileID.PlanteraBulb)
                {
                    Main.tile[Bulbs[i, 0], Bulbs[i, 1]].ClearTile();
                    Main.tile[Bulbs[i, 0], Bulbs[i, 1] - 1].ClearTile();
                    Main.tile[Bulbs[i, 0] + 1, Bulbs[i, 1]].ClearTile();
                    Main.tile[Bulbs[i, 0] + 1, Bulbs[i, 1] - 1].ClearTile();

                    //WorldGen.PlaceTile(Bulbs[i, 0], Bulbs[i, 1], TileID.Meteorite);
                }
            }

            //Harpy statues
            for (int i = 0; i < 2; i++)
            {
                Main.tile[HarpyStatues[i, 0], HarpyStatues[i, 1]].ClearTile();
                Main.tile[HarpyStatues[i, 0], HarpyStatues[i, 1] - 1].ClearTile();
                Main.tile[HarpyStatues[i, 0], HarpyStatues[i, 1] - 2].ClearTile();
                Main.tile[HarpyStatues[i, 0] + 1, HarpyStatues[i, 1]].ClearTile();
                Main.tile[HarpyStatues[i, 0] + 1, HarpyStatues[i, 1] - 1].ClearTile();
                Main.tile[HarpyStatues[i, 0] + 1, HarpyStatues[i, 1] - 2].ClearTile();
            }

            //Base of the pyramid
            for (int x = 5697; x < 5937; x++)
            {
                for (int y = 1696; y < 1773; y++)
                {
                    if (!Main.tile[x, y].IsActuated)
                    {
                        Wiring.ActuateForced(x, y);
                    }
                }
            }

            //109, 43
            //Middle of the pyramid
            for (int x = 5774; x < 5883; x++)
            {
                for (int y = 1653; y < 1696; y++)
                {
                    if (!Main.tile[x, y].IsActuated)
                    {
                        Wiring.ActuateForced(x, y);
                    }
                }
            }

            //Tip of the pyramid
            for (int x = 5814; x < 5843; x++)
            {
                for (int y = 1638; y < 1653; y++)
                {
                    if (!Main.tile[x, y].IsActuated)
                    {
                        Wiring.ActuateForced(x, y);
                    }
                }
            }

            //Covering the gap on the left
            int offset = 0;
            for (int y = 1773; y < 1777; y++)
            {
                for (int x = 5740; x < 5748; x++)
                {
                    if (Main.tile[x + offset, y].TileType == TileID.SandstoneBrick)
                    {
                        WorldGen.KillTile(x + offset, y, noItem: true);
                    }
                }
                offset++;
            }
        }
    }
    public static void DeactuatePyramid()
    {
        if (ModContent.GetInstance<tsorcRevampConfig>().AdventureMode)
        {
            //Base of the pyramid
            for (int x = 5697; x < 5937; x++)
            {
                for (int y = 1696; y < 1773; y++)
                {
                    if (Main.tile[x, y].IsActuated)
                    {
                        Wiring.ActuateForced(x, y);
                    }
                }
            }
            //109, 43
            //Middle of the pyramid
            for (int x = 5774; x < 5883; x++)
            {
                for (int y = 1653; y < 1696; y++)
                {
                    if (Main.tile[x, y].IsActuated)
                    {
                        Wiring.ActuateForced(x, y);
                    }
                }
            }

            //Tip of the pyramid
            for (int x = 5814; x < 5843; x++)
            {
                for (int y = 1638; y < 1653; y++)
                {
                    if (Main.tile[x, y].IsActuated)
                    {
                        Wiring.ActuateForced(x, y);
                    }
                }
            }

            //Covering the gap on the left
            int offset = 0;
            for (int y = 1773; y < 1777; y++)
            {
                for (int x = 5740; x < 5748; x++)
                {
                    if (Main.tile[x + offset, y].TileType != TileID.SandstoneBrick)
                    {
                        WorldGen.PlaceTile(x + offset, y, TileID.SandstoneBrick, true, true);
                    }
                }
                offset++;
            }

            //Place lanterns (yes, they have to be seperate from breaking because Place[X] functions respect anchor points)
            for (int i = 0; i < 8; i++)
            {
                if (Main.tile[Lanterns[i, 0], Lanterns[i, 1]].TileType != TileID.HangingLanterns)
                {
                    WorldGen.Place1x2Top(Lanterns[i, 0], Lanterns[i, 1], TileID.HangingLanterns, 24);
                }
            }

            //And the bulbs
            for (int i = 0; i < 6; i++)
            {
                if (Main.tile[Bulbs[i, 0], Bulbs[i, 1]].TileType != TileID.PlanteraBulb)
                {
                    WorldGen.Place2x2(Bulbs[i, 0] + 1, Bulbs[i, 1], TileID.PlanteraBulb, 0);
                }
            }

            //And the harpy statues
            for (int i = 0; i < 2; i++)
            {
                WorldGen.Place2xX(HarpyStatues[i, 0], HarpyStatues[i, 1], TileID.Statues, 70);
            }
        }
    }
    #endregion

    #region Old AI
    public void FirstPhase()
    {
        #region "Classic" first phase AI // code by GrtAndPwrflTrtl (http://www.terrariaonline.com/members/grtandpwrfltrtl.86018/)


        #region set up NPC's attributes & behaviors
        // set parameters
        //  is_archer OR can_pass_doors OR shoot_and_walk, pick only 1.  They use the same ai[] vars (1&2)
        bool is_archer = false; // stops and shoots when target sighted; skel archer & gob archer are the archers
        bool can_pass_doors = false;  //  can open or break doors; c. bunny, crab, clown, skel archer, gob archer, & chaos elemental cannot
        bool shoot_and_walk = true;  //  can shoot while walking like clown; uses ai[2] so cannot be used with is_archer or can_pass_doors

        //  can_teleport==true code uses boredom_time and ai[3] (boredom), but not mutually exclusive
        bool can_teleport = true;  //  tp around like chaos ele
        int boredom_time = 20; // time until it stops targeting player if blocked etc, 60 for anything but chaos ele, 20 for chaos ele
        int boredom_cooldown = 10 * boredom_time; // boredom level where boredom wears off; usually 10*boredom_time

        bool hates_light = false;  //  flees in daylight like: Zombie, Skeleton, Undead Miner, Doctor Bones, The Groom, Werewolf, Clown, Bald Zombie, Possessed Armor
        bool can_pass_doors_bloodmoon_only = false;  //  can open or break doors, but only during bloodmoon: zombies & bald zombies. Will keep trying anyway.

        float acceleration = .05f;  //  how fast it can speed up
        float top_speed = 3f;  //  max walking speed, also affects jump length
        float braking_power = .2f;  //  %of speed that can be shed every tick when above max walking speed
        double bored_speed = .9;  //  above this speed boredom decreases(if not already bored); usually .9

        float enrage_percentage = .4f;  //  double movement speed below this life fraction. 0 for no enrage. Mummies enrage below .5
        float enrage_acceleration = .10f;  //  faster when enraged, usually 2*acceleration
        float enrage_top_speed = 5;  //  faster when enraged, usually 2*top_speed

        bool clown_sized = false; // is hitbox the same as clowns' for purposes of when to jump?
        bool jump_gaps = true; // attempt to jump gaps; everything but crabs do this

        bool hops = true; // hops when close to target like Angry Bones, Corrupt Bunny, Armored Skeleton, and Werewolf
        float hop_velocity = 1f; // forward velocity needed to initiate hopping; usually 1
        float hop_range_x = 100; // less than this is 'close to target'; usually 100
        float hop_range_y = 50; // less than this is 'close to target'; usually 50
        float hop_power = 4; // how hard/high offensive hops are; usually 4
        float hop_speed = 3; // how fast hops can accelerate vertically; usually 3 (2xSpd is 4 for Hvy Skel & Werewolf so they're noticably capped)

        // is_archer & clown bombs only
        int shot_rate = 70;  //  rate at which archers/bombers fire; 70 for skeleton archer, 180 for goblin archer, 450 for clown; atm must be an even # or won't fire at shot_rate/2
                             //int fuse_time = 300;  //  fuse time on bombs, 300 for clown bombs
                             //int projectile_damage = 35;  //  projectile dmg: 35 for Skeleton Archer, 11 for Goblin Archer
        int projectile_id = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellMeteor>(); // projectile id: 82(Flaming Arrow) for Skeleton Archer, 81(Wooden Arrow) for Goblin Archer, 75(Happy Bomb) for Clown
        float projectile_velocity = 11; // initial velocity? 11 for Skeleton Archers, 9 for Goblin Archers, bombs have fixed speed & direction atm

        // can_pass_doors only
        float door_break_pow = 2; // 10 dmg breaks door; 2 for goblin thief and 7 for Angry Bones; 1 for others
        bool breaks_doors = false; // meaningless unless can_pass_doors; if this is true the door breaks down instead of trying to open; Goblin Peon is only warrior to do this

        // Omnirs creature sorts
        //bool tooBig = true; // force bigger creatures to jump
        //bool lavaJumping = true; // Enemies jump on lava.
        bool canDrown = false; // They will drown if in the water for too long
        bool quickBored = true; //Enemy will respond to boredom much faster(? -- test)
        bool oBored = false; //Whether they're bored under the "quickBored" conditions

        // calculated parameters
        bool moonwalking = false;  //  not jump/fall and moving backwards to facing
        if (NPC.velocity.Y == 0f && ((NPC.velocity.X > 0f && NPC.direction < 0) || (NPC.velocity.X < 0f && NPC.direction > 0)))
            moonwalking = true;
        #endregion
        //-------------------------------------------------------------------
        #region teleportation particle effects
        if (can_teleport)  //  chaos elemental type teleporter
        {
            if (NPC.ai[3] == -120f)  //  boredom goes negative? I think this makes disappear/arrival effects after it just teleported
            {
                NPC.velocity *= 0f; // stop moving
                NPC.ai[3] = 0f; // reset boredom to 0
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                Vector2 vector = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f); // current location
                float num6 = NPC.oldPos[2].X + (float)NPC.width * 0.5f - vector.X; // direction to where it was 3 frames ago?
                float num7 = NPC.oldPos[2].Y + (float)NPC.height * 0.5f - vector.Y; // direction to where it was 3 frames ago?
                float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7)); // distance to where it was 3 frames ago?
                num8 = 2f / num8; // to normalize to 2 unit long vector
                num6 *= num8; // direction to where it was 3 frames ago, vector normalized
                num7 *= num8; // direction to where it was 3 frames ago, vector normalized
                for (int j = 0; j < 20; j++) // make 20 dusts at current position
                {
                    int num9 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 71, num6, num7, 200, default(Color), 2f);
                    Main.dust[num9].noGravity = true; // floating
                    Dust expr_19EE_cp_0 = Main.dust[num9]; // make a dust handle?
                    expr_19EE_cp_0.velocity.X *= 2f; // faster in x direction
                }
                for (int k = 0; k < 20; k++) // more dust effects at old position
                {
                    int num10 = Dust.NewDust(NPC.oldPos[2], NPC.width, NPC.height, 71, -num6, -num7, 200, default(Color), 2f);
                    Main.dust[num10].noGravity = true;
                    Dust expr_1A6F_cp_0 = Main.dust[num10];
                    expr_1A6F_cp_0.velocity.X *= 2f;
                }
            } // END just teleported
        } // END can teleport
        #endregion
        //-------------------------------------------------------------------
        #region adjust boredom level
        if (!is_archer || NPC.ai[2] <= 0f)  //  loop to set ai[3] (boredom)
        {
            if (NPC.position.X == NPC.oldPosition.X || NPC.ai[3] >= (float)boredom_time || moonwalking)  //  stopped or bored or moonwalking
                NPC.ai[3] += 1f; // increase boredom
            else if ((double)Math.Abs(NPC.velocity.X) > bored_speed && NPC.ai[3] > 0f)  //  moving fast and not bored
                NPC.ai[3] -= 1f; // decrease boredom

            if (NPC.justHit || NPC.ai[3] > boredom_cooldown)
                NPC.ai[3] = 0f; // boredom wears off if enough time passes, or if hit

            if (NPC.ai[3] == (float)boredom_time)
                NPC.netUpdate = true; // netupdate when state changes to bored
        }
        #endregion
        //-------------------------------------------------------------------
        #region play creature sounds, target/face player, respond to boredom
        if ((!hates_light || !Main.dayTime || (double)NPC.position.Y > Main.worldSurface * 16.0) && NPC.ai[3] < (float)boredom_time)
        {  // not fleeing light & not bored
            if (!canDrown || (canDrown && !NPC.wet) || (quickBored && boredTimer > tBored))
            {
                //npc.TargetClosest(true); //  Target the closest player & face him (If passed as a parameter, a bool will determine whether it should face the target or not)
            }
        }
        else if (!is_archer || NPC.ai[2] <= 0f) //  fleeing light or bored (& not aiming)
        {
            if (hates_light && Main.dayTime && (double)(NPC.position.Y / 16f) < Main.worldSurface && NPC.timeLeft > 10)
                //npc.timeLeft = 10;  //  if hates light & in light, hasten despawn

                if (NPC.velocity.X == 0f)
                {
                    if (NPC.velocity.Y == 0f)
                    { // not moving
                        if (NPC.ai[0] == 0f)
                            NPC.ai[0] = 1f; // facing change delay
                        else
                        { // change movement and facing direction, reset delay
                            NPC.direction *= -1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.ai[0] = 0f;
                        }
                    }
                }
                else // moving in x direction,
                    NPC.ai[0] = 0f; // reset facing change delay

            if (NPC.direction == 0) // what does it mean if direction is 0?
                NPC.direction = 1; // flee right if direction not set? or is initial direction?
        } // END fleeing light or bored (& not aiming)
        #endregion
        //-------------------------------------------------------------------
        #region enrage
        bool enraged = false; // angry from damage; not stored from tick to tick
        if ((enrage_percentage > 0) && (NPC.life < (float)NPC.lifeMax * enrage_percentage))  //  speed up at low life
            enraged = true;
        if (enraged)
        { // speed up movement if enraged
            acceleration = enrage_acceleration;
            top_speed = enrage_top_speed;
        }
        #endregion
        //-------------------------------------------------------------------
        #region melee movement

        int dust = Dust.NewDust(new Vector2((float)NPC.position.X, (float)NPC.position.Y), NPC.width, NPC.height, 6, NPC.velocity.X - 6f, NPC.velocity.Y, 150, Color.Blue, 1f + comboDamage / 500);
        Main.dust[dust].noGravity = true;



        if (!is_archer || (NPC.ai[2] <= 0f && !NPC.confused))  //  meelee attack/movement. archers only use while not aiming
        {
            if (Math.Abs(NPC.velocity.X) > top_speed)  //  running/flying faster than top speed
            {
                if (NPC.velocity.Y == 0f)  //  and not jump/fall
                    NPC.velocity *= (1f - braking_power);  //  decelerate
            }
            else if ((NPC.velocity.X < top_speed && NPC.direction == 1) || (NPC.velocity.X > -top_speed && NPC.direction == -1))
            {  //  running slower than top speed (forward), can be jump/fall
                if (can_teleport && moonwalking)
                    NPC.velocity.X = NPC.velocity.X * 0.99f;  //  ? small decelerate for teleporters

                NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * acceleration;  //  accellerate fwd; can happen midair
                if ((float)NPC.direction * NPC.velocity.X > top_speed)
                    NPC.velocity.X = (float)NPC.direction * top_speed;  //  but cap at top speed
            }  //  END running slower than top speed (forward), can be jump/fall
        } // END non archer or not aiming*/
        #endregion
        //-------------------------------------------------------------------
        #region archer projectile code (stops moving to shoot)
        if (is_archer)
        {
            if (NPC.confused)
                NPC.ai[2] = 0f; // won't try to stop & aim if confused
            else // not confused
            {
                if (NPC.ai[1] > 0f)
                    NPC.ai[1] -= 1f; // decrement fire & reload counter

                if (NPC.justHit) // was just hit?
                {
                    NPC.ai[1] = 30f; // shot on .5 sec cooldown
                    NPC.ai[2] = 0f; // not aiming
                }
                if (NPC.ai[2] > 0f) // if aiming: adjust aim and fire if needed
                {
                    //npc.TargetClosest(true); // target and face closest player
                    if (NPC.ai[1] == (float)(shot_rate / 2))  //  fire at halfway through; first half of delay is aim, 2nd half is cooldown
                    { // firing:
                        Vector2 npc_center = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f); // npc position
                        float npc_to_target_x = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - npc_center.X; // x vector to target
                        float num16 = Math.Abs(npc_to_target_x) * 0.1f; // 10% of x distance to target: to aim high if farther?
                        float npc_to_target_y = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - npc_center.Y - num16; // y vector to target (aiming high at distant targets)
                        npc_to_target_x += (float)Main.rand.Next(-40, 41); //  targeting error: 40 pix=2.5 blocks
                        npc_to_target_y += (float)Main.rand.Next(-40, 41); //  targeting error: 40 pix=2.5 blocks
                        float target_dist = (float)Math.Sqrt((double)(npc_to_target_x * npc_to_target_x + npc_to_target_y * npc_to_target_y)); // distance to target
                        NPC.netUpdate = true; // ??
                        target_dist = projectile_velocity / target_dist; // to normalize by projectile_velocity
                        npc_to_target_x *= target_dist; // normalize by projectile_velocity
                        npc_to_target_y *= target_dist; // normalize by projectile_velocity
                        npc_center.X += npc_to_target_x;  //  initial projectile position includes one tick of initial movement
                        npc_center.Y += npc_to_target_y;  //  initial projectile position includes one tick of initial movement
                        if (Main.netMode != 1)  //  is server
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), npc_center.X, npc_center.Y, npc_to_target_x, npc_to_target_y, projectile_id, meteorDamage, 0f, Main.myPlayer);

                        if (Math.Abs(npc_to_target_y) > Math.Abs(npc_to_target_x) * 2f) // target steeply above/below NPC
                        {
                            if (npc_to_target_y > 0f)
                                NPC.ai[2] = 1f; // aim downward
                            else
                                NPC.ai[2] = 5f; // aim upward
                        }
                        else if (Math.Abs(npc_to_target_x) > Math.Abs(npc_to_target_y) * 2f) // target on level with NPC
                            NPC.ai[2] = 3f;  //  aim straight ahead
                        else if (npc_to_target_y > 0f) // target is below NPC
                            NPC.ai[2] = 2f;  //  aim slight downward
                        else // target is not below NPC
                            NPC.ai[2] = 4f;  //  aim slight upward
                    } // END firing
                    if (NPC.velocity.Y != 0f || NPC.ai[1] <= 0f) // jump/fall or firing reload
                    {
                        NPC.ai[2] = 0f; // not aiming
                        NPC.ai[1] = 0f; // reset firing/reload counter (necessary? nonzero maybe)
                    }
                    else // no jump/fall and no firing reload
                    {
                        NPC.velocity.X = NPC.velocity.X * 0.9f; // decelerate to stop & shoot
                        NPC.spriteDirection = NPC.direction; // match animation to facing
                    }
                } // END if aiming: adjust aim and fire if needed
                if (NPC.ai[2] <= 0f && NPC.velocity.Y == 0f && NPC.ai[1] <= 0f && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                { // not aiming & no jump/fall & fire/reload ctr is 0 & target is alive and LOS to target: start aiming
                    float num21 = 10f; // dummy vector length in place of initial velocity? not sure why this is needed
                    Vector2 npc_center = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float npc_to_target_x = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - npc_center.X;
                    float num23 = Math.Abs(npc_to_target_x) * 0.1f; // 10% of x distance to target: to aim high if farther?
                    float npc_to_target_y = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - npc_center.Y - num23; // y vector to target (aiming high at distant targets)
                    npc_to_target_x += (float)Main.rand.Next(-40, 41);
                    npc_to_target_y += (float)Main.rand.Next(-40, 41);
                    float target_dist = (float)Math.Sqrt((double)(npc_to_target_x * npc_to_target_x + npc_to_target_y * npc_to_target_y));
                    if (target_dist < 700f) // 700 pix = 43.75 blocks
                    { // target is in range
                        NPC.netUpdate = true; // ??
                        NPC.velocity.X = NPC.velocity.X * 0.5f; // hard brake
                        target_dist = num21 / target_dist; // to normalize by num21
                        npc_to_target_x *= target_dist; // normalize by num21
                        npc_to_target_y *= target_dist; // normalize by num21
                        NPC.ai[2] = 3f; // aim straight ahead
                        NPC.ai[1] = (float)shot_rate; // start fire & reload counter
                        if (Math.Abs(npc_to_target_y) > Math.Abs(npc_to_target_x) * 2f) // target steeply above/below NPC
                        {
                            if (npc_to_target_y > 0f)
                                NPC.ai[2] = 1f; // aim downward
                            else
                                NPC.ai[2] = 5f; // aim upward
                        }
                        else if (Math.Abs(npc_to_target_x) > Math.Abs(npc_to_target_y) * 2f) // target on level with NPC
                            NPC.ai[2] = 3f; // aim straight ahead
                        else if (npc_to_target_y > 0f)
                            NPC.ai[2] = 2f; // aim slight downward
                        else
                            NPC.ai[2] = 4f; // aim slight upward
                    } // END target is in range
                } // END start aiming
            } // END not confused
        }  //  END is archer
        #endregion
        //-------------------------------------------------------------------


        #region shoot and walk
        if (!oBored && shoot_and_walk && !Main.player[NPC.target].dead) // can generalize this section to moving+projectile code 
        {
            // Main.netMode != 1 &&

            #region Charge
            //if(Main.netMode != 1)
            //{
            if (breakCombo == true || (enraged == true && Main.rand.NextBool(700)) || (enraged == false && Main.rand.NextBool(1700)))
            {
                chargeDamageFlag = true;
                Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                float rotation = (float)Math.Atan2(vector8.Y - (Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)), vector8.X - (Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)));
                NPC.velocity.X = (float)(Math.Cos(rotation) * 10) * -1;
                NPC.velocity.Y = (float)(Math.Sin(rotation) * 10) * -1;
                NPC.knockBackResist = 0f;
                breakCombo = false;
                NPC.netUpdate = true;
            }
            if (chargeDamageFlag == true)
            {
                NPC.damage = 120;
                NPC.knockBackResist = 0;
                chargeDamage++;
            }
            if (chargeDamage >= 96)
            {
                chargeDamageFlag = false;
                NPC.damage = 95;
                NPC.knockBackResist = 0.2f;
                chargeDamage = 0;
            }

            //}
            #endregion

            #region Projectiles
            //if(Main.netMode != 1)
            //{
            customAi1 += (Main.rand.Next(2, 5) * 0.1f) * NPC.scale;
            if (customAi1 >= 10f)
            {
                //npc.TargetClosest(true);
                if ((customspawn1 < 1) && Main.rand.NextBool(1000))
                {
                    int Spawned = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<NPCs.Enemies.SuperHardMode.CrystalKnight>(), 0);
                    Main.npc[Spawned].velocity.Y = -8;
                    Main.npc[Spawned].velocity.X = Main.rand.Next(-10, 10) / 10;
                    NPC.ai[0] = 20 - Main.rand.Next(80);
                    customspawn1 += 1f;
                    if (Main.netMode != 1)
                    {
                        NetMessage.SendData(23, -1, -1, null, Spawned, 0f, 0f, 0f, 0);
                    }
                }
                if ((customspawn2 < 2) && Main.rand.NextBool(3500))
                {
                    int Spawned = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<NPCs.Enemies.SuperHardMode.BarrowWightNemesis>(), 0);
                    Main.npc[Spawned].velocity.Y = -8;
                    Main.npc[Spawned].velocity.X = Main.rand.Next(-10, 10) / 10;
                    NPC.ai[0] = 20 - Main.rand.Next(80);
                    customspawn2 += 1f;
                    if (Main.netMode != 1)
                    {
                        NetMessage.SendData(23, -1, -1, null, Spawned, 0f, 0f, 0f, 0);
                    }
                }



                if ((customspawn3 < 0) && Main.rand.NextBool(9950))
                {
                    int Spawned = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<NPCs.Enemies.Assassin>(), 0);
                    Main.npc[Spawned].velocity.Y = -8;
                    Main.npc[Spawned].velocity.X = Main.rand.Next(-10, 10) / 10;
                    NPC.ai[0] = 20 - Main.rand.Next(80);
                    customspawn3 += 1f;
                    if (Main.netMode != 1)
                    {
                        NetMessage.SendData(23, -1, -1, null, Spawned, 0f, 0f, 0f, 0);
                    }
                }



                if (Main.rand.NextBool(700))
                {
                    float num48 = 10f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellSuddenDeathBall>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, deathBallDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 6;
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }

                if (Main.rand.NextBool(195))
                {
                    float num48 = 13f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellAbyssPoisonStrikeBall>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, poisonStrikeDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 700;
                        Main.projectile[num54].aiStyle = 23;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }


                if (Main.rand.NextBool(520))
                {
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float rotation = (float)Math.Atan2(vector8.Y - (Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)), vector8.X - (Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)));
                    NPC.velocity.X = (float)(Math.Cos(rotation) * 14) * -1;
                    NPC.velocity.Y = (float)(Math.Sin(rotation) * 14) * -1;
                    NPC.ai[1] = 1f;
                    NPC.netUpdate = true;
                }
                if (Main.rand.NextBool(340))
                {
                    float num48 = 18f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y - 100 + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellHoldBall>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, holdBallDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 105;
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        NPC.ai[1] = 1f;
                    }
                    NPC.netUpdate = true;
                }



                if (Main.rand.NextBool(120))
                {
                    float num48 = 13f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.Center.Y - 10f);
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = (((Main.player[NPC.target].position.Y - 10) + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemyDragoonLance>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, dragoonLanceDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 700;
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }



                if (Main.rand.NextBool(300))
                {
                    float num48 = 15f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellAbyssPoisonStrikeBall>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, poisonStrikeDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 600;
                        Main.projectile[num54].aiStyle = 23;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        NPC.ai[1] = 1f;
                    }
                    NPC.netUpdate = true;
                }



                if (Main.rand.NextBool(85))
                {
                    float num48 = 12f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-10, 20);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-10, 30);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        //int damage = 80;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellAbyssPoisonStrikeBall>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, poisonStrikeDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 450;
                        Main.projectile[num54].aiStyle = 23;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }


                if (Main.rand.NextBool(350))
                {
                    float num48 = 12f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-10, 20);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-10, 30);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellArmageddonBlastBall>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, armageddonDamage, 0f, Main.myPlayer);
                        //Main.projectile[num54].timeLeft = 0;
                        Main.projectile[num54].aiStyle = 23;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }



                if (Main.rand.NextBool(70))
                {
                    float num48 = 14f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellGravity1Ball>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, gravityBallDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 40;
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        NPC.ai[1] = 1f;
                    }
                    NPC.netUpdate = true;
                }
                if (Main.rand.NextBool(280))
                {
                    float num48 = 11f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellAbyssPoisonStrikeBall>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, poisonStrikeDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 270;
                        Main.projectile[num54].aiStyle = 23;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }
                if (Main.rand.NextBool(350))
                {
                    float num48 = 13f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y - 1000 + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.CrazedPurpleCrush>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, crazedPurpleCrushDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 600;
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }











                if (Main.rand.NextBool(526))
                {
                    float num48 = 7f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.ShadowShot>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, shadowShotDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 200;
                        Main.projectile[num54].aiStyle = 23; //was 23
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }

                if (Main.rand.NextBool(50))
                {
                    float num48 = 8f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellIcestormBall>();//44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, iceStormDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 0;//was 70
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        NPC.ai[1] = 1f;
                    }
                    NPC.netUpdate = true;
                }





                if (Main.rand.NextBool(65))
                {
                    float num48 = 13f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.DarkCloud.EnemyArrowOfDarkCloud>(); //44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, darkArrowDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 1300;
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }


                if (Main.rand.NextBool(555))
                {
                    float num48 = 13f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.EnemySpellAbyssStormWave>(); //44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, stormWaveDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 1300;
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }



                if (Main.rand.NextBool(205))
                {
                    float num48 = 15f;
                    Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height / 2));
                    float speedX = ((Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)) - vector8.X) + Main.rand.Next(-20, 0x15);
                    float speedY = ((Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)) - vector8.Y) + Main.rand.Next(-20, 0x15);
                    if (((speedX < 0f) && (NPC.velocity.X < 0f)) || ((speedX > 0f) && (NPC.velocity.X > 0f)))
                    {
                        float num51 = (float)Math.Sqrt((double)((speedX * speedX) + (speedY * speedY)));
                        num51 = num48 / num51;
                        speedX *= num51;
                        speedY *= num51;
                        int type = ModContent.ProjectileType<Projectiles.Enemy.DarkCloud.EnemyArrowOfDarkCloud>(); //44;//0x37; //14;
                        int num54 = Projectile.NewProjectile(NPC.GetSource_FromThis(), vector8.X, vector8.Y, speedX, speedY, type, darkArrowDamage, 0f, Main.myPlayer);
                        Main.projectile[num54].timeLeft = 1300;
                        Main.projectile[num54].aiStyle = 1;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                        customAi1 = 1f;
                    }
                    NPC.netUpdate = true;
                }
            }

            // } //end of MP thing

            #endregion
        }

        #endregion


        //-------------------------------------------------------------------
        #region check if standing on a solid tile
        // warning: this section contains a return statement
        bool standing_on_solid_tile = false;
        if (NPC.velocity.Y == 0f) // no jump/fall
        {
            int y_below_feet = (int)(NPC.position.Y + (float)NPC.height + 8f) / 16;
            int x_left_edge = (int)NPC.position.X / 16;
            int x_right_edge = (int)(NPC.position.X + (float)NPC.width) / 16;
            for (int l = x_left_edge; l <= x_right_edge; l++) // check every block under feet
            {
                if (Main.tile[l, y_below_feet] == null) // null tile means ??
                    return;

                if (Main.tile[l, y_below_feet].HasTile && Main.tileSolid[(int)Main.tile[l, y_below_feet].TileType]) // tile exists and is solid
                {
                    standing_on_solid_tile = true;
                    break; // one is enough so stop checking
                }
            } // END traverse blocks under feet
        } // END no jump/fall
        #endregion
        //-------------------------------------------------------------------
        #region new Tile()s, door opening/breaking
        if (standing_on_solid_tile)  //  if standing on solid tile
        {
            int x_in_front = (int)((NPC.position.X + (float)(NPC.width / 2) + (float)(15 * NPC.direction)) / 16f); // 15 pix in front of center of mass
            int y_above_feet = (int)((NPC.position.Y + (float)NPC.height - 15f) / 16f); // 15 pix above feet
            if (clown_sized)
                x_in_front = (int)((NPC.position.X + (float)(NPC.width / 2) + (float)((NPC.width / 2 + 16) * NPC.direction)) / 16f); // 16 pix in front of edge
                                                                                                                                     //  create? 5 tile high stack in front
            if (Main.tile[x_in_front, y_above_feet] == null)
                Main.tile[x_in_front, y_above_feet].ClearTile();

            if (Main.tile[x_in_front, y_above_feet - 1] == null)
                Main.tile[x_in_front, y_above_feet - 1].ClearTile();

            if (Main.tile[x_in_front, y_above_feet - 2] == null)
                Main.tile[x_in_front, y_above_feet - 2].ClearTile();

            if (Main.tile[x_in_front, y_above_feet - 3] == null)
                Main.tile[x_in_front, y_above_feet - 3].ClearTile();

            if (Main.tile[x_in_front, y_above_feet + 1] == null)
                Main.tile[x_in_front, y_above_feet + 1].ClearTile();
            //  create? 2 other tiles farther in front
            if (Main.tile[x_in_front + NPC.direction, y_above_feet - 1] == null)
                Main.tile[x_in_front + NPC.direction, y_above_feet - 1].ClearTile();

            if (Main.tile[x_in_front + NPC.direction, y_above_feet + 1] == null)
                Main.tile[x_in_front + NPC.direction, y_above_feet + 1].ClearTile();

            if (Main.tile[x_in_front, y_above_feet - 1].HasTile && Main.tile[x_in_front, y_above_feet - 1].TileType == 10 && can_pass_doors)
            { // tile in front is active, is door and NPC can pass doors: trying to break door
                NPC.ai[2] += 1f; // inc knock countdown
                NPC.ai[3] = 0f; // not bored if working on breaking a door
                if (NPC.ai[2] >= 60f)  //  knock once per second
                {
                    if (!Main.bloodMoon && can_pass_doors_bloodmoon_only)
                        NPC.ai[1] = 0f;  //  damage counter zeroed unless bloodmoon, but will still knock

                    NPC.velocity.X = 0.5f * (float)(-(float)NPC.direction); //  slight recoil from hitting it
                    NPC.ai[1] += door_break_pow;  //  increase door damage counter
                    NPC.ai[2] = 0f;  //  knock finished; start next knock
                    bool door_breaking = false;  //  door break flag
                    if (NPC.ai[1] >= 10f)  //  at 10 damage, set door as breaking (and cap at 10)
                    {
                        door_breaking = true;
                        NPC.ai[1] = 10f;
                    }
                    WorldGen.KillTile(x_in_front, y_above_feet - 1, true, false, false);  //  kill door ? when door not breaking too? can fail=true; effect only would make more sense, to make knocking sound
                    if (door_breaking && Main.netMode != 1)  //  server and door breaking
                    {
                        if (breaks_doors)  //  breaks doors rather than attempt to open
                        {
                            WorldGen.KillTile(x_in_front, y_above_feet - 1, false, false, false);  //  kill door
                            if (Main.netMode == 2) // server
                                NetMessage.SendData(17, -1, -1, null, 0, (float)x_in_front, (float)(y_above_feet - 1), 0f, 0); // ?? tile breaking and/or item drop probably
                        }
                        else  //  try to open without breaking
                        {
                            bool door_opened = WorldGen.OpenDoor(x_in_front, y_above_feet, NPC.direction);  //  open the door
                            if (!door_opened)  //  door not opened successfully
                            {
                                NPC.ai[3] = (float)boredom_time;  //  bored if door is stuck
                                NPC.netUpdate = true;
                                NPC.velocity.X = 0; // cancel recoil so boredom wall reflection can trigger
                            }
                            if (Main.netMode == 2 && door_opened) // is server & door was just opened
                                NetMessage.SendData(19, -1, -1, null, 0, (float)x_in_front, (float)y_above_feet, (float)NPC.direction, 0); // ??
                        }
                    }  //  END server and door breaking
                } // END knock on door
            } // END trying to break door
            #endregion
            //-------------------------------------------------------------------
            #region jumping, reset door knock & damage counters
            else // standing on solid tile but not in front of a passable door
            {
                if ((NPC.velocity.X < 0f && NPC.spriteDirection == -1) || (NPC.velocity.X > 0f && NPC.spriteDirection == 1))
                {  //  moving forward
                    if (Main.tile[x_in_front, y_above_feet - 2].HasTile && Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet - 2].TileType])
                    { // 3 blocks above ground level(head height) blocked
                        if (Main.tile[x_in_front, y_above_feet - 3].HasTile && Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet - 3].TileType])
                        { // 4 blocks above ground level(over head) blocked
                            NPC.velocity.Y = -8f; // jump with power 8 (for 4 block steps)
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.velocity.Y = -7f; // jump with power 7 (for 3 block steps)
                            NPC.netUpdate = true;
                        }
                    } // for everything else, head height clear:
                    else if (Main.tile[x_in_front, y_above_feet - 1].HasTile && Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet - 1].TileType])
                    { // 2 blocks above ground level(mid body height) blocked
                        NPC.velocity.Y = -6f; // jump with power 6 (for 2 block steps)
                        NPC.netUpdate = true;
                    }
                    else if (Main.tile[x_in_front, y_above_feet].HasTile && Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet].TileType])
                    { // 1 block above ground level(foot height) blocked
                        NPC.velocity.Y = -5f; // jump with power 5 (for 1 block steps)
                        NPC.netUpdate = true;
                    }
                    else if (NPC.directionY < 0 && jump_gaps && (!Main.tile[x_in_front, y_above_feet + 1].HasTile || !Main.tileSolid[(int)Main.tile[x_in_front, y_above_feet + 1].TileType]) && (!Main.tile[x_in_front + NPC.direction, y_above_feet + 1].HasTile || !Main.tileSolid[(int)Main.tile[x_in_front + NPC.direction, y_above_feet + 1].TileType]))
                    { // rising? & jumps gaps & no solid tile ahead to step on for 2 spaces in front
                        NPC.velocity.Y = -8f; // jump with power 8
                        NPC.velocity.X = NPC.velocity.X * 1.5f; // jump forward hard as well; we're trying to jump a gap
                        NPC.netUpdate = true;
                    }
                    else if (can_pass_doors) // standing on solid tile but not in front of a passable door, moving forward, didnt jump.  I assume recoil from hitting door is too small to move passable door out of range and trigger this
                    {
                        NPC.ai[1] = 0f;  //  reset door dmg counter
                        NPC.ai[2] = 0f;  //  reset knock counter
                    }
                } // END moving forward, still: standing on solid tile but not in front of a passable door
                if (hops && NPC.velocity.Y == 0f && Math.Abs(NPC.position.X + (float)(NPC.width / 2) - (Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))) < hop_range_x && Math.Abs(NPC.position.Y + (float)(NPC.height / 2) - (Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2))) < hop_range_y && ((NPC.direction > 0 && NPC.velocity.X >= hop_velocity) || (NPC.direction < 0 && NPC.velocity.X <= -hop_velocity)))
                { // type that hops & no jump/fall & near target & moving forward fast enough: hop code
                    NPC.velocity.X = NPC.velocity.X * 2f; // burst forward
                    if (NPC.velocity.X > hop_speed) // but cap at hop_speed
                        NPC.velocity.X = hop_speed;
                    else if (NPC.velocity.X < -hop_speed)
                        NPC.velocity.X = -hop_speed;

                    NPC.velocity.Y = -hop_power; // and jump of course
                    NPC.netUpdate = true;
                }
                if (can_teleport && NPC.velocity.Y < 0f) // jumping
                    NPC.velocity.Y = NPC.velocity.Y * 1.1f; // infinite jump? antigravity?
            }
        }
        else if (can_pass_doors)  //  not standing on a solid tile & can open/break doors
        {
            NPC.ai[1] = 0f;  //  reset door damage counter
            NPC.ai[2] = 0f;  //  reset knock counter
        }//*/
        #endregion
        //-------------------------------------------------------------------
        #region teleportation
        if (Main.netMode != 1 && can_teleport && NPC.ai[3] >= (float)boredom_time) // is server & chaos ele & bored
        {
            int target_x_blockpos = (int)Main.player[NPC.target].position.X / 16; // corner not center
            int target_y_blockpos = (int)Main.player[NPC.target].position.Y / 16; // corner not center
            int x_blockpos = (int)NPC.position.X / 16; // corner not center
            int y_blockpos = (int)NPC.position.Y / 16; // corner not center
            int tp_radius = 25; // radius around target(upper left corner) in blocks to teleport into
            int tp_counter = 0;
            bool flag7 = false;
            if (Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) + Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 2000f)
            { // far away from target; 2000 pixels = 125 blocks
                tp_counter = 100;
                flag7 = true; // no teleport
            }
            while (!flag7) // loop always ran full 100 time before I added "flag7 = true" below
            {
                if (tp_counter >= 100) // run 100 times
                    break; //return;
                tp_counter++;

                int tp_x_target = Main.rand.Next(target_x_blockpos - tp_radius, target_x_blockpos + tp_radius);  //  pick random tp point (centered on corner)
                int tp_y_target = Main.rand.Next(target_y_blockpos - tp_radius, target_y_blockpos + tp_radius);  //  pick random tp point (centered on corner)
                for (int m = tp_y_target; m < target_y_blockpos + tp_radius; m++) // traverse y downward to edge of radius
                { // (tp_x_target,m) is block under its feet I think
                    if ((m < target_y_blockpos - 9 || m > target_y_blockpos + 9 || tp_x_target < target_x_blockpos - 9 || tp_x_target > target_x_blockpos + 6) && (m < y_blockpos - 1 || m > y_blockpos + 1 || tp_x_target < x_blockpos - 1 || tp_x_target > x_blockpos + 1) && Main.tile[tp_x_target, m].HasTile)
                    { // over 6 blocks distant from player & over 1 block distant from old position & tile active(to avoid surface? want to tp onto a block?)
                        bool safe_to_stand = true;
                        bool dark_caster = false; // not a fighter type AI...
                        if (dark_caster && Main.tile[tp_x_target, m - 1].WallType == 0) // Dark Caster & ?outdoors
                            safe_to_stand = false;
                        else if (Main.tile[tp_x_target, m - 1].LiquidType == LiquidID.Lava) // feet submerged in lava
                            safe_to_stand = false;

                        if (safe_to_stand && Main.tileSolid[(int)Main.tile[tp_x_target, m].TileType] && !Collision.SolidTiles(tp_x_target - 1, tp_x_target + 1, m - 4, m - 1))
                        { // safe enviornment & solid below feet & 3x4 tile region is clear; (tp_x_target,m) is below bottom middle tile
                            NPC.position.X = (float)(tp_x_target * 16 - NPC.width / 2); // center x at target
                            NPC.position.Y = (float)(m * 16 - NPC.height); // y so block is under feet
                            NPC.netUpdate = true;
                            NPC.ai[3] = -120f; // -120 boredom is signal to display effects & reset boredom next tick in section "teleportation particle effects"
                            flag7 = true; // end the loop (after testing every lower point :/)
                        }
                    } // END over 6 blocks distant from player...
                } // END traverse y down to edge of radius
            } // END try 100 times
        } // END is server & chaos ele & bored
        #endregion
        //-------------------------------------------------------------------

        #region New Boredom by Omnir
        if (quickBored)
        {
            if (!oBored)
            {
                if (NPC.velocity.X == 0f)
                {
                    boredTimer++;
                    if (boredTimer > tBored)
                    {
                        boredResetT = 0;
                        NPC.directionY = -1;
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.direction = 1;
                        }
                        NPC.direction = -1;
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.direction = 1;
                        }
                        oBored = true;
                    }
                }
            }
            if (oBored)
            {
                boredResetT++;
                if (boredResetT > bReset)
                {
                    boredTimer = 0;
                    oBored = false;
                }
            }
        }
        #endregion

        #endregion
    }
    #endregion

    #region Vanilla overrides and misc
    public override bool CheckActive()
    {
        return false;
    }
    public override void BossLoot(ref string name, ref int potionType)
    {
        potionType = ItemID.SuperHealingPotion;
    }
    #endregion       

    //This class exists to pair up the Move, Attack, Draw, and ID of each attack type into one nice and neat state object
    class DarkCloudMove
    {
        public Action Move;
        public Action Attack;
        public int ID;
        public Action<SpriteBatch, Color> Draw;
        public string Name;

        public DarkCloudMove(Action MoveAction, Action AttackAction, int MoveID, string AttackName, Action<SpriteBatch, Color> DrawAction = null)
        {
            Move = MoveAction;
            Attack = AttackAction;
            ID = MoveID;
            Draw = DrawAction;
            Name = AttackName;
        }
    }

    //So I don't have to remember magic numbers
    //Public because Dark Cloud Mirror NPC's also use it
    public class DarkCloudAttackID
    {
        public const short DragoonLance = 0;
        public const short DivineSpark = 1;
        public const short DarkFlow = 2;
        public const short UltimaWeapon = 3;
        public const short FreezeBolts = 4;
        public const short AntiMat = 5;
        public const short ConfinedBlasts = 6;
        public const short ArrowRain = 7;
        public const short TeleportingSlashes = 8;
        public const short BulletPortals = 9;
        public const short Thunderstorm = 10;
        public const short DivineSparkThirds = 11;
    }
}