namespace CodingDungeon
{

    interface UserInterface
    {
        // - 사용자 입력 처리 메서드 (메뉴 선택, 아이템 선택 등)
        // - 게임 상태 출력 메서드 (플레이어 정보, 인벤토리, 상점 등)
        // - 게임 진행 상태 표시 메서드 (현재 위치, 진행 단계 등)
        // - 사용자 피드백 메서드 (성공/실패 메시지 등)
    }
    class Player
    {
        public string Name { get; private set; }
        public string Job { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int Health { get; private set; }
        public Player(string name, string job)
        {
            Name = name;
            Job = job;
            SetInitialStats();
        }

        // 직업에 따른 초기 능력치 설정
        private void SetInitialStats()
        {
            switch (Job)
            {
                case "전사":
                    Attack = 10;
                    Defense = 15;
                    Health = 100;
                    break;
                case "마법사":
                    Attack = 15;
                    Defense = 5;
                    Health = 80;
                    break;
                case "궁수":
                    Attack = 12;
                    Defense = 8;
                    Health = 90;
                    break;
                default:
                    Attack = 100;
                    Defense = 100;
                    Health = 100;
                    break;
            }
        }

        public void UpdateStats(int attackModifier, int defenseModifier)
        {
            Attack += attackModifier;
            Defense += defenseModifier;
        }

        public void DisplayStatus()
        {
            Console.WriteLine("===== 캐릭터 정보 =====");
            Console.WriteLine($"이름: {Name}");
            Console.WriteLine($"직업: {Job}");
            Console.WriteLine($"공격력: {Attack}");
            Console.WriteLine($"방어력: {Defense}");
            Console.WriteLine($"체력: {Health}");
            Console.WriteLine("======================");
        }
    }


    class Inventory
    {
        private List<Item> items;         // 보유 아이템 목록
        public int Gold { get; private set; }  // 보유 골드


        // 아이템 목록을 반환하는 메서드
        public List<Item> GetItems()
        {
            return new List<Item>(items);  // 복사본 반환하여 외부 변경 방지
        }



        // 생성자: 인벤토리 초기화
        public Inventory(int startingGold = 1000)
        {
            items = new List<Item>();
            Gold = startingGold;  // 기본 시작 골드 설정
        }

        // 골드 추가 메서드
        public void AddGold(int amount)
        {
            Gold += amount;
            Console.WriteLine($"{amount}G를 획득했습니다. 현재 골드: {Gold}G");
        }

        // 골드 사용 메서드 (구매 등)
        public bool UseGold(int amount)
        {
            // 보유 골드가 충분한지 확인
            if (Gold >= amount)
            {
                Gold -= amount;
                Console.WriteLine($"{amount}G를 사용했습니다. 남은 골드: {Gold}G");
                return true;
            }
            else
            {
                Console.WriteLine("골드가 부족합니다.");
                return false;
            }
        }

        // 아이템 추가 메서드
        public void AddItem(Item item)
        {
            items.Add(item);
            Console.WriteLine($"{item.Name}을(를) 획득했습니다.");
        }

        // 아이템 제거 메서드
        public bool RemoveItem(Item item)
        {
            if (items.Contains(item))
            {
                // 장착 중인 아이템이면 장착 해제
                if (item.IsEquipped)
                {
                    ToggleEquip(item, null); // Player 객체 없이 호출하여 장착만 해제
                }

                items.Remove(item);
                Console.WriteLine($"{item.Name}을(를) 제거했습니다.");
                return true;
            }
            else
            {
                Console.WriteLine("해당 아이템이 인벤토리에 없습니다.");
                return false;
            }
        }

        // 아이템 장착/해제 메서드
        public void ToggleEquip(Item item, Player player)
        {
            if (!items.Contains(item))
            {
                Console.WriteLine("해당 아이템이 인벤토리에 없습니다.");
                return;
            }

            // 아이템 장착 상태 변경
            item.ToggleEquip();

            // Player가 제공된 경우 능력치 업데이트
            if (player != null)
            {
                // 장착/해제에 따라 능력치 변화 적용
                int attackMod = item.IsEquipped ? item.AttackBonus : -item.AttackBonus;
                int defenseMod = item.IsEquipped ? item.DefenseBonus : -item.DefenseBonus;

                player.UpdateStats(attackMod, defenseMod);

                // 상태 메시지 출력
                if (item.IsEquipped)
                    Console.WriteLine($"{item.Name}을(를) 장착했습니다.");
                else
                    Console.WriteLine($"{item.Name}을(를) 해제했습니다.");
            }
        }

