using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Yodo1.MAS;

public class AdsManager : Singleton<AdsManager>
{
    [Header("Ad Placements")]
    public InputField rewardAdPlacement;

    bool enabledRewardVideoV2 = true;

    public Action OnRewardAdEarnedEvent_External;
    
    private void Start()
    {
        Yodo1U3dMasCallback.OnSdkInitializedEvent += (success, error) =>
        {
            Debug.Log(Yodo1U3dMas.TAG + "OnSdkInitializedEvent, success:" + success + ", error: " + error.ToString());
            Debug.Log(Yodo1U3dMas.TAG + "OnSdkInitializedEvent, age:" + Yodo1U3dMas.GetUserAge());
            if (success)
            {
                InitializeRewardedAds();
            }
        };

        Yodo1MasUserPrivacyConfig userPrivacyConfig = new Yodo1MasUserPrivacyConfig()
            .titleBackgroundColor(Color.green)
            .titleTextColor(Color.blue)
            .contentBackgroundColor(Color.black)
            .contentTextColor(Color.white)
            .buttonBackgroundColor(Color.red)
            .buttonTextColor(Color.green);

        Yodo1AdBuildConfig config = new Yodo1AdBuildConfig()
            .enableAdaptiveBanner(true)
            .enableUserPrivacyDialog(true)
            .userPrivacyConfig(userPrivacyConfig);
        
        Yodo1U3dMas.SetAdBuildConfig(config);

        Yodo1U3dMas.InitializeMasSdk();

        RequestRewardedAds();
    }
    
    private void RequestRewardedAds()
    {
        Yodo1U3dRewardAd.GetInstance().LoadAd();
    }
    
    private void InitializeRewardedAds()
    {
        // Instantiate
        Yodo1U3dRewardAd.GetInstance();
        
        // Ad Events
        Yodo1U3dRewardAd.GetInstance().OnAdLoadedEvent += OnRewardAdLoadedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdLoadFailedEvent += OnRewardAdLoadFailedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdOpenedEvent += OnRewardAdOpenedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdOpenFailedEvent += OnRewardAdOpenFailedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdClosedEvent += OnRewardAdClosedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdEarnedEvent += OnRewardAdEarnedEvent;
    }

    public void ShowRewardedAds()
    {
        bool isLoaded = Yodo1U3dRewardAd.GetInstance().IsLoaded();

        if(isLoaded) Yodo1U3dRewardAd.GetInstance().ShowAd();
    }
    
    public IEnumerator CheckInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }

    #region Rewarded Ads Callback Methods
    
    private void OnRewardAdLoadedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdLoadedEvent event received");
    }

    private void OnRewardAdLoadFailedEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdLoadFailedEvent event received with error: " + adError.ToString());
    }

    private void OnRewardAdOpenedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdOpenedEvent event received");
    }

    private void OnRewardAdOpenFailedEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdOpenFailedEvent event received with error: " + adError.ToString());
        // Load the next ad
        Yodo1U3dRewardAd.GetInstance().LoadAd();
    }

    private void OnRewardAdClosedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdClosedEvent event received");
        // Load the next ad
        Yodo1U3dRewardAd.GetInstance().LoadAd();
    }

    private void OnRewardAdEarnedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdEarnedEvent event received");
        OnRewardAdEarnedEvent_External?.Invoke();
        if(OnRewardAdEarnedEvent_External == null)
            Debug.LogWarning("There was no event subscribed to OnRewardAdEarnedEvent");
        OnRewardAdEarnedEvent_External = null;
        // Add your reward code here
    }    

    #endregion
}
