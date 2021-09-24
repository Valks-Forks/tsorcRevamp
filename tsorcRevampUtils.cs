using Terraria;

namespace tsorcRevamp {

    //using static tsorcRevamp.SpawnHelper;
    public static class SpawnHelper {

        //undergroundJungle, undergroundEvil, and undergroundHoly are deliberately missing. call Cavern && p.zone instead.

        public static bool Cavern(Player p) { //if youre calling Cavern without a p.zone check, also call NoSpecialBiome
            return p.position.Y >= Main.rockLayer && p.position.Y <= Main.rockLayer * 25;
        }

        public static bool NoSpecialBiome(Player p) {
            return (!p.ZoneJungle && !p.ZoneCorrupt && !p.ZoneCrimson && !p.ZoneHoly && !p.ZoneMeteor && !p.ZoneDungeon);
        }

        public static bool Sky(Player p) { //p.ZoneSkyHeight is more restrictive than this, so use this if an enemy uses it
            return p.position.Y < Main.worldSurface * 0.44999998807907104;
        }

        public static bool Surface(Player p) {
            return !Sky(p) && (p.position.Y < Main.worldSurface); //dont need to check nospecialbiome here since we're already calling Sky
        }

        public static bool Underground(Player p) {
            return Main.worldSurface > p.position.Y && p.position.Y < Main.rockLayer;
        } 

        public static bool Underworld(Player p) {
            int playerYTile = (int)(p.Bottom.Y + 8f) / 16;
            return playerYTile > Main.maxTilesY - 190;
        }
    }

    //using static tsorcRevamp.oSpawnHelper;
    public static class oSpawnHelper { 

        public static bool oCavern(Player p) {
            return (p.position.Y >= (Main.rockLayer * 17)) && (p.position.Y < (Main.rockLayer * 24));
        }

        public static bool oCavernByTile(Player p) {
            int playerYTile = (int)(p.Bottom.Y + 8f) / 16;
            return playerYTile >= (Main.maxTilesY * 0.4f) && playerYTile < (Main.maxTilesY * 0.6f);
        }

        public static bool oMagmaCavern(Player p) {
            return (p.position.Y >= (Main.rockLayer * 24)) && (p.position.Y < (Main.rockLayer * 32));
        }

        public static bool oMagmaCavernByTile(Player p) {
            int playerYTile = (int)(p.Bottom.Y + 8f) / 16;
            return playerYTile >= (Main.maxTilesY * 0.6f) && playerYTile < (Main.maxTilesY * 0.8f);
        }

        public static bool oSky(Player p) {
            return p.position.Y <= Main.rockLayer * 4;
        }

        public static bool oSkyByTile(Player p) {
            int playerYTile = (int)(p.Bottom.Y + 8f) / 16;
            return playerYTile < (Main.maxTilesY * 0.1f);
        }

        public static bool oSurface(Player p) {
            return !p.ZoneSkyHeight && (p.position.Y <= Main.worldSurface);
        }

        public static bool oUnderground(Player p) {
            return (p.position.Y >= (Main.rockLayer * 13)) && (p.position.Y < (Main.rockLayer * 17));
        }

        public static bool oUndergroundByTile(Player p) {
            int playerYTile = (int)(p.Bottom.Y + 8f) / 16;
            return playerYTile >= (Main.maxTilesY * 0.3f) && playerYTile < (Main.maxTilesY * 0.4f);
        }

        public static bool oUnderSurface(Player p) {
            return (p.position.Y > (Main.rockLayer * 8)) && (p.position.Y < (Main.rockLayer * 13));
        }

        public static bool oUnderSurfaceByTile(Player p) {
            int playerYTile = (int)(p.Bottom.Y + 8f) / 16;
            return (playerYTile >= (Main.maxTilesY * 0.2f) && playerYTile < (Main.maxTilesY * 0.3f));
        }

        public static bool oUnderworld(Player p) {
            return p.position.Y >= (Main.rockLayer * 32);
        }

        public static bool oUnderworldByHeight(Player p) {
            int playerYTile = (int)(p.Bottom.Y + 8f) / 16;
            return (playerYTile >= (Main.maxTilesY * 0.8f));
        }
    }

    public static class VariousConstants {
        public const int CUSTOM_MAP_WORLD_ID = 44874972;
    }

    public static class PriceByRarity { 
        
        //minimal exploration. pre-hardmode ores. likely no items that craft from souls will use this
        public static readonly int White_0 = Item.buyPrice(platinum: 0, gold: 0, silver: 40, copper: 0);

        //underground chest loots (shoe spikes, CiaB, etc), shadow orb items, floating island
        public static readonly int Blue_1 = Item.buyPrice(platinum: 0, gold: 2, silver: 50, copper: 0);

        //gold dungeon chest (handgun, cobalt shield, etc), goblin invasion
        public static readonly int Green_2 = Item.buyPrice(platinum: 0, gold: 5, silver: 0, copper: 0);

        //hell, underground jungle
        public static readonly int Orange_3 = Item.buyPrice(platinum: 0, gold: 10, silver: 0, copper: 0);

        //early hardmode (hm ores), mimics
        public static readonly int LightRed_4 = Item.buyPrice(platinum: 0, gold: 14, silver: 50, copper: 0);

        //hallowed tier. post mech, pre plantera. pirates.
        public static readonly int Pink_5 = Item.buyPrice(platinum: 0, gold: 25, silver: 50, copper: 0);

        //some biome mimic gear, high level tinkerer combinations (ankh charm, mechanical glove). seldom used in vanilla
        public static readonly int LightPurple_6 = Item.buyPrice(platinum: 0, gold: 37, silver: 50, copper: 0);

        //plantera, golem, chlorophyte
        public static readonly int Lime_7 = Item.buyPrice(platinum: 0, gold: 47, silver: 50, copper: 0);
        
        //post-plantera dungeon, martian madness, pumpkin/frost moon
        public static readonly int Yellow_8 = Item.buyPrice(platinum: 0, gold: 55, silver: 0, copper: 0);
        
        //lunar fragments, dev armor. seldom used in vanilla
        public static readonly int Cyan_9 = Item.buyPrice(platinum: 0, gold: 67, silver: 50, copper: 0);
        
        //luminite, lunar fragment gear, moon lord drops
        public static readonly int Red_10 = Item.buyPrice(platinum: 0, gold: 80, silver: 0, copper: 0);
        
        //no vanilla items have purple rarity base. only cyan and red with modifiers can be purple. im guessing on the price.
        public static readonly int Purple_11 = Item.buyPrice(platinum: 1, gold: 20, silver: 0, copper: 0);



    }

}