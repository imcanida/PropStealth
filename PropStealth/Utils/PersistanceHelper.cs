using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PropStealth
{
    public static class PersistanceHelper
    {
        // Set on start of Mod load.
        public static string ModID;
        public static string Folder;

        public static string GetModDataPath(this PlayerProfile profile)
        {
            return Path.Combine(Utils.GetSaveDataPath(), "ModData", PersistanceHelper.ModID, "char_" + profile.GetFilename());
        }

        public static TData LoadModData<TData>(this PlayerProfile profile) where TData : new()
        {
            bool flag = !File.Exists(profile.GetModDataPath());
            TData result;
            if (flag)
            {
                result = Activator.CreateInstance<TData>();
            }
            else
            {
                string text = File.ReadAllText(profile.GetModDataPath());
                result = JsonUtility.FromJson<TData>(text);
            }
            return result;
        }

        public static void SaveModData<TData>(this PlayerProfile profile, TData data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(profile.GetModDataPath()));
            File.WriteAllText(profile.GetModDataPath(), JsonUtility.ToJson(data));
        }
    }

}