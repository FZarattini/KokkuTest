using System;
using System.Collections.Generic;
using System.Text;

namespace AutoBattle
{
    public class Types
    {
        public struct GridBox
        {
            public int xIndex;
            public int yIndex;
            public bool ocupied;
            public int Index;
            public char occupyingCharacter; // The first letter of the character class to represent him

            public GridBox(int x, int y, bool ocupied, int index, char occupyingCharacter)
            {
                xIndex = x;
                yIndex = y;
                this.ocupied = ocupied;
                this.Index = index;
                this.occupyingCharacter = occupyingCharacter;
            }
        }

        public struct CharacterClassSpecific
        {
            public CharacterClass characterClass;
            public float hpModifier;
            public float damageModifier;
            public SpecialAbility specialAbility;
        }

        // Struct describing the special abilities of the character classes
        public struct SpecialAbility
        {
            public string abilityName;
            public CharacterClass characterClass;
            public float hpModifier;
            public float damageModifier;
            public int turnsActive; // Doesn't revert if the value is 0
            public int turnsCountDown;
            public float odds;

            // creates the special ability data based on the character class
            public SpecialAbility(CharacterClass characterClass) 
            { 
                switch (characterClass)
                {
                    case CharacterClass.Paladin: // Increases the character HP by 20% everytime it activates
                        this.characterClass = characterClass;
                        this.abilityName = "Endure";
                        this.hpModifier = 1.2f;
                        this.damageModifier = 1f;
                        turnsActive = 0;
                        turnsCountDown = 0;
                        odds = 0.2f;
                        break;
                    case CharacterClass.Warrior: // Hits twice with 20% more damage
                        this.characterClass = characterClass;
                        this.abilityName = "Berserker Strike";
                        this.hpModifier = 1f;
                        this.damageModifier = 1.2f;
                        turnsActive = 0;
                        turnsCountDown = 0;
                        odds = 0.25f;
                        break;
                    case CharacterClass.Cleric: // Character doesn't take any damage for 2 turns;
                        this.characterClass = characterClass;
                        this.abilityName = "Holy";
                        this.hpModifier = 1f;
                        this.damageModifier = 1f;
                        turnsActive = 2;
                        turnsCountDown = 0;
                        odds = 0.10f;
                        break;
                    case CharacterClass.Archer: // Hits target anywhere on the battlefield;
                        this.characterClass = characterClass;
                        this.abilityName = "Long Shot";
                        this.hpModifier = 1f;
                        this.damageModifier = 1f;
                        this.turnsActive = 0;
                        turnsCountDown = 0;
                        odds = 0.40f;
                        break;
                    default:
                        this.characterClass = characterClass;
                        this.abilityName = "";
                        this.hpModifier = 1f;
                        this.damageModifier = 1f;
                        this.turnsActive = 0;
                        turnsCountDown = 0;
                        odds = 0f;
                        break;
                }
            }
        }

        public enum CharacterClass : uint
        {
            Paladin = 1,
            Warrior = 2,
            Cleric = 3,
            Archer = 4
        }

    }
}
