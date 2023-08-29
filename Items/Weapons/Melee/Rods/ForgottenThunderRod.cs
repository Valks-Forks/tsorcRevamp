﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Projectiles;

namespace tsorcRevamp.Items.Weapons.Melee.Rods;

class ForgottenThunderRod : ModItem
{

    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("Randomly casts Bolt 2.");
    }

    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.Pink;
        Item.damage = 20;
        Item.width = 26;
        Item.height = 26;
        Item.knockBack = 4;
        Item.DamageType = DamageClass.Melee;
        Item.autoReuse = true;
        Item.useAnimation = 27;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 21;
        Item.value = PriceByRarity.Pink_5;
        Item.shoot = ModContent.ProjectileType<Nothing>();
    }

    public override bool? UseItem(Player player)
    {
        if (Main.rand.NextBool(5))
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.position.X, player.position.Y, (float)(-40 + Main.rand.Next(80)) / 10, 14.9f, ModContent.ProjectileType<Projectiles.Bolt2Ball>(), 20, 2.0f, player.whoAmI);
        }
        return true;
    }
}
