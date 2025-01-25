using UnityEngine;


public abstract class Singleton<T> : StaticReference<T> where T : MonoBehaviour
{
    protected override void BaseAwake(T tInstance)
    {
        if (instance != null && instance != tInstance)
        {
            tInstance.gameObject.SetActive(false);
            Destroy(tInstance.gameObject);
        }
        else
        {
            instance = tInstance;
            DontDestroyOnLoad(instance);
        }

    }

    protected override void BaseOnDestroy()
    {
        // d0 nothing
        if (instance != this)
        {
            // effect of singleton which is destroying the non singleton instance
        }
        else
        {
            //TODO
        }
    }

    //TODO: create method to actually destroy the singleton (rare occasion but still a possible case)
}