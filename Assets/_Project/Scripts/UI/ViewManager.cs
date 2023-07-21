using System;
using System.Collections.Generic;

public class ViewManager : Singleton<ViewManager>
{
    private Dictionary<Type, ViewController> viewManagers;
    private Stack<ViewController> viewControllerStack;

    public bool HasAnyViewOpened()
    {
        bool open = false;

        foreach (var item in viewManagers.Values)
        {
            if (item.IsViewOpenned())
            {
                //UnityEngine.Debug.Log("This view is open: " + item.name);
                open = true;

                break;
            }
        }

        return open;
    }

    public bool HasAnyPopUpOpen()
    {
        foreach (var item in viewManagers.Values)
        {
            if (item.isPopup && item.IsViewOpenned())
            {
                return true;
            }
        }

        return false;
    }

    public void ClearViewManager()
    {
        foreach (var item in viewManagers.Values)
        {
            Destroy(item.gameObject);
        }

        viewManagers = new Dictionary<Type, ViewController>();
        viewControllerStack = new Stack<ViewController>();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        viewManagers = new Dictionary<Type, ViewController>();
        viewControllerStack = new Stack<ViewController>();
    }

    /// <summary>
    /// Subscribe the View to the View Manager
    /// </summary>
    /// <param name="viewController"></param>
    public void SubscribeViewControler(ViewController viewController)
    {
        if (viewManagers.ContainsKey(viewController.GetType()))
            return;

        viewManagers.Add(viewController.GetType(), viewController);
    }

    /// <summary>
    /// Unsubscribe the View to the View Manager
    /// </summary>
    /// <param name="viewController"></param>
    public void UnSubscribeViewController(ViewController viewController)
    {
        viewManagers.Remove(viewController.GetType());
    }

    /// <summary>
    /// Get a Subscribed View Controller
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public ViewController GetViewController(Type type)
    {
        ViewController viewController;
        viewManagers.TryGetValue(type, out viewController);

        if (viewController)
            return viewController;
        else
            return null;
    }

    public bool IsViewControllerOppened(Type type)
    {
        ViewController viewController;
        viewManagers.TryGetValue(type, out viewController);

        if (viewController)
            return viewController.IsViewOpenned();
        else
            return false;
    }

    /// <summary>
    /// Open the view controller
    /// </summary>
    /// <param name="type"></param>
    /// <param name="param"></param>
    /// <param name="animated"></param>
    public void OpenViewController(Type type, object param = null, bool animated = true)
    {
        ViewController viewController;
        viewManagers.TryGetValue(type, out viewController);

        if (viewController)
        {
            PushViewController(type, param);
            PopViewController();
        }
    }

    /// <summary>
    /// Close the View Controller
    /// </summary>
    /// <param name="type"></param>
    /// <param name="animated"></param>
    public void CloseViewController(Type type, bool animated = true, System.Action callback = null)
    {
        ViewController viewController;
        viewManagers.TryGetValue(type, out viewController);

        if (viewController)
        {
            viewController.CloseView(animated, callback);
        }
    }

    /// <summary>
    /// Add the View Controller to the stack navigation
    /// </summary>
    /// <param name="type"></param>
    /// <param name="param"></param>
    /// <param name="animated"></param>
    private void PushViewController(Type type, object param = null)
    {
        ViewController viewController;
        viewManagers.TryGetValue(type, out viewController);

        if (viewController)
        {
            viewControllerStack.Push(viewController);
        }
    }

    /// <summary>
    /// Open the the stacked View Contorller 
    /// </summary>
    /// <param name="animated"></param>
    private void PopViewController()
    {
        if (viewControllerStack.Count > 0)
        {
            var view = viewControllerStack.Pop();
            view.OpenView();
        }
    }

    /// <summary>
    /// Open the the  View Contorller instant
    /// </summary>
    /// <param name="animated"></param>
    public void PopViewController(Type type, object param = null)
    {
        ViewController viewController;
        viewManagers.TryGetValue(type, out viewController);

        if (viewController)
        {
            viewController.OpenView();
        }
    }

    public List<ViewController> GetSubscribedViewControllers()
    {
        List<ViewController> viewNames = new List<ViewController>();

        foreach (var view in viewManagers)
        {
            viewNames.Add(view.Value);
        }

        return viewNames;
    }

    private void Update()
    {
        if (!HasAnyViewOpened() && viewControllerStack.Count > 0)
        {
            PopViewController();
        }
    }
}
