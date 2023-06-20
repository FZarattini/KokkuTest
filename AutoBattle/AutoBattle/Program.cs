using System;
using static AutoBattle.Character;
using static AutoBattle.Grid;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using static AutoBattle.Types.SpecialAbility;

namespace AutoBattle
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid grid;
            GridBox EnemyCurrentLocation;
            Character PlayerCharacter;
            Character EnemyCharacter;
            List<Character> AllPlayers;
            Dictionary<CharacterClass, CharacterClassSpecific> classesDictionary;
            Utilities utilities = new Utilities();
            int currentTurn = 0;
            int numberOfPossibleTiles = 0;
            Setup();


            void Setup()
            {
                AllPlayers = new List<Character>();

                InitializeClasses();
                GetBattlefieldSize();
            }

            // Initializes the CharacterClassSpecific dictionary
            void InitializeClasses()
            {
                CharacterClassSpecific paladinClass = new CharacterClassSpecific()
                {
                    characterClass = CharacterClass.Paladin,
                    hpModifier = 1.8f,
                    damageModifier = 1f,
                    specialAbility = new SpecialAbility(CharacterClass.Paladin),
                };

                CharacterClassSpecific warriorClass = new CharacterClassSpecific()
                {
                    characterClass = CharacterClass.Warrior,
                    hpModifier = 1.2f,
                    damageModifier = 1.9f,
                    specialAbility = new SpecialAbility(CharacterClass.Warrior),
                };

                CharacterClassSpecific clericClass = new CharacterClassSpecific()
                {
                    characterClass = CharacterClass.Cleric,
                    hpModifier = 1f,
                    damageModifier = 1.2f,
                    specialAbility = new SpecialAbility(CharacterClass.Cleric),
                };

                CharacterClassSpecific archerClass = new CharacterClassSpecific()
                {
                    characterClass = CharacterClass.Archer,
                    hpModifier = 1f,
                    damageModifier = 1.5f,
                    specialAbility = new SpecialAbility(CharacterClass.Archer),
                };

                classesDictionary = new Dictionary<CharacterClass, CharacterClassSpecific>()
                {
                    { CharacterClass.Paladin, paladinClass},
                    { CharacterClass.Warrior, warriorClass},
                    { CharacterClass.Cleric, clericClass},
                    { CharacterClass.Archer, archerClass},
                };

            }

            // Gets from the player the size of the battlefield and creates it before asking about character choice
            void GetBattlefieldSize()
            {
                Console.WriteLine("What is the size of the battlefield? Write it as nxn, where n is a integer number!\n");
                Console.WriteLine("Example: 5x5,  6x10 or 5x6\n");
                string battlefieldSize = Console.ReadLine();

                // Regular expression pattern
                string pattern = @"(\d+)x(\d+)";

                Match match = utilities.ValidateString(battlefieldSize, pattern);

                // Checks if the choice is valid before attempting to create the grid
                if (match.Success)
                {
                    int xLength = int.Parse(match.Groups[1].Value);
                    int yLength = int.Parse(match.Groups[2].Value);

                    CreateGrid(xLength, yLength);
                    GetPlayerChoice();
                }
                else
                {
                    GetBattlefieldSize();
                }
            }

            // Creates the grid based on the players' battlefield size choice
            void CreateGrid(int xLength, int yLength)
            {
                grid = new Grid(yLength, xLength);

                Console.WriteLine("The battle field has been created\n");

                numberOfPossibleTiles = grid.grids.Count;
            }


            // Gets the character choice for the player
            void GetPlayerChoice()
            {
                //asks for the player to choose between for possible classes via console.
                Console.WriteLine("Choose Between One of this Classes:\n");
                Console.WriteLine("[1] Paladin, [2] Warrior, [3] Cleric, [4] Archer");
                //store the player choice in a variable
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreatePlayerCharacter(Int32.Parse(choice));
                        break;
                    case "2":
                        CreatePlayerCharacter(Int32.Parse(choice));
                        break;
                    case "3":
                        CreatePlayerCharacter(Int32.Parse(choice));
                        break;
                    case "4":
                        CreatePlayerCharacter(Int32.Parse(choice));
                        break;
                    default:
                        GetPlayerChoice();
                        break;
                }

                CreateEnemyCharacter();

                // Assigns the method that handles the end of the game to the event of a character dying
                Character.onCharacterDied += HandleEndGame;
                StartGame();
            }


            void CreatePlayerCharacter(int classIndex)
            {
                CharacterClass characterClass = (CharacterClass)classIndex;

                Console.WriteLine($"Player Class Choice: {characterClass}");

                PlayerCharacter = new Character(characterClass, "Player", 100, 20, classesDictionary[characterClass]);

                PlayerCharacter.PlayerIndex = 0;
            }

            void CreateEnemyCharacter()
            {
                //randomly choose the enemy class and set up vital variables
                int randomInteger = Utilities.GetRandomInt(1, 5); // swapped use of random.next to the method already implemented
                CharacterClass enemyClass = (CharacterClass)randomInteger;

                Console.WriteLine($"Enemy Class Choice: {enemyClass}");

                EnemyCharacter = new Character(enemyClass, "Enemy", 100, 20, classesDictionary[enemyClass]);

                EnemyCharacter.PlayerIndex = 1;
            }

            void StartGame()
            {
                //populates the character variables and targets
                EnemyCharacter.Target = PlayerCharacter;
                PlayerCharacter.Target = EnemyCharacter;

                AllPlayers.Add(PlayerCharacter);
                AllPlayers.Add(EnemyCharacter);

                AlocatePlayers();
                HandleTurn();

            }

            void StartTurn()
            {
                // Might be a good idea to clear the console every turn instead of maintaining past rounds
                //Console.Clear();

                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");

                if (currentTurn == 0)
                {
                    ShufflePlayerList(AllPlayers); // Swapped sorted list for shuffled list for more dynamism
                }

                foreach(Character character in AllPlayers)
                {
                    character.DisplayCharacterStats();
                }

                Console.WriteLine(Environment.NewLine);

                foreach (Character character in AllPlayers)
                {
                    character.StartTurn(grid);
                }

                currentTurn++;
                HandleTurn();
            }

            // Shuffles the order of players' list so that the first player to have a turn is random
            void ShufflePlayerList(List<Character> list)
            {
                list.OrderBy(x => Guid.NewGuid()).ToList();
            }

            void HandleTurn()
            {
                Console.Write(Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Press any key to start the next turn...\n");
                Console.Write(Environment.NewLine + Environment.NewLine);

                ConsoleKeyInfo key = Console.ReadKey();
                StartTurn();
            }

            // Handles the end of the game, triggered by a character dying. Character.onCharacterDied event assigned method
            void HandleEndGame()
            {
                Character.onCharacterDied -= HandleEndGame;

                // Enemy Wins
                if (PlayerCharacter.Health <= 0)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);
                    Console.Write("Game Over!\nYou Lose!\n");
                    CheckRestart();

                }
                else if (EnemyCharacter.Health <= 0) // Player Wins
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);
                    Console.Write("YOU WIN!\n");

                    CheckRestart();
                }
            }

            // Checks if the player wants to play again and resets the game
            void CheckRestart()
            {
                Console.Write("Restart Game? (Y/N)\n");
                string answer = Console.ReadLine();

                switch (answer)
                {
                    case "y":
                    case "Y":
                        ResetValues();
                        Setup();
                        break;
                    case "n":
                    case "N":
                        Environment.Exit(0);
                        break;
                    default:
                        CheckRestart();
                        break;
                }
            }

            void ResetValues()
            {
                currentTurn = 0;
                numberOfPossibleTiles = 0;
            }

            // Positioned AlocateEnemyCharacter's call here instead of inside AlocatePlayerCharacter
            void AlocatePlayers()
            {
                AlocatePlayerCharacter();
                AlocateEnemyCharacter();
            }

            void AlocatePlayerCharacter()
            {
                int random = Utilities.GetRandomInt(0, grid.xLength * grid.yLength);

                GridBox RandomLocation = (grid.grids.ElementAt(random));

                if (!RandomLocation.ocupied)
                {
                    GridBox PlayerCurrentLocation = RandomLocation;
                    RandomLocation.ocupied = true;
                    RandomLocation.occupyingCharacter = PlayerCharacter.characterClass.ToString()[0];
                    grid.grids[random] = RandomLocation;
                    //PlayerCharacter.currentBox = grid.grids[random];
                    PlayerCharacter.currentBox = RandomLocation;
                }
                else
                {
                    AlocatePlayerCharacter();
                }
            }

            void AlocateEnemyCharacter()
            {
                int random = Utilities.GetRandomInt(0, grid.xLength * grid.yLength);
                GridBox RandomLocation = (grid.grids.ElementAt(random));

                if (!RandomLocation.ocupied)
                {
                    EnemyCurrentLocation = RandomLocation;
                    RandomLocation.ocupied = true;
                    RandomLocation.occupyingCharacter = EnemyCharacter.characterClass.ToString()[0];
                    grid.grids[random] = RandomLocation;
                    EnemyCharacter.currentBox = RandomLocation;

                    //changed the hardcoded values to the size of grids
                    grid.drawBattlefield();
                }
                else
                {
                    AlocateEnemyCharacter();
                }
            }
        }
    }
}
