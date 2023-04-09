using System;
using System.Collections.Generic;
using System.Text;

namespace AutoBattle
{
    public class Types
    {
        public struct CharacterClassSpecific
        {
            float hpModifier;
            float ClassDamage; 
            CharacterClass characterClass;
            CharacterSkill skill;

            public CharacterClassSpecific(float hp, float damage, CharacterClass characterClass, CharacterSkill skill)
            {
                this.hpModifier = hp;
                this.ClassDamage = damage;
                this.characterClass = characterClass;
                this.skill = skill;
            }
        }

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

        public struct CharacterSkill
        {
            string Name;
            float damage;
            float damageMultiplier;
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
