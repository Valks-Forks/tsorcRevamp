using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.BossItems;

class AaronsProtectionStone : ModItem
{

    public override void SetStaticDefaults()
    {
        Tooltip.SetDefault("The volcanic stoned etched in Aaron's image" +
                            "\nSaid to protect the one who carries it in times of despair" +
                            "\n[c/00ffd4:It's true use may reveal itself in time...or at a particular location...]" +
                            "\nNot MP compatible currently");
        //"\n[c/00ffd4:Use this at the top of The Temple Shrine of The Wall] if your first attempt" +
        //"\ndoes not succeed. (Use it, don't drop it in lava.)"); //+
        //"\nBut first: save, quit and reload before each time you resummon him.");
    }
    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 26;
        Item.consumable = false;
        Item.maxStack = 1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 5;
        Item.useTime = 5;
        Item.defense = 3;
        //item.UseSound = SoundID.Item21;
        Item.value = 1000;
        Item.rare = ItemRarityID.Blue;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ZoneUnderworldHeight && !NPC.AnyNPCs(NPCID.WallofFlesh);
    }

    public override bool? UseItem(Player player)
    {
        UsefulFunctions.BroadcastText("A Gate has been opened. The Great Wall has passed into this dimension!... ", 175, 75, 255);
        NPC.NewNPC(player.GetSource_ItemUse(Item), (int)Main.player[Main.myPlayer].position.X - (1070), (int)Main.player[Main.myPlayer].position.Y - 150, NPCID.WallofFlesh, 1);
        return true;
    }
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.GuideVoodooDoll, 1);
        //recipe.AddTile(TileID.DemonAltar);
        recipe.Register();
    }
}
