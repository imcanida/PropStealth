using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PropStealth
{
    public static class GameHelpers
    {
        public class CharactersWithinRange
        {
            public Character character { get; set; }
            public float distance { get; set; }
            public float angle { get; set; }
            public bool isNPC { get; set; }
        }

        public static List<CharactersWithinRange> CharactersWithInRange(this Player player, float distance, List<Character> checkList = null)
        {
            List<CharactersWithinRange> results = new List<CharactersWithinRange>();
            foreach (Character character in checkList ?? Character.GetAllCharacters())
            {
                Vector3 direction = player.transform.position - character.transform.position;
                if (direction.magnitude < distance)
                {
                    results.Add(new CharactersWithinRange()
                    {
                        character = character,
                        distance = distance,
                        angle = Vector3.Angle(player.GetLookDir(), direction),
                        isNPC = BaseAI.IsEnemy(player, character)
                    });
                }
            }
            return results;
        }

        public static List<CharactersWithinRange> CharactersWithInAngle(this Player player, float distance, float angle, List<Character> checkList = null)
        {
            List<CharactersWithinRange> results = new List<CharactersWithinRange>();
            foreach (Character character in checkList ?? Character.GetAllCharacters())
            {
                Vector3 direction = player.transform.position - character.transform.position;
                if (direction.magnitude < distance)
                {
                    if (Vector3.Angle(player.GetLookDir(), direction) > angle) continue;
                    results.Add(new CharactersWithinRange()
                    {
                        character = character,
                        distance = distance,
                        angle = Vector3.Angle(player.GetLookDir(), direction),
                        isNPC = BaseAI.IsEnemy(player, character)
                    });
                }
            }
            return results;
        }

        public static List<CharactersWithinRange> CharactersWithInAngle(this Transform transform, Vector3 lookDir, float distance, float angle, List<Character> checkList = null)
        {
            List<CharactersWithinRange> results = new List<CharactersWithinRange>();
            foreach (Character character in checkList ?? Character.GetAllCharacters())
            {
                Vector3 direction = transform.position - character.transform.position;
                if (direction.magnitude < distance)
                {
                    if (Vector3.Angle(lookDir, direction) > angle) continue;
                    results.Add(new CharactersWithinRange()
                    {
                        character = character,
                        distance = distance,
                        angle = Vector3.Angle(lookDir, direction),
                        //isNPC = BaseAI.IsEnemy(player, character)
                    });
                }
            }
            return results;
        }
    }
}
