using UnityEngine;

public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T[] assets = Resources.LoadAll<T>("Settings");
                
                if (assets == null || assets.Length < 1)
                {
                    throw new System.Exception("Could not find any ScriptableSingleton instances in the resources");
                }

                instance = assets[0];
            }
            return instance;
        }
    }
}