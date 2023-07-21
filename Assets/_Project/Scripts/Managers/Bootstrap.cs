using UnityEngine;

public class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RunBootstrap()
    {
        GameObject app;

        try
        {
            app = Object.Instantiate(Resources.Load("ManagersBase")) as GameObject;
        }
        catch (System.ArgumentException)
        {
            throw new System.ApplicationException("Nao foi possivel achar o prefab ManagersBase em uma pasta Resources!");
        }
        catch (System.Exception e)
        {
            throw new System.Exception(e.ToString());
        }

        app.name = "ManagersBase";
        Object.DontDestroyOnLoad(app);
    }
}
