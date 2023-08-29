﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Weapons.Melee.Broadswords;

class LionheartGunblade : ModItem
{

    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("\"Of gunblades, the finest model is the Lionheart.\"");
    }
    public override void SetDefaults()
    {
        Item.damage = 80;
        Item.width = 73;
        Item.height = 29;
        Item.knockBack = 7;
        Item.expert = true;
        Item.scale = 1.1f;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 10;
        Item.useAmmo = AmmoID.Bullet;
        Item.DamageType = DamageClass.Ranged;
        Item.DamageType = DamageClass.Melee;
        Item.useAnimation = 15;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 45;
        Item.value = PriceByRarity.LightRed_4;
    }
}