        // 인벤토리 출력 메서드
        public void DisplayInventory()
        {
            Console.WriteLine("===== 인벤토리 =====");
            Console.WriteLine($"보유 골드: {Gold}G");
            Console.WriteLine("아이템 목록:");

            if (items.Count == 0)
            {
                Console.WriteLine("아이템이 없습니다.");
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Console.Write($"{i + 1}. ");
                    items[i].DisplayInfo();
                    Console.WriteLine();
                }
            }
            Console.WriteLine("====================");
        }

        // 장착 중인 아이템 목록 출력 메서드
        public void DisplayEquippedItems()
        {
            var equippedItems = items.Where(item => item.IsEquipped).ToList();

            Console.WriteLine("===== 장착 아이템 =====");
            if (equippedItems.Count == 0)
            {
                Console.WriteLine("장착 중인 아이템이 없습니다.");
            }
            else
            {
                foreach (var item in equippedItems)
                {
                    item.DisplayInfo();
                    Console.WriteLine();
                }
            }
            Console.WriteLine("======================");
        }

        // 특정 아이템 찾기 메서드 (이름으로 검색)
        public Item FindItemByName(string name)
        {
            return items.FirstOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }


    class Item
    {
        // 아이템의 기본 속성 정의
        public string Name { get; private set; }      // 아이템 이름
        public string Description { get; private set; }  // 아이템 설명
        public int Price { get; private set; }          // 아이템 가격
        public ItemType Type { get; private set; }      // 아이템 타입(무기, 방어구, 소비품)
        public int AttackBonus { get; private set; }    // 공격력 증가량
        public int DefenseBonus { get; private set; }   // 방어력 증가량
        public int HealthBonus { get; private set; }    // 체력 증가량
        public bool IsEquipped { get; private set; }    // 장착 상태

        // 아이템 타입을 열거형으로 정의
        public enum ItemType
        {
            Weapon,     // 무기
            Armor,      // 방어구
            Consumable  // 소비품
        }

        // 아이템 생성자: 새로운 아이템 객체를 생성할 때 필요한 정보 초기화
        public Item(string name, string description, int price, ItemType type,
                   int attackBonus = 0, int defenseBonus = 0, int healthBonus = 0)
        {
            Name = name;
            Description = description;
            Price = price;
            Type = type;
            AttackBonus = attackBonus;
            DefenseBonus = defenseBonus;
            HealthBonus = healthBonus;
            IsEquipped = false;  // 생성 시 기본적으로 장착되지 않은 상태
        }

        // 아이템 장착 상태 변경 메서드
        public void ToggleEquip()
        {
            IsEquipped = !IsEquipped;  // 현재 상태의 반대로 전환
        }

        // 아이템 정보 출력 메서드
        // 아이템 정보 출력 메서드
        public void DisplayInfo()
        {
            // 장착 중인 아이템이면 이름 앞에 [E] 표시
            string displayName = IsEquipped ? $"[E] {Name}" : Name;

            Console.WriteLine($"[{displayName}] - {Price}G");
            Console.WriteLine($"설명: {Description}");

            // 아이템 능력치 정보 출력
            if (AttackBonus != 0)
                Console.WriteLine($"공격력: +{AttackBonus}");
            if (DefenseBonus != 0)
                Console.WriteLine($"방어력: +{DefenseBonus}");
            if (HealthBonus != 0)
                Console.WriteLine($"체력: +{HealthBonus}");

            // 기존 표시 방식을 제거하고 이름에 [E]만 표시하도록 수정
            // 이미 이름에 [E]가 포함되어 있으므로 추가 표시는 필요없음
        }
    }

    class Shop
    {
        private List<Item> items;  // 판매 아이템 목록

        // 생성자: 상점 초기화
        public Shop()
        {
            items = new List<Item>();
            InitializeShop();  // 기본 아이템 설정
        }

        // 상점 초기화 메서드 (기본 판매 아이템 설정)
        private void InitializeShop()
        {
            // 무기 추가
            items.Add(new Item("낡은 검", "오래된 검입니다. 그래도 무기가 없는 것보다는 낫습니다.", 300, Item.ItemType.Weapon, 5, 0, 0));
            items.Add(new Item("청동 도끼", "단단한 청동으로 만든 도끼입니다.", 500, Item.ItemType.Weapon, 8, 0, 0));
            items.Add(new Item("강철 검", "날카로운 강철로 만든 검입니다.", 1000, Item.ItemType.Weapon, 12, 0, 0));

            // 방어구 추가
            items.Add(new Item("가죽 갑옷", "가죽으로 만든 기본 갑옷입니다.", 300, Item.ItemType.Armor, 0, 5, 0));
            items.Add(new Item("사슬 갑옷", "쇠사슬로 만든 유연한 갑옷입니다.", 800, Item.ItemType.Armor, 0, 10, 0));
            items.Add(new Item("강철 갑옷", "단단한 강철로 만든 갑옷입니다.", 1500, Item.ItemType.Armor, 0, 15, 0));

            // 소비품 추가
            items.Add(new Item("체력 물약", "체력을 30 회복합니다.", 100, Item.ItemType.Consumable, 0, 0, 30));
            items.Add(new Item("고급 체력 물약", "체력을 60 회복합니다.", 300, Item.ItemType.Consumable, 0, 0, 60));
        }

