﻿using System;
using static AutoBattle.Character;
using static AutoBattle.Grid;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace AutoBattle
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid grid;
            CharacterClass playerCharacterClass;
            GridBox PlayerCurrentLocation;
            GridBox EnemyCurrentLocation;
            Character PlayerCharacter;
            Character EnemyCharacter;
            List<Character> AllPlayers = new List<Character>();
            Utilities utilities = new Utilities();
            int currentTurn = 0;
            int numberOfPossibleTiles = 0;
            Setup(); 


            void Setup()
            {
                GetBattlefieldSize();
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
                    Console.WriteLine(match.Groups[1].Value);
                    Console.WriteLine(match.Groups[2].Value);

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
            }


            void CreatePlayerCharacter(int classIndex)
            {
               
                CharacterClass characterClass = (CharacterClass)classIndex;
                Console.WriteLine($"Player Class Choice: {characterClass}");
                PlayerCharacter = new Character(characterClass);
                PlayerCharacter.Title = "Player";
                PlayerCharacter.Health = 100;
                PlayerCharacter.BaseDamage = 20;
                PlayerCharacter.PlayerIndex = 0;
                
                CreateEnemyCharacter();

            }

            void CreateEnemyCharacter()
            {
                //randomly choose the enemy class and set up vital variables
                int randomInteger = Utilities.GetRandomInt(1, 4); // swapped use of random.next to the method already implemented
                CharacterClass enemyClass = (CharacterClass)randomInteger;
                Console.WriteLine($"Enemy Class Choice: {enemyClass}");
                EnemyCharacter = new Character(enemyClass);
                EnemyCharacter.Title = "Enemy";
                EnemyCharacter.Health = 100;
                EnemyCharacter.BaseDamage = 20;
                EnemyCharacter.PlayerIndex = 1;
                StartGame();

            }

            void StartGame()
            {
                //populates the character variables and targets
                EnemyCharacter.Target = PlayerCharacter;
                PlayerCharacter.Target = EnemyCharacter;
                AllPlayers.Add(PlayerCharacter);
                AllPlayers.Add(EnemyCharacter);
                AlocatePlayers();
                StartTurn();

            }

            void StartTurn(){

                if (currentTurn == 0)
                {
                    ShufflePlayerList(AllPlayers); // Swapped sorted list for shuffled list for more dynamism
                }

                foreach(Character character in AllPlayers)
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
                // Enemy Wins
                if(PlayerCharacter.Health <= 0)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);
                    Console.Write("Game Over!\nYou Lose!\n");
                    CheckRestart();

                } else if (EnemyCharacter.Health <= 0) // Player Wins
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);
                    Console.Write("YOU WIN!\n");

                    CheckRestart();
                } else
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);
                    Console.WriteLine("Click on any key to start the next turn...\n");
                    Console.Write(Environment.NewLine + Environment.NewLine);

                    ConsoleKeyInfo key = Console.ReadKey();
                    StartTurn();
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

            void ResetValues(){
                currentTurn = 0;
                numberOfPossibleTiles = 0;
            }

            void AlocatePlayers()
            {
                AlocatePlayerCharacter();
                AlocateEnemyCharacter();
            }

            void AlocatePlayerCharacter()
            {
                int random = Utilities.GetRandomInt(0, grid.xLength * grid.yLength);

                GridBox RandomLocation = (grid.grids.ElementAt(random));
                Console.Write($"{random}\n");
                if (!RandomLocation.ocupied)
                {
                    GridBox PlayerCurrentLocation = RandomLocation;
                    RandomLocation.ocupied = true;
                    RandomLocation.occupyingCharacter = PlayerCharacter.characterClass.ToString()[0];
                    grid.grids[random] = RandomLocation;
                    //PlayerCharacter.currentBox = grid.grids[random];
                    PlayerCharacter.currentBox = RandomLocation;
                } else
                {
                    AlocatePlayerCharacter();
                }
            }

            void AlocateEnemyCharacter()
            {
                int random = Utilities.GetRandomInt(0, grid.xLength * grid.yLength);
                GridBox RandomLocation = (grid.grids.ElementAt(random));
                Console.Write($"{random}\n");
                if (!RandomLocation.ocupied)
                {
                    EnemyCurrentLocation = RandomLocation;
                    RandomLocation.ocupied = true;
                    RandomLocation.occupyingCharacter = EnemyCharacter.characterClass.ToString()[0];
                    grid.grids[random] = RandomLocation;
                    //EnemyCharacter.currentBox = grid.grids[random];
                    EnemyCharacter.currentBox = RandomLocation;

                    //changed the hardcored values to the size of grids
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
