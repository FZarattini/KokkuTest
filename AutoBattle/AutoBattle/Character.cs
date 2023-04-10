using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AutoBattle.Types;

namespace AutoBattle
{
    public class Character
    {
        public string Name { get; set; }
        public string Title; // Added a Title to differentiate between Player and Enemy in the text log
        public bool Invulnerable = false;
        public float Health;
        public float BaseDamage;
        public float DamageMultiplier { get; set; }
        public GridBox currentBox;
        public int PlayerIndex;
        public CharacterClass characterClass;
        public SpecialAbility SpecialAbility; // Added a special ability to the character
        public Character Target { get; set; }

        // Event triggered whenever the character dies
        public delegate void OnCharacterDied();
        public static OnCharacterDied onCharacterDied;

        public Character(CharacterClass characterClass)
        {
            this.characterClass = characterClass;
            this.SpecialAbility = new SpecialAbility(characterClass);
        }

        public void StartTurn(Grid battlefield)
        {
            // Archer special ability allows for attacks anywhere, so must be rolled before everything else
            if (characterClass == CharacterClass.Archer)
            {
                bool success = TrySpecialAbility();

                if (success)
                    return;
            }

            CheckSpecialAbilityStatus(); // Make sure there isn't a timed ability that needs to reset

            if (CheckCloseTargets(battlefield))
            {
                bool success = TrySpecialAbility();

                if (success)
                    return;

                // if special ability doesn't happen, then attack as usual
                Attack(Target);
            }
            else
            {
                // if there is no target close enough, calculates in wich direction this character should move to be closer to a possible target
                HandleMovement(battlefield);
            }
        }

        // Check in x and y directions if there is any character close enough to be a target.
        // Modified to detect characters in diagonal directions and to check positions based on x and y indexes instead of the index of the box. More reliable this way
        bool CheckCloseTargets(Grid battlefield)
        {
            bool west = battlefield.grids.Find(x => x.xIndex == currentBox.xIndex - 1 && x.yIndex == currentBox.yIndex).ocupied;
            bool east = battlefield.grids.Find(x => x.xIndex == currentBox.xIndex + 1 && x.yIndex == currentBox.yIndex).ocupied;
            bool north = battlefield.grids.Find(x => x.yIndex == currentBox.yIndex + 1 && x.xIndex == currentBox.xIndex).ocupied;
            bool south = battlefield.grids.Find(x => x.yIndex == currentBox.yIndex - 1 && x.xIndex == currentBox.xIndex).ocupied;
            bool northWest = battlefield.grids.Find(x => x.xIndex == currentBox.xIndex - 1 && x.yIndex == currentBox.yIndex - 1).ocupied;
            bool northEast = battlefield.grids.Find(x => x.xIndex == currentBox.xIndex + 1 && x.yIndex == currentBox.yIndex - 1).ocupied;
            bool southWest = battlefield.grids.Find(x => x.xIndex == currentBox.xIndex - 1 && x.yIndex == currentBox.yIndex + 1).ocupied;
            bool southEast = battlefield.grids.Find(x => x.xIndex == currentBox.xIndex + 1 && x.yIndex == currentBox.yIndex + 1).ocupied;

            if (west || east || north || south || northWest || northEast || southWest || southEast)
            {
                return true;
            }
            return false;
        }

        #region Movement Methods

        // Modified this method to allow for diagonal movement
        void HandleMovement(Grid battlefield)
        {
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;

            // Decides where the character must go based on the target's relative position
            if (currentBox.xIndex > Target.currentBox.xIndex)
            {
                currentBox.ocupied = false;
                currentBox.occupyingCharacter = '\0';
                battlefield.grids[currentBox.Index] = currentBox;
                currentBox = battlefield.grids.Find(x => x.xIndex == currentBox.xIndex - 1 && x.yIndex == currentBox.yIndex);
                currentBox.ocupied = true;
                currentBox.occupyingCharacter = characterClass.ToString()[0];
                battlefield.grids[currentBox.Index] = currentBox;
                left = true;
            }
            else if (currentBox.xIndex < Target.currentBox.xIndex)
            {
                currentBox.ocupied = false;
                currentBox.occupyingCharacter = '\0';
                battlefield.grids[currentBox.Index] = currentBox;
                currentBox = battlefield.grids.Find(x => x.xIndex == currentBox.xIndex + 1 && x.yIndex == currentBox.yIndex);
                currentBox.ocupied = true;
                currentBox.occupyingCharacter = characterClass.ToString()[0];
                battlefield.grids[currentBox.Index] = currentBox;
                right = true;
            }

            // on Y axis, board yIndex goes up as the cells go down
            if (currentBox.yIndex > Target.currentBox.yIndex)
            {
                currentBox.ocupied = false;
                currentBox.occupyingCharacter = '\0';
                battlefield.grids[currentBox.Index] = currentBox;
                currentBox = battlefield.grids.Find(x => x.yIndex == currentBox.yIndex - 1 && x.xIndex == currentBox.xIndex);
                currentBox.ocupied = true;
                currentBox.occupyingCharacter = characterClass.ToString()[0];
                battlefield.grids[currentBox.Index] = currentBox;
                up = true;
            }
            else if (currentBox.yIndex < Target.currentBox.yIndex)
            {
                currentBox.ocupied = false;
                currentBox.occupyingCharacter = '\0';
                battlefield.grids[currentBox.Index] = currentBox;
                currentBox = battlefield.grids.Find(x => x.yIndex == currentBox.yIndex + 1 && x.xIndex == currentBox.xIndex);
                currentBox.ocupied = true;
                currentBox.occupyingCharacter = characterClass.ToString()[0];
                battlefield.grids[currentBox.Index] = currentBox;
                down = true;
            }

            DisplayDirection(up, down, left, right);
            battlefield.drawBattlefield();
        }

