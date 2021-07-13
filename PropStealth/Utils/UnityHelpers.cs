using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PropStealth
{
    public static class EventTriggerExtensions
    {
        public static void AddListener(this EventTrigger trigger,
                                            EventTriggerType eventType,
                                            System.Action<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
            trigger.triggers.Add(entry);
        }
    }

    public static class UnityHelpers
    {
        public static T AssureComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component != null) return component;
            return gameObject.AddComponent<T>();
            
        }

        public static int DepthChecked = 0;
        public static Transform RecursiveFind(this Transform transform, string search, bool caseInsensitive = true)
        {
            return CheckChildren(transform, search, caseInsensitive);
        }

        private static Transform CheckChildren(this Transform transform, string search, bool caseInsensitive = true)
        {
            Transform transformFound = null;
            if (IsEqual(transform.name, search, caseInsensitive))
            {
                transformFound = transform;
            }

            if (transform.childCount > 0 && transformFound == null)
            {
                DepthChecked++;
                foreach (Transform child in transform)
                {
                    if (transformFound != null) continue;
                    transformFound = CheckChildren(child, search, caseInsensitive);
                }
            }
            return transformFound;
        }

        public static bool IsEqual(string name, string otherName, bool caseInsensitive = true)
        {
            return caseInsensitive ? name.ToLower().Equals(otherName.ToLower()) : name.Equals(otherName);
        }

        public static T GetChildComponentByName<T>(this GameObject objected, string name, bool caseSensitive = true) where T : Component
        {
            foreach (T component in objected.GetComponentsInChildren<T>(true))
            {
                bool comparison = false;
                if (!caseSensitive)
                    comparison = component.gameObject.name.ToLower() == name.ToLower();
                else
                    comparison = component.gameObject.name == name;
                if (comparison)
                    return component;
            }
            return null;
        }
    }
}
