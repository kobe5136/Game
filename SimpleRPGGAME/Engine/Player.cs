using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;

namespace Engine
{
    public class Player:LivingCreature
    {
        private int _gold;
        private int _experiencePoints;
        private Monster _currentMonster;
        public event EventHandler<MessageEventArgs> OnMessage;
        public int Gold { get { return _gold; } set { _gold = value;
                OnPropertyChanged("Gold");
            } }
        public int ExperiencePoints { get { return _experiencePoints; } private set {
                _experiencePoints = value;
                OnPropertyChanged("ExperiencePoints");
                OnPropertyChanged("Level");
            } }
        public int Level
        {
            get { return ((ExperiencePoints / 100) + 1); }
        }
        public BindingList<InventoryItem> Inventory { get; set; }
        public BindingList<PlayerQuest> Quests { get; set; }
        public Location _currentLocation;
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }
        public Weapon CurrentWeapon { get; set; }
        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
        }
        public List<HealingPotion> Potions
        {
            get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).ToList(); }
        }
        private void RaiseInventoryChangedEvent(Item item)
        {
            if(item is Weapon)
            {
                OnPropertyChanged("Weapons");
            }
            if(item is HealingPotion)
            {
                OnPropertyChanged("Potions");
            }
        }

        private Player(int currentHitPoints, int maxHitPoints,
             int gold, int experiencePoints) :
            base(currentHitPoints, maxHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
        }

        public void RemoveItemFromInventory(Item itemToRemove,int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);
            if (item == null)
            {

            }
            else
            {
                item.Quantity -= quantity;
                if (item.Quantity < 0)
                {
                    item.Quantity = 0;
                }
                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }
                RaiseInventoryChangedEvent(itemToRemove);
            }
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach(QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);
                if (item != null)
                {
                    RemoveItemFromInventory(item.Details, qci.Quantity);
                }
            }
        }

        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null)
            {
                return true;
            }
            return Inventory.Any(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);

        }

        public bool HasThisQuest(Quest quest)
        {

            return Quests.Any(playerQuest => playerQuest.Details.ID == quest.ID);
        }

        public bool CompletedThisQuest(Quest quest)
        {
            foreach (PlayerQuest playerQuest in Quests)
            {
                if (playerQuest.Details.ID == quest.ID)
                {
                    return playerQuest.IsCompleted;
                }
            }
            return false;
        }

        public bool HasAllQuestCompletedItem(Quest quest)
        {
            foreach (QuestCompletionItem qui in quest.QuestCompletionItems)
            {


                if (!Inventory.Any(ii => ii.Details.ID ==
                        qui.Details.ID && ii.Quantity >= qui.Quantity))
                {
                    return false;
                }
            }
            return true;
        }

        
        public void AddItemToInventory(Item itemToAdd,int quantity=1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);
            if (item == null)
            {
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                item.Quantity += quantity;
            }
            RaiseInventoryChangedEvent(itemToAdd);
        }
        public void MarkQuestCompleted(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(
                 pq => pq.Details.ID == quest.ID);
            if (playerQuest != null)
            {
                playerQuest.IsCompleted = true;
            }
        }
        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();
            XmlNode player = playerData.CreateElement("player");
            playerData.AppendChild(player);

            XmlNode stats = playerData.CreateElement("stats");
            player.AppendChild(stats);

            XmlNode currentHitPoints = playerData.CreateElement("CurrentHitPoints");
            currentHitPoints.AppendChild(playerData.CreateTextNode(this.CurrentHitPoints.ToString()));
            stats.AppendChild(currentHitPoints);

            XmlNode maxHitPoints = playerData.CreateElement("MaxHitPoints");
            maxHitPoints.AppendChild(playerData.CreateTextNode(this.MaxHitPoints.ToString()));
            stats.AppendChild(maxHitPoints);

            XmlNode gold = playerData.CreateElement("Gold");
            gold.AppendChild(playerData.CreateTextNode(this.Gold.ToString()));
            stats.AppendChild(gold);

            XmlNode experiencePoints = playerData.CreateElement("ExperiencePoints");
            experiencePoints.AppendChild(playerData.CreateTextNode(this.ExperiencePoints.ToString()));
            stats.AppendChild(experiencePoints);

            XmlNode currentLocation = playerData.CreateElement("CurrentLocation");
            currentLocation.AppendChild(playerData.CreateTextNode(this.CurrentLocation.ID.ToString()));
            stats.AppendChild(currentLocation);

            if (CurrentWeapon != null)
            {
                XmlNode currentWeapon = playerData.CreateElement("CurrentWeapon");
                currentWeapon.AppendChild(playerData.CreateTextNode
                    (this.CurrentWeapon.ID.ToString()));
                stats.AppendChild(currentWeapon);
            }

            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);

            foreach(InventoryItem item in this.Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("InventoryItem");
                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = item.Details.ID.ToString();
                inventoryItem.Attributes.Append(idAttribute);

                XmlAttribute quantityAttribute = playerData.CreateAttribute("Quantity");
                quantityAttribute.Value = item.Quantity.ToString();
                inventoryItem.Attributes.Append(quantityAttribute);

                inventoryItem.AppendChild(inventoryItem);
            }

            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);

            foreach(PlayerQuest quest in this.Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = quest.Details.ID.ToString();
                playerQuest.Attributes.Append(idAttribute);

                XmlAttribute isCompletedAttribute = playerData.CreateAttribute("IsCompleted");
                isCompletedAttribute.Value = quest.IsCompleted.ToString();
                playerQuest.Attributes.Append(isCompletedAttribute);

                playerQuests.AppendChild(playerQuest);
            }
            return playerData.InnerXml;
        }
        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(10, 10, 20, 0);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);

            return player;
        }

        public void AddEXPPoints(int expPointsToADD)
        {
            ExperiencePoints += expPointsToADD;
            MaxHitPoints = (Level * 10);
        }

        public static Player CreatPlayerFromXmlString(string xmlPlayerData)
        {
            try
            {
                XmlDocument playerdata = new XmlDocument();
                playerdata.LoadXml(xmlPlayerData);

                int currentHitPoints = Convert.ToInt32(playerdata.SelectSingleNode
                    ("/Player/Stats/CurrentHitPoints").InnerText);
                int maxHitPoints = Convert.ToInt32(playerdata.SelectSingleNode
                    ("/Player/Stats/MaxHitPoints").InnerText);
                int gold = Convert.ToInt32(playerdata.SelectSingleNode
                    ("/Player/Stats/Gold").InnerText);
                int experiencePoints = Convert.ToInt32(playerdata.SelectSingleNode
                    ("/Player/Stats/ExperiencePoints").InnerText);
                Player player = new Player(currentHitPoints, maxHitPoints, gold, experiencePoints);
                int currentLocationID = Convert.ToInt32(playerdata.SelectSingleNode
                    ("/Player/Stats/CurrentLocation").InnerText);
                player.CurrentLocation = World.LocationByID(currentLocationID);
                if (playerdata.SelectSingleNode("/Player/Stats/CurrentWeapon") != null)
                {
                    int currentWeaponID = Convert.ToInt32(playerdata.SelectSingleNode
                        ("/Player/Stats/CurrentWeapon").InnerText);
                    player.CurrentWeapon = (Weapon)World.ItemByID(currentWeaponID);
                }
                foreach(XmlNode node in playerdata.SelectSingleNode("/Player/InventoryItems/InventoryItem"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);

                    for(int i = 0; i < quantity; i++)
                    {
                        player.AddItemToInventory(World.ItemByID(id));
                    }
                }
                foreach(XmlNode node in playerdata.SelectSingleNode("/Player/PlayerQuests/PlayerQuest"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);
                    PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(id));
                    playerQuest.IsCompleted = isCompleted;
                    player.Quests.Add(playerQuest);
                }
                return player;
            }
            catch
            {
                return Player.CreateDefaultPlayer();
            }
        }

        private void RaiseMessage(string message,bool addExtraNewLine=false)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new MessageEventArgs(message, addExtraNewLine));
            }
        }
        public void MoveNorth()
        {
            if (CurrentLocation.LocationToNorth != null)
            {
                MoveTo(CurrentLocation.LocationToNorth);
            }
        }
        public void MoveEast()
        {
            if (CurrentLocation.LocationToEast != null)
            {
                MoveTo(CurrentLocation.LocationToEast);
            }
        }
        public void MoveWest()
        {
            if (CurrentLocation.LocationToWest != null)
            {
                MoveTo(CurrentLocation.LocationToWest);
            }
        }
        public void MoveSouth()
        {
            if (CurrentLocation.LocationToSouth != null)
            {
                MoveTo(CurrentLocation.LocationToSouth);
            }
        }


        public void MoveTo(Location newLocation)
        {
            if (!HasRequiredItemToEnterThisLocation(newLocation))
            {
                RaiseMessage("你必須" + newLocation.ItemRequiredToEnter.Name + "才能進入此區域");
                return;
            }



            CurrentLocation = newLocation;
            CurrentHitPoints = MaxHitPoints;

            if (newLocation.QuestAvailableHere != null)
            {
                bool playerAlreadyHasQuest = HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompleteQuest = CompletedThisQuest(newLocation.QuestAvailableHere);

                if (playerAlreadyHasQuest)
                {
                    if (!playerAlreadyCompleteQuest)
                    {
                        bool playerHasAllItemsToCompleteQuest =
                            HasAllQuestCompletedItem(newLocation.QuestAvailableHere);

                        if (playerHasAllItemsToCompleteQuest)
                        {
                            RaiseMessage("");
                            RaiseMessage("你完成了" + newLocation.QuestAvailableHere.Name + "任務");
                            RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

                            RaiseMessage("妳得到了: " + newLocation.QuestAvailableHere.RewardEXP.ToString() + "經驗值");
                            RaiseMessage("以及" + newLocation.QuestAvailableHere.RewardGold + "黃金");
                            RaiseMessage(newLocation.QuestAvailableHere.RewardItem.Name, true);

                            AddEXPPoints(newLocation.QuestAvailableHere.RewardEXP);
                            Gold += newLocation.QuestAvailableHere.RewardGold;

                            AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

                            MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                else
                {
                    
                    RaiseMessage("你得到了" + newLocation.QuestAvailableHere.Name + "任務");
                    RaiseMessage(newLocation.QuestAvailableHere.Description);
                    RaiseMessage("為了完成任務，需要物品:");

                    foreach (QuestCompletionItem qui in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qui.Quantity == 1)
                        {
                            RaiseMessage(qui.Quantity + " " + qui.Details.Name);
                        }

                        else
                        {
                            RaiseMessage(qui.Quantity + " " + qui.Details.NamePlural);
                        }
                    }

                    RaiseMessage(" ");

                    Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            if (newLocation.MonsterLivingHere != null)
            {
                RaiseMessage("你看到了" + newLocation.MonsterLivingHere.Name);
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, "standardMonster.Name", standardMonster.MaxDamage,
                    standardMonster.RewardEXP, standardMonster.RewardGold, standardMonster.CurrentHitPoints,
                    standardMonster.MaxHitPoints);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }
                
            }
            else
            {
                _currentMonster = null;
            }


        }

        public void UseWeapon(Weapon weapon)
        {

            int damageToMonster = RandomNumberGenerator.NumberBetween(weapon.MinDamage, weapon.MaxDamage);
            _currentMonster.CurrentHitPoints -= damageToMonster;
            RaiseMessage("你給於" + _currentMonster.Name +
                        + damageToMonster + "點傷害");
            if (_currentMonster.CurrentHitPoints <= 0)
            {
                RaiseMessage("");
                RaiseMessage("你擊敗了" + _currentMonster.Name);
                AddEXPPoints(_currentMonster.RewardEXP);
                RaiseMessage("你得到了" + _currentMonster.RewardEXP +"經驗值");
                Gold += _currentMonster.RewardGold;
                RaiseMessage("你得到了" + _currentMonster.RewardGold + "金幣");
                List<InventoryItem> lootedItems = new List<InventoryItem>();
                foreach (LootItem lootItem in _currentMonster.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Detail, 1));
                    }
                }
                if (lootedItems.Count == 0)
                {
                    foreach (LootItem lootItem in _currentMonster.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Detail, 1));
                        }
                    }
                }
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    AddItemToInventory(inventoryItem.Details);
                    if (inventoryItem.Quantity == 1)
                    {
                        RaiseMessage("你得到"+inventoryItem.Quantity + " " + inventoryItem.Details.Name);
                    }
                    else
                    {
                        RaiseMessage("你得到" + inventoryItem.Quantity +" " + inventoryItem.Details.NamePlural);
                    }
                }
                RaiseMessage(" ");
                MoveTo(CurrentLocation);

            }
            else
            {
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaxDamage);
                RaiseMessage(_currentMonster.Name + "對你造成了" +damageToPlayer + "點傷害");
                CurrentHitPoints -= damageToPlayer;

                if (CurrentHitPoints <= 0)
                {
                    RaiseMessage(_currentMonster.Name + "擊殺你");

                    MoveHome();
                }
            }
        }
        public void UsePotion(HealingPotion potion)
        {
            CurrentHitPoints = (CurrentHitPoints + potion.AmountToHeal);
            if (CurrentHitPoints > MaxHitPoints)
            {
                CurrentHitPoints = MaxHitPoints;
            }
            RemoveItemFromInventory(potion, 1);
            RaiseMessage("你使用了" + potion.Name);
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaxDamage);
            RaiseMessage(_currentMonster.Name +"對你造成了" + damageToPlayer + "點傷害");
            CurrentHitPoints -= damageToPlayer;
            
            if (CurrentHitPoints <= 0)
            {
                RaiseMessage(_currentMonster.Name + "擊殺你");

                MoveHome();
            }
        }
        private void MoveHome()
        {
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
        }
    }
}
