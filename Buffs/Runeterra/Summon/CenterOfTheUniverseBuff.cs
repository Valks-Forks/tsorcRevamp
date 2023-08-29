using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using tsorcRevamp.Items.Weapons.Summon.Runeterra;
using tsorcRevamp.Projectiles.Summon.Runeterra;

namespace tsorcRevamp.Buffs.Runeterra.Summon;

public class CenterOfTheUniverseBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Center of the Universe");
        Description.SetDefault("You're the center of the Universe!");

        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        // If the minions exist reset the buff time, otherwise remove the buff from the player
        if (player.ownedProjectileCounts[ModContent.ProjectileType<CenterOfTheUniverseStar>()] > 0)
        {
            // update projectiles
            CenterOfTheUniverse.ReposeProjectiles(player);
            player.buffTime[buffIndex] = 18000;
        }
        else
        {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
        if (player.GetModPlayer<tsorcRevampPlayer>().CritCounter == 3)
        {
            Projectile.NewProjectile(Projectile.GetSource_None(), player.Center, Vector2.One, ModContent.ProjectileType<CenterOfTheUniverseStellarNova>(), 100, 1f, Main.myPlayer);
            player.GetModPlayer<tsorcRevampPlayer>().CritCounter = 0;
        }
    }
}