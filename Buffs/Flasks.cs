﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Buffs;

class Flasks : GlobalBuff
{

    public override void Update(int type, Player player, ref int buffIndex)
    {
        if (type == BuffID.WeaponImbueVenom
            || type == BuffID.WeaponImbueCursedFlames
            || type == BuffID.WeaponImbueFire
            || type == BuffID.WeaponImbueGold
            || type == BuffID.WeaponImbueIchor
            || type == BuffID.WeaponImbueNanites
            || type == BuffID.WeaponImbueConfetti
            || type == BuffID.WeaponImbuePoison
            )
        {
            player.GetDamage(DamageClass.Melee) += 0.1f;
            player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.1f;
        }
    }

    public override void ModifyBuffTip(int type, ref string tip, ref int rare)
    {
        if (type == BuffID.WeaponImbueVenom)
        {
            tip = "Gives 10% melee and whip damage, melee and whip attacks inflict Venom on enemies";
        }
        if (type == BuffID.WeaponImbueCursedFlames)
        {
            tip = "Gives 10% melee and whip damage, melee and whip attacks inflict enemies with cursed flames";
        }
        if (type == BuffID.WeaponImbueFire)
        {
            tip = "Gives 10% melee and whip damage, melee and whip attacks set enemies on fire";
        }
        if (type == BuffID.WeaponImbueGold)
        {
            tip = "Gives 10% melee and whip damage, melee and whip attacks make enemies drop more gold";
        }
        if (type == BuffID.WeaponImbueIchor)
        {
            tip = "Gives 10% melee and whip damage, melee and whip attacks decrease enemies' defense";
        }
        if (type == BuffID.WeaponImbueNanites)
        {
            tip = "Gives 10% melee and whip damage, melee and whip attacks confuse enemies";
        }
        if (type == BuffID.WeaponImbueConfetti)
        {
            tip = "Gives 10% melee and whip damage, melee and whip attacks cause confetti to appear";
        }
        if (type == BuffID.WeaponImbuePoison)
        {
            tip = "Gives 10% melee and whip damage, melee and whip attacks poison enemies";
        }
    }

}
