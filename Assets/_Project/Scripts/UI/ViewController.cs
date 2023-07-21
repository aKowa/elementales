using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum UITweenDirection
{
    Left,
    Right,
    Up,
    Down,
    Center
}

public class ViewController : MonoBehaviour
{
    [SerializeField]
    protected GameObject viewContent;

    [Header("Tween")]
    [SerializeField]
    UITweenDirection tweenDirection;

    [SerializeField]
    protected Ease openTweenType;
    [SerializeField]
    protected Ease closeTweenType;
    [SerializeField]
    protected float closeTweenTime = 0.3f;
    [SerializeField]
    protected float openTweenTime = 0.2f;
    [SerializeField]
    protected float tweenOfsset;
    [SerializeField]
    protected bool animated = true;

    public bool isPopup;
    public bool ignorarTimeScale;
    protected GameObject fader;
    protected CanvasGroup faderGroup;
    private bool isFaderWithBackground = false;
    private Vector2 initialPosition;
    private bool isViewOpened = false;

    private RectTransform viewRectTransform;
    private Vector3 initialScale;

    private bool dontEnableCameraMovementOnClose;

    public Action onOpen;
    public Action onLateOpen;

    protected CanvasGroup canvasGroup;
    private bool isTweenActive;

    private void Awake()
    {
        ViewManager.GetInstance().SubscribeViewControler(this);
        viewRectTransform = viewContent.gameObject.GetComponent<RectTransform>();
        initialPosition = viewRectTransform.localPosition;
        initialScale = viewRectTransform.localScale;
        Close();

        if (transform.Find("Fader"))
        {
            fader = transform.Find("Fader").gameObject;
        }
        else if (transform.Find("FaderBack"))
        {
            fader = transform.Find("FaderBack").gameObject;
            isFaderWithBackground = true;
        }

        if (fader)
        {
            faderGroup = fader.GetComponent<CanvasGroup>();
        }

        canvasGroup = viewContent.GetComponent<CanvasGroup>();
        isTweenActive = false;

        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    public bool IsViewOpenned()
    {
        return isViewOpened;
    }

#region Events

    /// <summary>
    /// Action executed after the Start Action
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// Action executed after the Awake Action
    /// </summary>
    protected virtual void OnAwake() { }

    /// <summary>
    /// Action executed after the view was opened
    /// </summary>
    public virtual void OnOpen() { }

    /// <summary>
    /// Action executed after the view was closed
    /// </summary>
    protected virtual void OnClose() { }

    /// <summary>
    /// Action executed before the Open animation
    /// </summary>
    protected virtual void OnStartOpenAnimation() { }

    /// <summary>
    /// Action executed after the Open animation
    /// </summary>
    protected virtual void OnAnimationEnd() { }

    /// <summary>
    /// Action executed before the Close animation
    /// </summary>
    protected virtual void OnStartCloseAnimation() { }

#endregion

    private void OnDestroy()
    {
        ViewManager.GetInstance().UnSubscribeViewController(this);
    }

    private void TweenStarted()
    {
        canvasGroup.blocksRaycasts = false;
        isTweenActive = true;
    }

    private void TweenEnded()
    {
        canvasGroup.blocksRaycasts = true;
        isTweenActive = false;
    }

    /// <summary>
    /// Call OpenView or CloseView, depending on the View state. Gets ignored if a tween is active.
    /// </summary>
    public void OpenOrCloseView()
    {
        if(isTweenActive == true)
        {
            return;
        }

        if(isViewOpened == false)
        {
            OpenView();
        }
        else
        {
            CloseView();
        }
    }

    /// <summary>
    /// Open the View and pass a optional parameter 
    /// </summary>
    /// <param name="param">If </param>
    /// <param name="animated"></param>
    public virtual void OpenView()
    {
        if (isViewOpened || viewContent == null)
            return;

        FadeIn();

        isViewOpened = true;

        if (!animated)
        {
            viewContent.gameObject.SetActive(true);
            viewRectTransform.localPosition = initialPosition;
            viewRectTransform.localScale = initialScale;
            onOpen?.Invoke();
            OnOpen();
            onLateOpen?.Invoke();
        }
        else
        {
            if (isPopup)
            {
                viewRectTransform.localScale = new Vector3(0, 0, 0);
            }

            viewContent.gameObject.SetActive(true);
            Vector2 tweenPosition = GetTweenPosition();
            viewRectTransform.localPosition = tweenPosition;

            TweenStarted();

            var seq = DOTween.Sequence();
            seq.SetUpdate(ignorarTimeScale);

            seq.AppendCallback(() => { OnStartOpenAnimation(); });
            seq.AppendCallback(() =>
            {
                // fire event after tween
                onOpen?.Invoke();
                OnOpen();
            });

            if (isPopup)
            {
                seq.AppendCallback(() => { viewRectTransform.localScale = new Vector3(0, 0, 0); });
                seq.Append(viewRectTransform.DOScale(initialScale, openTweenTime).SetEase(openTweenType));
            }
            else
            {
                seq.Append(viewRectTransform.DOLocalMove(initialPosition, openTweenTime).SetEase(openTweenType));
            }
            
            seq.AppendCallback(() => { OnAnimationEnd(); });

            seq.AppendCallback(() => { TweenEnded(); });
            seq.AppendCallback(() => onLateOpen?.Invoke());
        }
    }

    /// <summary>
    /// Close the View and execute OnClose Action
    /// </summary>
    /// <param name="animated"></param>
    public virtual void CloseView(bool animated = true, System.Action callback = null)
    {
        if (viewContent == null || !isViewOpened)
            return;

        if (!animated)
        {
            OnClose();
            Close();
            FadeOut();
        }
        else
        {
            EnableContentButtons(viewContent.transform, false);
            Vector2 tweenPosition = GetTweenPosition();

            TweenStarted();

            var seq = DOTween.Sequence();
            seq.SetUpdate(ignorarTimeScale);

            seq.AppendCallback(() => { OnStartCloseAnimation(); });

            if (isPopup)
            {
                seq.Append(viewRectTransform.DOScale(new Vector3(initialScale.x + 0.1f, initialScale.y + 0.1f, initialScale.z), 0.2f).SetEase(closeTweenType));
                seq.Append(viewRectTransform.DOScale(new Vector3(0, 0, 0), closeTweenTime).SetEase(closeTweenType));
            }
            else
            {
                seq.Append(viewRectTransform.DOLocalMove(tweenPosition, closeTweenTime + 0.1f).SetEase(closeTweenType));
            }

            seq.AppendCallback(() =>
            {
                // fire event after tween
                FadeOut();
                OnClose();
                callback?.Invoke();
                isViewOpened = false;
                viewContent.gameObject.SetActive(false);
                EnableContentButtons(viewContent.transform, true);
            });

            seq.AppendCallback(() => { TweenEnded(); });
        }
    }

    public virtual void CloseView()
    {
        if (viewContent == null || !isViewOpened)
            return;

        if (!animated)
        {
            OnClose();
            Close();
            FadeOut();
        }
        else
        {
            EnableContentButtons(viewContent.transform, false);
            Vector2 tweenPosition = GetTweenPosition();

            TweenStarted();

            var seq = DOTween.Sequence();
            seq.SetUpdate(ignorarTimeScale);

            seq.AppendCallback(() => { OnStartCloseAnimation(); });

            if (isPopup)
            {
                seq.Append(viewRectTransform.DOScale(new Vector3(initialScale.x + 0.1f, initialScale.y + 0.1f, initialScale.z), 0.2f).SetEase(closeTweenType));
                seq.Append(viewRectTransform.DOScale(new Vector3(0, 0, 0), closeTweenTime).SetEase(closeTweenType));
            }
            else
            {
                seq.Append(viewRectTransform.DOLocalMove(tweenPosition, closeTweenTime + 0.1f).SetEase(closeTweenType));
            }

            seq.AppendCallback(() =>
            {
                // fire event after tween
                FadeOut();
                OnClose();
                isViewOpened = false;
                viewContent.gameObject.SetActive(false);
                EnableContentButtons(viewContent.transform, true);

                viewRectTransform.localPosition = initialPosition;
                viewRectTransform.localScale = initialScale;
            });

            seq.AppendCallback(() => { TweenEnded(); });
        }
    }

    /// <summary>
    /// Close the ViewController
    /// </summary>
    private void Close()
    {
        isViewOpened = false;
        viewContent.gameObject.SetActive(false);

        viewRectTransform.localPosition = initialPosition;
        viewRectTransform.localScale = initialScale;
    }

    void FadeIn()
    {
        if (fader)
        {
            fader.SetActive(true);
            faderGroup.alpha = 0;

            if (isFaderWithBackground)
            {
                faderGroup.DOFade(1f, .3f).SetUpdate(ignorarTimeScale);
            }
            else
            {
                faderGroup.DOFade(.5f, .3f).SetUpdate(ignorarTimeScale);
            }
        }
    }

    void FadeOut()
    {
        if (fader)
        {
            faderGroup.alpha = .5f;
            faderGroup.DOFade(0, .3f).SetUpdate(ignorarTimeScale);
            fader.SetActive(false);
        }
    }

    private void EnableContentButtons(Transform root, bool value)
    {
        foreach (Transform child in root)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
                button.enabled = value;

            if (child.childCount > 0)
                EnableContentButtons(child, value);
        }
    }

    /// <summary>
    /// Get the off screen position based in the tween direction
    /// </summary>
    /// <returns></returns>
    private Vector2 GetTweenPosition()
    {
        var camera = Camera.main;

        if (!camera)
        {
            camera = FindObjectOfType<Camera>();
        }

        Vector3 stageDimensions = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector2 rectTransforSize = viewRectTransform.sizeDelta;
        Vector2 tweenPosition = initialPosition;

        switch (tweenDirection)
        {
            case UITweenDirection.Left:
                tweenPosition.x -= (stageDimensions.x + rectTransforSize.x + tweenOfsset);

                break;

            case UITweenDirection.Right:
                tweenPosition.x += (stageDimensions.x + rectTransforSize.x + tweenOfsset);

                break;

            case UITweenDirection.Up:
                tweenPosition.y -= (stageDimensions.y + rectTransforSize.y + tweenOfsset);

                break;

            case UITweenDirection.Down:
                tweenPosition.y += (stageDimensions.y + rectTransforSize.y + tweenOfsset);

                break;

            case UITweenDirection.Center:
                break;
        }

        return tweenPosition;
    }
}
