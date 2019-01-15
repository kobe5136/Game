using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Monster> Monsters = new List<Monster>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();

        public const int ITEM_ID_RUSTY_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_PIECE_OF_FUR = 3;
        public const int ITEM_ID_SNAKE_FANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_FANG = 8;
        public const int ITEM_ID_SPIDER_SILK = 9;
        public const int ITEM_ID_ADVENTURER_PASS = 10;

        public const int MONSTER_ID_RAT = 1;
        public const int MONSTER_ID_SNAKE = 2;
        public const int MONSTER_ID_GIANT_SPIDER = 3;

        public const int QUEST_ID_CLEAR_ALCHEMIST_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;

        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;

        static World()
        {
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();
        }

        private static void PopulateItems()
        {
            Items.Add(new Weapon(ITEM_ID_RUSTY_SWORD,"木劍", "木劍", 1,5));
            Items.Add(new Item(ITEM_ID_RAT_TAIL, "老鼠尾巴", "老鼠尾巴"));
            Items.Add(new Item(ITEM_ID_PIECE_OF_FUR, "鼠皮", "鼠皮"));            Items.Add(new Item(ITEM_ID_SNAKE_FANG, "蛇牙", "蛇牙"));            Items.Add(new Item(ITEM_ID_SNAKESKIN, "蛇皮", "蛇皮"));
            Items.Add(new Weapon(ITEM_ID_CLUB, "小刀", "小刀", 3, 10));
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION,                     "治療藥水", "治療藥水", 5));
            Items.Add(new Item(ITEM_ID_SPIDER_FANG, "蜘蛛牙", "蜘蛛牙"));
            Items.Add(new Item(ITEM_ID_SPIDER_SILK, "蜘蛛絲", "蜘蛛絲"));
            Items.Add(new Item(ITEM_ID_ADVENTURER_PASS,
                "通行證", "通行證"));
        }

        private static void PopulateMonsters()
        {
            Monster rat = new Monster(MONSTER_ID_RAT, "老鼠", 5, 3, 10, 3, 3);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL),75,false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_FUR), 75, true));            Monster snake = new Monster(MONSTER_ID_SNAKE, "蛇", 5, 3, 10, 3, 3);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG), 75, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 75, true));

            Monster giantSpider = new Monster(MONSTER_ID_GIANT_SPIDER,
                "巨大蜘蛛", 20, 5, 40, 10, 10);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));

            Monsters.Add(rat);
            Monsters.Add(snake);
            Monsters.Add(giantSpider);
        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden = new Quest(QUEST_ID_CLEAR_ALCHEMIST_GARDEN,
                "清理城郊花園",
                "殺死在城郊花園的老鼠，並取得三條老鼠尾巴" +
                "你將會得到治療藥草與10元的賞金",20,10);
            clearAlchemistGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RAT_TAIL), 3));
            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);

            Quest clearFarmersField = new Quest(QUEST_ID_CLEAR_FARMERS_FIELD, "清理農夫的農地",
                "殺死農地裡的蛇，並取得三根蛇的牙齒"+
               " 你將會取得通行證並且得到20元的賞金", 20, 20);

            clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKE_FANG), 3));

            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);

            Quests.Add(clearAlchemistGarden);
            Quests.Add(clearFarmersField);
        }

        private static void PopulateLocations()
        {
            Location home =new Location(LOCATION_ID_HOME, "家","你的家，溫暖的家");
            Location townSquare= new Location(LOCATION_ID_TOWN_SQUARE,
                        "城門口", "外面有一座森林");
            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT,
                        "奇怪的商店", "架上有許多奇怪的植物");
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_ALCHEMIST_GARDEN);
            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTS_GARDEN,
                        "城郊花園", "種滿了許多植物");
            alchemistsGarden.MonsterLivingHere = MonsterByID(MONSTER_ID_RAT);
            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE,
                    "農場", "這是一個小農場，農場前有位農夫");
            farmhouse.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELD);
            Location farmersField = new Location(LOCATION_ID_FARM_FIELD,
                    "農地", "這裡種滿了許多蔬菜");
            farmersField.MonsterLivingHere = MonsterByID(MONSTER_ID_SNAKE);
            Location guardPost = new Location(LOCATION_ID_GUARD_POST,
            "警衛哨岡", "有位看起來十分強狀的警衛",ItemByID(ITEM_ID_ADVENTURER_PASS));
            Location bridge = new Location(LOCATION_ID_BRIDGE,
                "石橋", "這座石橋穿越這條河");
            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD,
                    "森林", "森林裡佈滿了許多蜘蛛網");            spiderField.MonsterLivingHere = MonsterByID(MONSTER_ID_GIANT_SPIDER);            home.LocationToNorth = townSquare;            townSquare.LocationToNorth = alchemistHut;            townSquare.LocationToSouth = home;            townSquare.LocationToWest = farmhouse;            townSquare.LocationToEast = guardPost;            farmhouse.LocationToEast = townSquare;            farmhouse.LocationToWest = farmersField;            farmersField.LocationToEast = farmhouse;            alchemistHut.LocationToSouth = townSquare;            alchemistHut.LocationToNorth = alchemistsGarden;            alchemistsGarden.LocationToSouth = alchemistHut;            guardPost.LocationToEast = bridge;            guardPost.LocationToWest = townSquare;            bridge.LocationToWest = guardPost;            bridge.LocationToEast = spiderField;            spiderField.LocationToWest = bridge;            Locations.Add(home);            Locations.Add(townSquare);            Locations.Add(guardPost);            Locations.Add(alchemistHut);            Locations.Add(alchemistsGarden);            Locations.Add(farmhouse);            Locations.Add(farmersField);            Locations.Add(bridge);            Locations.Add(spiderField);            
        }


        public static Item ItemByID(int id)
        {
            foreach(Item item in Items)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }
            return null;
        }
        public static Monster MonsterByID(int id)
        {
            foreach(Monster monster in Monsters)
            {
                if (monster.ID == id)
                {
                    return monster;
                }
            }
            return null;
        }
        public static Quest QuestByID(int id)
        {
            foreach(Quest quest in Quests)
            {
                if (quest.ID == id)
                {
                    return quest;
                }
            }
            return null;
        }
        public static Location LocationByID(int id)
        {
            foreach(Location location in Locations)
            {
                if (location.ID == id)
                {
                    return location;
                }
            }
            return null;
        }

    }
}
