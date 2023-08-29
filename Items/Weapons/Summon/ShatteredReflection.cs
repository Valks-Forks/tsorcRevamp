using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Weapons.Summon;

	public class ShatteredReflection : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("What will you see next time you look at it?" +
				"\nSplits off a fragment of your soul to fight for you" +
            "\nUses 2 minion slots");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 2;
    }
		public override void SetDefaults()
		{
			Item.damage = 250;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 34;
			Item.height = 38;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.value = Item.buyPrice(1, 50, 0, 0);
			Item.rare = ItemRarityID.Expert;
			Item.expert = true;
			Item.UseSound = SoundID.Shatter;


			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<Buffs.Summon.ShatteredReflectionBuff>();
			Item.shoot = ModContent.ProjectileType<Projectiles.Summon.ShatteredReflectionProjectile>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
			position = player.Center;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			// Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
			return false;
		}
}