        // 상점 아이템 목록 출력 메서드
        public void DisplayItems()
        {
            if (items.Count == 0)
            {
                Console.WriteLine("판매 중인 아이템이 없습니다.");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                Console.Write($"{i + 1}. ");
                items[i].DisplayInfo();
                Console.WriteLine();
            }
        }

        // 아이템 구매 메서드
        public void BuyItem(Item item, Inventory inventory)
        {
            // 인벤토리에 충분한 골드가 있는지 확인
            if (inventory.Gold < item.Price)
            {
                Console.WriteLine("골드가 부족합니다!");
                return;
            }

            // 아이템 복사본 생성 (상점 아이템은 그대로 두고 복사본을 구매)
            Item boughtItem = new Item(
                item.Name,
                item.Description,
                item.Price,
                item.Type,
                item.AttackBonus,
                item.DefenseBonus,
                item.HealthBonus
            );

            // 인벤토리에 아이템 추가 및 골드 차감
            inventory.UseGold(item.Price);
            inventory.AddItem(boughtItem);

            Console.WriteLine($"{item.Name}을(를) {item.Price}G에 구매했습니다.");
        }

        // 아이템 판매 메서드
        public void SellItem(Item item, Inventory inventory, Player player)
        {
            // 판매 가격은 구매 가격의 절반
            int sellPrice = item.Price / 2;

            // 인벤토리에서 아이템 제거 및 골드 추가
            if (inventory.RemoveItem(item))
            {
                inventory.AddGold(sellPrice);
                Console.WriteLine($"{item.Name}을(를) {sellPrice}G에 판매했습니다.");
            }
        }

        // 아이템 목록을 반환하는 메서드
        public List<Item> GetItems()
        {
            return new List<Item>(items);  // 복사본 반환하여 외부 변경 방지
        }
    }

    class Game
    {
        private Player player;           // 플레이어 객체
        private Inventory inventory;     // 인벤토리 객체
        private Shop shop;               // 상점 객체
        private GameScene currentScene;  // 현재 활성화된 씬


        // 인벤토리 설정 메서드 (StartScene에서 호출)
        public void SetInventory(Inventory newInventory)
        {
            inventory = newInventory;
        }

        // 계속하기 위한 메서드 (메뉴 선택 후 사용)
        private void ContinueAfterAction()
        {
            Console.WriteLine("\n계속하려면 Enter 키를 누르세요...");
            Console.ReadLine();
        }
        // 생성자: 게임 초기화
        public Game()
        {
            // 기본 인벤토리 생성 (시작 골드 1000)
            inventory = new Inventory(1000);

            // 상점 객체 생성 및 초기화
            shop = new Shop();

            // 시작 씬 설정
            currentScene = new StartScene(this);
        }

        // 게임 시작 메서드
        public void Start()
        {
            // 시작 씬 실행
            currentScene.Run();

            // 플레이어 생성 후 메인 메뉴로 전환
            if (player != null)
            {
                MainMenu();
            }
        }

        // 플레이어 설정 메서드 (StartScene에서 호출)
        public void SetPlayer(Player newPlayer)
        {
            player = newPlayer;
        }

        // 메인 메뉴 구현
        // 메인 메뉴 구현
        private void MainMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("작업을 마치고 돌아온다. 전초기지는 인적없이 조용하다.");
                Console.WriteLine("===== 스파르타 던전 메인 메뉴 =====");
                Console.WriteLine("1. 상태 보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");
                Console.WriteLine("4. 던전 입장");
                Console.WriteLine("0. 게임 종료");
                Console.WriteLine("=============================");

                int choice = GetUserInput(0, 4);