        // Displays the direction the character walked as a cardinal direction
        void DisplayDirection(bool up, bool down, bool left, bool right)
        {
            switch (up)
            {
                case true:
                    if (left)
                    {
                        Console.WriteLine($"{Title} {characterClass} walked northwest!");
                        return;
                    }
                    else if (right)
                    {
                        Console.WriteLine($"{Title} {characterClass} walked northeast!");
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"{Title} {characterClass} walked north!");
                        return;
                    }
                case false:
                    switch (down)
                    {
                        case true:
                            if (left)
                            {
                                Console.WriteLine($"{Title} {characterClass} walked southwest!");
                                return;
                            }
                            else if (right)
                            {
                                Console.WriteLine($"{Title} {characterClass} walked southeast!");
                                return;
                            }
                            else
                            {
                                Console.WriteLine($"{Title} {characterClass} walked south!");
                                return;
                            }
                        case false:
                            if (left)
                            {
                                Console.WriteLine($"{Title} {characterClass} walked west!");
                                return;
                            }
                            else if (right)
                            {
                                Console.WriteLine($"{Title} {characterClass} walked east!");
                                return;
                            }
                            break;
                    }
                    break;
            }
        }

        #endregion Movement Methods


        #region Combat Methods

        public void Attack(Character target)
        {
            // Logging the correct amount of damage taken by target
            int realDamage = Utilities.GetRandomInt(0, (int)BaseDamage);
            Console.WriteLine($"{Title} {characterClass} is attacking the {Target.Title} {Target.characterClass} and did {realDamage} damage\n");
            target.TakeDamage(realDamage);
        }

        // Checks abilities with set amount of turns to end and resets them if needed
        void CheckSpecialAbilityStatus()
        {
            if (characterClass == CharacterClass.Cleric)
            {
                if (Invulnerable)
                {
                    if (SpecialAbility.turnsCountDown <= 0) // Resets ability
                    {
                        Invulnerable = false;
                    }
                    else
                    {
                        SpecialAbility.turnsCountDown--; // Count down the turn
                    }

                }
            }
        }

        // Character takes damage if they are not invulnerable
        public void TakeDamage(float amount)
        {
            if (Invulnerable)
            {
                Console.WriteLine($"{characterClass} took 0 damage because they are invulnerable!");
                return;
            }

            Health -= amount;

            if (Health <= 0)
            {
                Console.WriteLine($"{characterClass} died!");
                onCharacterDied?.Invoke();
            }
        }

        // Rolls a random chance to execute a special ability based on the own ability's odds of happening
        bool TrySpecialAbility()
        {
            float rolled = Utilities.GetRandomFloat(0f, 1f);

            if (rolled <= SpecialAbility.odds)
            {
                ExecuteSpecialAbility();
                return true;
            }
            return false;
        }

        // Executes the class special ability;
        void ExecuteSpecialAbility()
        {
            switch (characterClass)
            {
                case CharacterClass.Paladin: // increases paladins' health
                    Console.WriteLine($"{characterClass} uses {SpecialAbility.abilityName}. They gain a boost of health now they have {Health}HP!");
                    Health *= SpecialAbility.hpModifier;
                    break;
                case CharacterClass.Warrior: // attacks twice with more damage
                    Console.WriteLine($"{characterClass} uses {SpecialAbility.abilityName}.They grow stronger and attacks twice!");
                    float originalBaseDamage = BaseDamage;
                    BaseDamage *= SpecialAbility.damageModifier;
                    Attack(Target);
                    Attack(Target);
                    BaseDamage = originalBaseDamage;
                    break;
                case CharacterClass.Cleric: // turns invulnerable for a set amount of turns
                    if (Invulnerable) break;
                    Console.WriteLine($"{characterClass} uses {SpecialAbility.abilityName}. They become invulnerable for {SpecialAbility.turnsActive} turns!");
                    Invulnerable = true;
                    SpecialAbility.turnsCountDown = SpecialAbility.turnsActive;
                    break;
                case CharacterClass.Archer: // attack anywhere
                    Console.WriteLine($"{characterClass} uses {SpecialAbility.abilityName}. They attack from a distance!");
                    Attack(Target);
                    break;
                default:
                    break;
            }
        }
        #endregion Combat Methods
    }
}
