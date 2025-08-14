using UnityEngine;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        // Wenn es ein Array ist (z. B. [{"id":1,...}]), füge einen Wrapper hinzu
        if (json.StartsWith("[") && json.EndsWith("]"))
        {
            json = "{\"items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }
        // Wenn es bereits ein Wrapper-Objekt ist, verarbeite es normal
        else
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