                switch (choice)
                {
                    case 0:
                        exit = true;
                        Console.WriteLine("게임을 종료합니다...");
                        ContinueAfterAction();
                        break;
                    case 1:
                        // 상태 보기 씬으로 전환
                        currentScene = new StatusScene(player, inventory);
                        currentScene.Run();
                        break;
                    case 2:
                        // 인벤토리 씬으로 전환
                        currentScene = new InventoryScene(player, inventory);
                        currentScene.Run();
                        break;
                    case 3:
                        // 상점 씬으로 전환
                        currentScene = new ShopScene(player, inventory, shop);
                        currentScene.Run();
                        break;
                    case 4:
                        // 던전 기능은 아직 구현 중임을 알림
                        Console.Clear();
                        Console.WriteLine("===== 던전 입장 =====");
                        Console.WriteLine("\n던전 기능은 아직 구현 중입니다!");
                        Console.WriteLine("다음 업데이트를 기대해 주세요.");
                        Console.WriteLine("\n계속하려면 Enter 키를 누르세요...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        // 사용자 입력을 받는 메서드
        private int GetUserInput(int min, int max)
        {
            int input;
            bool isValid = false;

            do
            {
                Console.Write("선택: ");
                isValid = int.TryParse(Console.ReadLine(), out input);
                isValid = isValid && input >= min && input <= max;

                if (!isValid)
                    Console.WriteLine($"잘못된 입력입니다. {min}에서 {max} 사이의 값을 입력하세요.");

            } while (!isValid);

            return input;
        }
    }

    abstract class GameScene
    {
        // 씬의 이름 속성
        public string SceneName { get; protected set; }

        // 씬 초기화 메서드
        public virtual void Initialize()
        {
            // 기본 초기화 로직
            Console.Clear();  // 화면 지우기
        }

        // 씬 실행 메서드 (각 자식 클래스에서 구현해야 함)
        public abstract void Run();

        // 씬 종료 메서드
        public virtual void Exit()
        {
            Console.WriteLine($"{SceneName} 씬을 종료합니다...");
            Console.WriteLine("계속하려면 아무 키나 누르세요...");
            Console.ReadKey();
        }

        // 사용자 입력을 받는 메서드
        protected int GetUserInput(int min, int max)
        {
            int input;
            bool isValid = false;

            do
            {
                Console.Write("선택: ");
                isValid = int.TryParse(Console.ReadLine(), out input);
                isValid = isValid && input >= min && input <= max;

                if (!isValid)
                    Console.WriteLine($"잘못된 입력입니다. {min}에서 {max} 사이의 값을 입력하세요.");

            } while (!isValid);

            return input;
        }
    }

    class StartScene : GameScene
    {
        private Game game;  // Game 객체 참조를 저장

        // 생성자: 게임 참조 설정
        public StartScene(Game game)
        {
            SceneName = "시작 화면";
            this.game = game;
        }

        // 씬 초기화 메서드
        public override void Initialize()
        {
            base.Initialize();
        }

        // 씬 실행 메서드
        public override void Run()
        {
            Initialize();
            DisplayWelcomeMessage();

            // 사용자가 엔터 키를 누를 때까지 대기
            Console.ReadLine();

            // 캐릭터 생성 절차 시작
            Player player = CreateCharacter();

            // 생성된 플레이어 객체를 게임에 전달
            game.SetPlayer(player);

            Exit();
        }

        // 환영 메시지 표시 메서드: 게임 시작 시 플레이어에게 환영 메시지 출력
        public void DisplayWelcomeMessage()
        {
            Console.WriteLine("===== 스파르타 던전에 오신 것을 환영합니다 =====");
            Console.WriteLine("지하 20층의 던전에서 적들과 싸우고 보물을 찾으세요.");
            Console.WriteLine("게임을 시작하려면 Enter 키를 누르세요.");
            Console.WriteLine("===========================================");
        }

        // 게임 시작 안내 및 캐릭터 생성 메서드
        public Player CreateCharacter()
        {
            string playerName = GetPlayerName();
            string playerJob = GetPlayerJob();

            // 이름과 직업을 통해 플레이어 객체 생성
            Player newPlayer = new Player(playerName, playerJob);

            if (playerJob == "육군 아미타이거")
            {
                Console.Clear(); // 화면을 한번 지우고

                // 먼저 요정 대사만 출력
                Console.WriteLine();
                Console.WriteLine("갑자기 요정이 나타난다");
                Console.WriteLine("요정 : \"역시 그렇죠? 화약과 공학의 시대에 검과 활이라뇨...어휴 야만적이야.\"");
                Console.WriteLine("요정 : \"이제부터 당신은 정예 국군장병입니다!\"");

                // 사용자 입력 대기
                Console.WriteLine("\n계속하려면 Enter 키를 누르세요...");
                Console.ReadLine();

                // 엔터 누른 후에 캐릭터 생성 메시지 표시
                Console.WriteLine($"\n{playerName} 캐릭터가 생성되었습니다!");
                Console.WriteLine($"직업: {playerJob}");
            }
            else
            {
                Console.WriteLine($"\n{playerName} 캐릭터가 생성되었습니다!");
                Console.WriteLine($"직업: {playerJob}");
            }


            // 인벤토리 생성 (기본 골드: 1000)
            Inventory inventory = new Inventory(1000);

            // 직업별 시작 장비 지급
            switch (playerJob)
            {
                case "전사":
                    // 전사 시작 장비
                    Item warriorSword = new Item("견습 기사의 검", "기본적인 검입니다.", 300, Item.ItemType.Weapon, 5, 0, 0);
                    Item warriorArmor = new Item("가죽 갑옷", "기본적인 보호를 제공합니다.", 200, Item.ItemType.Armor, 0, 5, 0);

                    inventory.AddItem(warriorSword);
                    inventory.AddItem(warriorArmor);

                    Console.WriteLine("\n시작 장비가 지급되었습니다:");
                    Console.WriteLine(" - 견습 기사의 검 (공격력 +5)");
                    Console.WriteLine(" - 가죽 갑옷 (방어력 +5)");
                    break;

                case "마법사":
                    // 마법사 시작 장비
                    Item wizardStaff = new Item("견습 마법사의 지팡이", "마법 에너지를 모으는 기본 지팡이입니다.", 300, Item.ItemType.Weapon, 5, 0, 0);
                    Item wizardRobe = new Item("천 로브", "마법 에너지에 반응하는 로브입니다.", 200, Item.ItemType.Armor, 0, 3, 0);

                    inventory.AddItem(wizardStaff);
                    inventory.AddItem(wizardRobe);

                    Console.WriteLine("\n시작 장비가 지급되었습니다:");
                    Console.WriteLine(" - 견습 마법사의 지팡이 (공격력 +5)");
                    Console.WriteLine(" - 천 로브 (방어력 +3)");
                    break;

                case "궁수":
                    // 궁수 시작 장비
                    Item archerBow = new Item("연습용 활", "기본적인 활입니다.", 300, Item.ItemType.Weapon, 5, 0, 0);
                    Item archerVest = new Item("가벼운 조끼", "움직임을 방해하지 않는 가벼운 보호구입니다.", 200, Item.ItemType.Armor, 0, 4, 0);

                    inventory.AddItem(archerBow);
                    inventory.AddItem(archerVest);

                    Console.WriteLine("\n시작 장비가 지급되었습니다:");
                    Console.WriteLine(" - 연습용 활 (공격력 +5)");
                    Console.WriteLine(" - 가벼운 조끼 (방어력 +4)");
                    break;

                case "육군 아미타이거":
                    // 육군 아미타이거 특별 장비 (기존 코드)
                    inventory = new Inventory(5000);  // 더 많은 초기 골드 지급

                    // 특별 무기 추가 (공격력 2배)
                    Item k2 = new Item("K2 소총", "대한민국 군인의 주력 소총입니다.", 2000, Item.ItemType.Weapon, 20, 0, 0);
                    Item k2c1 = new Item("K2C1 소총", "K2의 개량형 소총으로 전술레일이 장착되어 있습니다.", 3000, Item.ItemType.Weapon, 25, 0, 0);

                    // 특별 방어구 추가 (방어력 각각 +25)
                    Item gasMask = new Item("K3 방독면", "화생방 공격으로부터 보호해주는 장비입니다.", 1000, Item.ItemType.Armor, 0, 25, 0);
                    Item bodyArmor = new Item("레벨3 방탄복", "총탄을 막아주는 고급 방탄복입니다.", 2000, Item.ItemType.Armor, 0, 25, 0);
                    Item helmet = new Item("경량화 방탄헬맷", "가볍고 튼튼한 군용 헬멧입니다.", 1500, Item.ItemType.Armor, 0, 25, 0);

                    // 인벤토리에 장비 추가
                    inventory.AddItem(k2);
                    inventory.AddItem(k2c1);
                    inventory.AddItem(gasMask);
                    inventory.AddItem(bodyArmor);
                    inventory.AddItem(helmet);

                    Console.WriteLine("\n특별 장비가 지급되었습니다:");
                    Console.WriteLine(" - K2 소총 (공격력 +20)");
                    Console.WriteLine(" - K2C1 소총 (공격력 +25)");
                    Console.WriteLine(" - K3 방독면 (방어력 +25)");
                    Console.WriteLine(" - 레벨3 방탄복 (방어력 +25)");
                    Console.WriteLine(" - 경량화 방탄헬맷 (방어력 +25)");
                    break;

                default:
                    // 기본 장비
                    Item basicSword = new Item("녹슨 검", "오래된 기본 무기입니다.", 100, Item.ItemType.Weapon, 3, 0, 0);
                    inventory.AddItem(basicSword);

                    Console.WriteLine("\n시작 장비가 지급되었습니다:");
                    Console.WriteLine(" - 녹슨 검 (공격력 +3)");
                    break;
            }

            // 게임 인벤토리에 설정
            game.SetInventory(inventory);

            Console.WriteLine("\n계속하려면 아무 키나 누르세요...");
            Console.ReadKey();

            return newPlayer;
        }

        // 플레이어 이름 입력 받는 메서드
        private string GetPlayerName()
        {
            Console.Clear();
            Console.WriteLine("===== 캐릭터 생성 =====");
            Console.WriteLine("캐릭터의 이름을 입력하세요:");

            string name;
            do
            {
                name = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("이름을 입력해주세요!");
                }
            } while (string.IsNullOrEmpty(name));

            return name;
        }

        // 플레이어 직업 선택 메서드
        // 플레이어 직업 선택 메서드
        private string GetPlayerJob()
        {
            Console.WriteLine("\n캐릭터의 직업을 선택해주세요:");
            Console.WriteLine("1. 전사 - 높은 방어력과 체력을 가진 근접 전투 전문가");
            Console.WriteLine("2. 마법사 - 강력한 공격력을 가진 원거리 마법 사용자");
            Console.WriteLine("3. 궁수 - 균형 잡힌 능력치를 가진 원거리 공격 전문가");

            // 사용자 입력 검증
            Console.Write("선택 (번호 또는 직업명 입력): ");
            string input = Console.ReadLine().Trim();

            // 입력이 없는 경우 기본값 반환
            if (string.IsNullOrEmpty(input))
            {
                return "육군 아미타이거";
            }

            // 번호로 입력한 경우 처리
            if (int.TryParse(input, out int choice))
            {
                switch (choice)
                {
                    case 1:
                        return "전사";
                    case 2:
                        return "마법사";
                    case 3:
                        return "궁수";
                    default:
                        return "육군 아미타이거";  // 잘못된 번호 입력 시 기본값
                }
            }

            // 직업명으로 입력한 경우 처리 (대소문자 구분 없이)
            string inputLower = input.ToLower();
            if (inputLower == "전사" || inputLower == "warrior")
            {
                return "전사";
            }
            else if (inputLower == "마법사" || inputLower == "wizard" || inputLower == "mage")
            {
                return "마법사";
            }
            else if (inputLower == "궁수" || inputLower == "archer")
            {
                return "궁수";
            }
            else if (inputLower == "육군 아미타이거" || inputLower == "아미타이거" || inputLower == "army" || inputLower == "tiger")
            {
                return "육군 아미타이거";
            }

            // 알 수 없는 입력은 기본값 반환
            return "육군 아미타이거";
        }
    }

