using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AutoBattle.Types;
using System.Security.Cryptography.X509Certificates;

namespace AutoBattle
{
    public class Character
    {
        public string Name { get; set; }
        public string Title; // Added a Title to differentiate between Player and Enemy in the text log
        public bool Invulnerable = false;
        public int Health;
        public int Damage;

        public GridBox currentBox;
        public int PlayerIndex;
        public CharacterClass characterClass;
        public CharacterClassSpecific characterClassSpecific;
        public Character Target { get; set; }
        public SpecialAbility specialAbility;

        // Event triggered whenever the character dies
        public delegate void OnCharacterDied();
        public static OnCharacterDied onCharacterDied;

        public Character(CharacterClass characterClass, string title, int baseHealth, int baseDamage, CharacterClassSpecific characterClassSpecific)
        {
            this.characterClass = characterClass;
            this.Title = title;
            this.Health = (int) (baseHealth * characterClassSpecific.hpModifier);
            this.Damage = (int) (baseDamage * characterClassSpecific.damageModifier);
            this.characterClassSpecific = characterClassSpecific;
            specialAbility = characterClassSpecific.specialAbility;
        }

        public void StartTurn(Grid battlefield)
        {
            // Archer special ability allows for attacks anywhere, so must be rolled before everything else

            if (characterClass == CharacterClass.Archer && !CheckCloseTargets(battlefield))
            {
                bool success = TrySpecialAbility();

                if (success)
                    return;
            }

            CheckSpecialAbilityRemainingTurns(); // Checks for abilities that need to reset

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
                battlefield.drawBattlefield();
            }
        }

        // Shows how much health the character has at the beginning of round
        public void DisplayCharacterStats()
        {
            Console.WriteLine($"{Title} {characterClass} has {Health} HP!");
        }

        // Check in x and y directions if there is any character close enough to be a target.
        // Modified to detect characters in diagonal directions and to check positions based on x and y indexes instead of the index of the box. More reliable this way

        bool CheckCloseTargets(Grid battlefield)
        {
            foreach(GridBox g in battlefield.grids)
            {
                if (((g.xIndex == currentBox.xIndex - 1 && g.yIndex == currentBox.yIndex) ||
                    (g.xIndex == currentBox.xIndex + 1 && g.yIndex == currentBox.yIndex) ||
                    (g.yIndex == currentBox.yIndex + 1 && g.xIndex == currentBox.xIndex) ||
                    (g.yIndex == currentBox.yIndex - 1 && g.xIndex == currentBox.xIndex) ||
                    (g.xIndex == currentBox.xIndex - 1 && g.yIndex == currentBox.yIndex - 1) ||
                    (g.xIndex == currentBox.xIndex + 1 && g.yIndex == currentBox.yIndex - 1) ||
                    (g.xIndex == currentBox.xIndex - 1 && g.yIndex == currentBox.yIndex + 1) ||
                    (g.xIndex == currentBox.xIndex + 1 && g.yIndex == currentBox.yIndex + 1)) && 
                    g.ocupied)
                    
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
            // Maintained linq even at cost of performance for legibility. Swap for loop if performance issues arise

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
                        Console.WriteLine($"{Title} {characterClass} walked north!\n");
                        return;
                    }
                case false:
                    switch (down)
                    {
                        case true:
                            if (left)
                            {
                                Console.WriteLine($"{Title} {characterClass} walked southwest!\n");
                                return;
                            }
                            else if (right)
                            {
                                Console.WriteLine($"{Title} {characterClass} walked southeast!\n");
                                return;
                            }
                            else
                            {
                                Console.WriteLine($"{Title} {characterClass} walked south!\n");
                                return;
                            }
                        case false:
                            if (left)
                            {
                                Console.WriteLine($"{Title} {characterClass} walked west!\n");
                                return;
                            }
                            else if (right)
                            {
                                Console.WriteLine($"{Title} {characterClass} walked east!\n");
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
            int realDamage = Utilities.GetRandomInt(0, (int)Damage);
            Console.WriteLine($"{Title} {characterClass} is attacking the {Target.Title} {Target.characterClass} and did {realDamage} damage\n");
            target.TakeDamage(realDamage);
        }

        // Checks abilities with set amount of turns to end and resets them if needed
        void CheckSpecialAbilityRemainingTurns()
        {
            if (characterClass == CharacterClass.Cleric)
            {
                if (Invulnerable)
                {
                    if (specialAbility.turnsCountDown <= 0) // Resets ability
                    {
                        Invulnerable = false;
                    }
                    else
                    {
                        specialAbility.turnsCountDown--; // Count down the turn
                    }

                }
            }
        }

        // Character takes damage if they are not invulnerable
        public void TakeDamage(int amount)
        {
            if (Invulnerable)
            {
                Console.WriteLine($"{characterClass} took 0 damage because they are invulnerable!\n");
                return;
            }

            Health -= amount;

            if (Health <= 0)
            {
                Console.WriteLine($"{characterClass} died!\n");
                onCharacterDied?.Invoke();
            }
        }

        // Rolls a random chance to execute a special ability based on the own ability's odds of happening
        bool TrySpecialAbility()
        {
            float rolledValue = Utilities.GetRandomFloat(0f, 1f);

            if (rolledValue <= specialAbility.odds)
            {
                ExecuteSpecialAbility();
                return true;
            }
            return false;
        }

        // Executes the class special ability
        void ExecuteSpecialAbility()
        {
            switch (characterClass)
            {
                case CharacterClass.Paladin: // increases paladins' health
                    Console.WriteLine($"{characterClass} uses {specialAbility.abilityName}. They gain a boost of health and now have {Health} HP!\n");
                    Health = (int) (Health * specialAbility.hpModifier);
                    break;
                case CharacterClass.Warrior: // attacks twice with more damage
                    Console.WriteLine($"{characterClass} uses {specialAbility.abilityName}. They grow stronger and attacks twice!\n");
                    int originalBaseDamage = Damage;
                    Damage = (int) (Damage * specialAbility.damageModifier);
                    Attack(Target);
                    Attack(Target);
                    Damage = originalBaseDamage;
                    break;
                case CharacterClass.Cleric: // turns invulnerable for a set amount of turns
                    if (Invulnerable) break;
                    Console.WriteLine($"{characterClass} uses {specialAbility.abilityName}. They become invulnerable for {specialAbility.turnsActive} turns!\n");
                    Invulnerable = true;
                    specialAbility.turnsCountDown = specialAbility.turnsActive;
                    break;
                case CharacterClass.Archer: // attack anywhere
                    Console.WriteLine($"{characterClass} uses {specialAbility.abilityName}. They attack from a distance!\n");
                    Attack(Target);
                    break;
                default:
                    break;
            }
        }
        #endregion Combat Methods
    }
}
