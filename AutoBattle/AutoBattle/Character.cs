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
        public string Title;
        public float Health;
        public float BaseDamage;
        public float DamageMultiplier { get; set; }
        public GridBox currentBox;
        public int PlayerIndex;
        public CharacterClass characterClass;
        public Character Target { get; set; }
        public Character(CharacterClass characterClass)
        {
            this.characterClass = characterClass;
        }


        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (Health <= 0)
            {
                Console.WriteLine($"{characterClass} died!");
            }
        }

        // Death is being handled elsewhere
        /*public void Die()
        {
            //TODO >> maybe kill him?
        }*/

        // Movement is being handled elsewhere
        /*public void WalkTO(bool CanWalk)
        {

        }*/

        public void StartTurn(Grid battlefield)
        {

            if (CheckCloseTargets(battlefield))
            {
                Attack(Target);

                return;
            }
            else
            {
                // if there is no target close enough, calculates in wich direction this character should move to be closer to a possible target
                bool up = false;
                bool down = false;
                bool left = false;
                bool right = false;

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
        }

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

        // Check in x and y directions if there is any character close enough to be a target.
        bool CheckCloseTargets(Grid battlefield)
        {
            bool left = (battlefield.grids.Find(x => x.Index == currentBox.Index - 1).ocupied);
            bool right = (battlefield.grids.Find(x => x.Index == currentBox.Index + 1).ocupied);
            bool up = (battlefield.grids.Find(x => x.Index == currentBox.Index + battlefield.xLength).ocupied);
            bool down = (battlefield.grids.Find(x => x.Index == currentBox.Index - battlefield.xLength).ocupied);

            if (left || right || up || down)
            {
                return true;
            }
            return false;
        }

        public void Attack(Character target)
        {
            // Logging the correct amount of damage taken by target
            int realDamage = Utilities.GetRandomInt(0, (int)BaseDamage);
            target.TakeDamage(realDamage);
            Console.WriteLine($"Player {characterClass} is attacking the player {Target.characterClass} and did {realDamage} damage\n");
        }
    }
}