    class InventoryScene : GameScene
    {
        private Player player;
        private Inventory inventory;

        // 생성자: 필요한 참조 설정
        public InventoryScene(Player player, Inventory inventory)
        {
            SceneName = "인벤토리";
            this.player = player;
            this.inventory = inventory;

        }
        // 씬 초기화 메서드
        public override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("===== 인벤토리 =====");
        }

        // 씬 실행 메서드
        public override void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Initialize();

                // 인벤토리 내용 표시
                inventory.DisplayInventory();

                Console.WriteLine("\n무엇을 하시겠습니까?");
                Console.WriteLine("1. 아이템 장착/해제하기");
                Console.WriteLine("0. 돌아가기");

                int choice = GetUserInput(0, 1);

                switch (choice)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        EquipItem();
                        Console.WriteLine("\n계속하려면 Enter 키를 누르세요...");
                        Console.ReadLine();
                        break;
                }
            }

            Exit();
        }

        // 아이템 장착/해제 메서드
        private void EquipItem()
        {
            Console.WriteLine("\n장착/해제할 아이템의 번호를 입력하세요 (0: 취소):");

            // 인벤토리의 아이템 목록 얻기
            List<Item> items = inventory.GetItems();

            if (items.Count == 0)
            {
                Console.WriteLine("인벤토리에 아이템이 없습니다.");
                return;
            }

            int choice = GetUserInput(0, items.Count);

            if (choice == 0)
                return;

            // 선택한 아이템 장착/해제
            inventory.ToggleEquip(items[choice - 1], player);
        }
    }
    class StatusScene : GameScene
    {
        private Player player;
        private Inventory inventory;

        // 생성자: 필요한 참조 설정
        public StatusScene(Player player, Inventory inventory)
        {
            SceneName = "상태 보기";
            this.player = player;
            this.inventory = inventory;
        }

        // 씬 초기화 메서드
        public override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("===== 상태 보기 =====");
        }

        // 씬 실행 메서드
        public override void Run()
        {
            Initialize();

            // 플레이어 상태 표시
            player.DisplayStatus();

            // 장착 아이템 표시
            inventory.DisplayEquippedItems();

            // 사용자 입력 대기
            Console.WriteLine("메인 메뉴로 돌아가려면 아무 키나 누르세요...");
            Console.ReadKey();

            Exit();
        }
    }

    class ShopScene : GameScene
    {
        private Player player;
        private Inventory inventory;
        private Shop shop;

        // 생성자: 필요한 참조 설정
        public ShopScene(Player player, Inventory inventory, Shop shop)
        {
            SceneName = "상점";
            this.player = player;
            this.inventory = inventory;
            this.shop = shop;
        }

        // 씬 초기화 메서드
        public override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("===== 스파르타 상점 =====");
            Console.WriteLine("몸이 근육질인, 검게 탄 상점 주인이 당신을 반긴다.");
            Console.WriteLine("그렇게 친절해 보이는 인상은 아니다.");
            Console.WriteLine("상점 주인: \"필요한 물건만 빠르게 고르게나.\"");
            Console.WriteLine($"보유 골드: {inventory.Gold}G");
        }

        // 씬 실행 메서드
        public override void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Initialize();

                Console.WriteLine("\n1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("0. 돌아가기");

                int choice = GetUserInput(0, 2);

                switch (choice)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        BuyItem();
                        Console.WriteLine("\n계속하려면 Enter 키를 누르세요...");
                        Console.ReadLine();
                        break;
                    case 2:
                        SellItem();
                        Console.WriteLine("\n계속하려면 Enter 키를 누르세요...");
                        Console.ReadLine();
                        break;
                }
            }

            Exit();
        }

        // 아이템 구매 메서드
        private void BuyItem()
        {
            Console.WriteLine("\n===== 구매 가능한 아이템 =====");
            shop.DisplayItems();

            Console.WriteLine("\n구매할 아이템의 번호를 입력하세요 (0: 취소):");
            List<Item> shopItems = shop.GetItems();

            if (shopItems.Count == 0)
            {
                Console.WriteLine("상점에 아이템이 없습니다.");
                return;
            }

            int choice = GetUserInput(0, shopItems.Count);

            if (choice == 0)
                return;

            // 선택한 아이템 구매
            shop.BuyItem(shopItems[choice - 1], inventory);
        }

        // 아이템 판매 메서드
        private void SellItem()
        {
            Console.WriteLine("\n===== 판매 가능한 아이템 =====");
            inventory.DisplayInventory();

            Console.WriteLine("\n판매할 아이템의 번호를 입력하세요 (0: 취소):");
            List<Item> inventoryItems = inventory.GetItems();

            if (inventoryItems.Count == 0)
            {
                Console.WriteLine("인벤토리에 아이템이 없습니다.");
                return;
            }

            int choice = GetUserInput(0, inventoryItems.Count);

            if (choice == 0)
                return;

            // 선택한 아이템 판매
            shop.SellItem(inventoryItems[choice - 1], inventory, player);
        }
    }


    class DungeonScene : GameScene
    {
        private Player player;
        private Inventory inventory;
        private Random random;

        // 생성자: 필요한 참조 설정
        public DungeonScene(Player player, Inventory inventory)
        {
            SceneName = "던전";
            this.player = player;
            this.inventory = inventory;
            random = new Random();
        }

        // 씬 초기화 메서드
        public override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("===== 던전 입장 =====");
        }

        // 씬 실행 메서드
        public override void Run()
        {
            Initialize();

            Console.WriteLine("어떤 던전에 입장하시겠습니까?");
            Console.WriteLine("1. 쉬운 던전 (권장: 공격력 5+)");
            Console.WriteLine("2. 일반 던전 (권장: 공격력 10+)");
            Console.WriteLine("3. 어려운 던전 (권장: 공격력 15+)");
            Console.WriteLine("0. 돌아가기");

            int choice = GetUserInput(0, 3);

            if (choice == 0)
            {
                Exit();
                return;
            }

            // 던전 탐험 시작
            ExploreDungeon(choice);

            Console.WriteLine("\n계속하려면 Enter 키를 누르세요...");
            Console.ReadLine();

            Exit();
        }

        // 던전 탐험 메서드
        private void ExploreDungeon(int dungeonLevel)
        {
            Console.WriteLine("\n던전에 입장합니다...");

            // 던전 난이도에 따른 적 능력치 및 보상 설정
            int enemyAttack = 0;
            int enemyDefense = 0;
            int rewardGold = 0;

            switch (dungeonLevel)
            {
                case 1:  // 쉬운 던전
                    enemyAttack = 5;
                    enemyDefense = 3;
                    rewardGold = 500;
                    break;
                case 2:  // 일반 던전
                    enemyAttack = 10;
                    enemyDefense = 6;
                    rewardGold = 1500;
                    break;
                case 3:  // 어려운 던전
                    enemyAttack = 18;
                    enemyDefense = 12;
                    rewardGold = 3000;
                    break;
            }

            Console.WriteLine("전투가 시작됩니다!");
            Console.WriteLine($"적 공격력: {enemyAttack}, 적 방어력: {enemyDefense}");

            // 전투 결과 계산
            bool success = SimulateBattle(enemyAttack, enemyDefense);

            if (success)
            {
                // 성공 시 보상 지급
                Console.WriteLine("\n던전 공략 성공!");
                Console.WriteLine($"보상으로 {rewardGold}G를 획득했습니다!");
                inventory.AddGold(rewardGold);

                // 랜덤 아이템 획득 가능성 (30%)
                if (random.Next(100) < 30)
                {
                    Item reward = GenerateRandomItem(dungeonLevel);
                    inventory.AddItem(reward);
                    Console.WriteLine($"추가 보상으로 {reward.Name}을(를) 획득했습니다!");
                }
            }
            else
            {
                // 실패 시 패널티
                Console.WriteLine("\n던전 공략 실패...");
                Console.WriteLine("체력이 모두 소진되어 마을로 귀환합니다.");

                // 패널티: 일부 골드 상실 (10%)
                int penalty = inventory.Gold / 10;
                inventory.UseGold(penalty);
                Console.WriteLine($"귀환 비용으로 {penalty}G를 소모했습니다.");
            }
        }

        // 전투 시뮬레이션 메서드
        private bool SimulateBattle(int enemyAttack, int enemyDefense)
        {
            // 간단한 전투 계산: 플레이어와 적의 능력치 비교
            int playerEffectiveAttack = player.Attack - enemyDefense;
            if (playerEffectiveAttack < 1) playerEffectiveAttack = 1;

            int enemyEffectiveAttack = enemyAttack - player.Defense;
            if (enemyEffectiveAttack < 1) enemyEffectiveAttack = 1;

            // 플레이어가 적을 물리치는데 필요한 턴 수
            int turnsToDefeatEnemy = (enemyDefense * 2) / playerEffectiveAttack;
            if (turnsToDefeatEnemy < 1) turnsToDefeatEnemy = 1;

            // 적이 플레이어를 물리치는데 필요한 턴 수
            int turnsToDefeatPlayer = player.Health / enemyEffectiveAttack;
            if (turnsToDefeatPlayer < 1) turnsToDefeatPlayer = 1;

            // 전투 시뮬레이션 결과 출력
            Console.WriteLine("\n전투 진행 중...");
            Console.WriteLine($"플레이어 턴당 피해량: {playerEffectiveAttack}");
            Console.WriteLine($"적 턴당 피해량: {enemyEffectiveAttack}");

            // 플레이어가 더 빨리 적을 물리치면 승리
            return turnsToDefeatEnemy <= turnsToDefeatPlayer;
        }

        // 랜덤 아이템 생성 메서드
        private Item GenerateRandomItem(int dungeonLevel)
        {
            string[] weaponNames = { "녹슨 검", "단검", "장검", "마법 지팡이", "대검" };
            string[] armorNames = { "천 갑옷", "가죽 조끼", "청동 갑옷", "강철 갑옷", "마법 로브" };

            // 아이템 타입 결정 (무기 50%, 방어구 50%)
            Item.ItemType type = random.Next(2) == 0 ? Item.ItemType.Weapon : Item.ItemType.Armor;

            string name;
            string description;
            int price;
            int attackBonus = 0;
            int defenseBonus = 0;

            // 던전 레벨에 따른 아이템 능력치 스케일링
            int statBonus = dungeonLevel * (random.Next(3, 6));
            price = dungeonLevel * random.Next(300, 600);

            if (type == Item.ItemType.Weapon)
            {
                name = weaponNames[random.Next(weaponNames.Length)];
                description = "던전에서 발견한 무기입니다.";
                attackBonus = statBonus;
            }
            else
            {
                name = armorNames[random.Next(armorNames.Length)];
                description = "던전에서 발견한 방어구입니다.";
                defenseBonus = statBonus;
            }

            return new Item(name, description, price, type, attackBonus, defenseBonus, 0);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            // 게임 객체 생성
            Game game = new Game();

            // 게임 시작
            game.Start();
        }
    }
}
