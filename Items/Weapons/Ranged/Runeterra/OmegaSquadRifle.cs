using tsorcRevamp.Projectiles.Ranged.Runeterra;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using tsorcRevamp.Buffs.Runeterra.Ranged;

namespace tsorcRevamp.Items.Weapons.Ranged.Runeterra;

[Autoload(false)]
public class OmegaSquadRifle : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Omega Squad Rifle");
        Tooltip.SetDefault("Converts seeds into Radioactive Darts and allows you to gather Seeds from grass" +
            "\nRadioactive Darts apply a short burst of Irradiated home into enemies" +
            "\nAlso uses all darts as ammo, but Poison Darts deal double damage" +
            "\nStops players movement for a fraction of the weapon's usetime if recently hurt, slows otherwise" +
            "\nGrants movement speed and stamina regen boost whilst being held that gets removed upon taking damage temporarily" +
            "\nPress Special Ability to gain an even higher temporary boost and remove the movement penalties" +
            "\nRight click to shoot a homing blinding dart which inflicts confusion" +
            "\nPress Shift and Special Ability to drop a Nuclear Mushroom" +
            "\n'There's a mushroom out there with your name on it'");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 62;
        Item.height = 22;
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.buyPrice(1, 0, 0, 0);
        Item.useTime = 22;
        Item.useAnimation = 22;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item99;
        Item.DamageType = DamageClass.Ranged; 
        Item.damage = 220;
        Item.knockBack = 6f;
        Item.noMelee = true;
        Item.shoot = ProjectileID.Seed;
        Item.shootSpeed = 10f;
        Item.useAmmo = AmmoID.Dart;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(0f, -8f);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (type == ProjectileID.Seed & player.altFunctionUse == 1)
        {
            type = ModContent.ProjectileType<RadioactiveDart>();
        }
        if (type == ProjectileID.PoisonDartBlowgun)
        {
            damage *= 4;
            damage /= 3;
        }
        if (player.altFunctionUse == 2)
        {
            if (type == ProjectileID.Seed)
            {
                damage /= 2;
            }
            type = ModContent.ProjectileType<RadioactiveBlindingLaser>();
        }
    }
    public override void HoldItem(Player player)
    {
        if (!player.HasBuff(ModContent.BuffType<ScoutsBoostCooldown>()))
        {
            if (!player.HasBuff(ModContent.BuffType<ScoutsBoost2>()))
            {
                player.AddBuff(ModContent.BuffType<ScoutsBoost>(), 1);
            }
        }
        if (player.HasBuff(ModContent.BuffType<ScoutsBoost2>()))
        {
            //nothing
        }
        else
        if (player.itemAnimation > 14 && (!player.HasBuff(ModContent.BuffType<ScoutsBoostCooldown>())))
        {
            player.velocity *= 0.92f;
        }
        else if (player.itemAnimation > 14 && player.HasBuff(ModContent.BuffType<ScoutsBoostCooldown>()))
        {
            player.velocity *= 0.01f;
        }
    }
    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (Main.mouseRight & !Main.mouseLeft & !player.HasBuff(ModContent.BuffType<RadioactiveBlindingLaserCooldown>()))  //cooldown gets applied on projectile spawn
        {
            player.altFunctionUse = 2;
        }
        if (Main.mouseLeft)
        {
            player.altFunctionUse = 1;
        }
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override bool CanUseItem(Player player)
    {
        if (player.altFunctionUse != 2 || !player.HasBuff(ModContent.BuffType<RadioactiveBlindingLaserCooldown>()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();

        recipe.AddIngredient(ModContent.ItemType<AlienRifle>());
        recipe.AddIngredient(ItemID.LunarBar, 12);
        recipe.AddIngredient(ModContent.ItemType<DarkSoul>(), 70000);
        recipe.AddTile(TileID.DemonAltar);

        recipe.Register();
    }
}