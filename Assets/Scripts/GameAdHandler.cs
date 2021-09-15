using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GameAdHandler : MonoBehaviour
{
    private float TIME_THRESHOLD = 2.5f * 60; //2.5 Min in seconds

    private InterstitialAd interstitial;
    private BannerView bannerView;
    private Delegates.ObjectDelegate objectDelegate;
    private float lastAdShowSeconds;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        RequestBanner();
        RequestInterstitial();
    }

    #region Requests

    private void RequestBanner()
    {
        String adUnitId = "unexpected_platform";
        if (Constants.CURRENT_PLATFORM == Constants.PLATFORM_ANDROID)
        {
            adUnitId = Constants.DEVELOPMENT_BANNER_ID_ANDROID;
            //adUnitId = Constants.LIVE_BANNER_ID_ANDROID;
        }
        else if (Constants.CURRENT_PLATFORM == Constants.PLATFORM_IOS)
        {
            adUnitId = Constants.DEVELOPMENT_BANNER_ID_ANDROID;
        }

        // Create a 320x50 banner at the top of the screen.
        //AdSize adSize = new AdSize(728,90);
        AdSize adSize = AdSize.Banner;

        //adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(0);
        this.bannerView = new BannerView(adUnitId, adSize, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnBannerAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnBannerAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnBannerAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnBannerAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        // Create an ad request.
        AdRequest request = new AdRequest.Builder()
              //.AddTestDevice(AdRequest.TestDeviceSimulator)
              //.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
              .AddKeyword("game")
              //.SetGender(Gender.Male)
              //.SetBirthday(new DateTime(1985, 1, 1))
              .TagForChildDirectedTreatment(true)
              .AddExtra("color_bg", "23162E")
              .Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);

        //Hide banner
        HideBannerAd();
    }

    private void RequestInterstitial()
    {
        String adUnitId = "unexpected_platform";
        if (Constants.CURRENT_PLATFORM == Constants.PLATFORM_ANDROID)
        {
            adUnitId = Constants.DEVELOPMENT_INTERSTITIAL_ID_ANDROID;
            //adUnitId = Constants.LIVE_INTERSTITIAL_ID_ANDROID;
        }
        else if (Constants.CURRENT_PLATFORM == Constants.PLATFORM_IOS)
        {
            adUnitId = Constants.DEVELOPMENT_INTERSTITIAL_ID_IOS;
        }

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an ad request.
        AdRequest request = new AdRequest.Builder()
              //.AddTestDevice(AdRequest.TestDeviceSimulator)
              //.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
              .AddKeyword("game")
              //.SetGender(Gender.Male)
              //.SetBirthday(new DateTime(1985, 1, 1))
              .TagForChildDirectedTreatment(true)
              .AddExtra("color_bg", "23162E")
              .Build();

        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    #endregion


    #region Handlers BANNER

    public void HandleOnBannerAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleOnBannerAdLoaded event received");
    }

    public void HandleOnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleOnBannerAdFailedToLoad event received with message: " + args.Message);
    }

    public void HandleOnBannerAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleOnBannerAdOpened event received");
    }

    public void HandleOnBannerAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleOnBannerAdClosed event received");
        DestroyBannerAd();
    }

    public void HandleOnBannerAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    #endregion


    #region Handlers INTERSTITIAL

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
        objectDelegate?.Invoke(false);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        lastAdShowSeconds = Time.timeSinceLevelLoad;
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        objectDelegate?.Invoke(false);
        DestroyInterstitialAd();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    #endregion


    #region Show 

    public void ShowBannerAd()
    {
        this.bannerView.Show();
    }

    public void ShowInterstitialAd(Delegates.ObjectDelegate receivedObjectDelegate)
    {
        objectDelegate = receivedObjectDelegate;
        if (this.interstitial.IsLoaded() && lastAdShowSeconds + TIME_THRESHOLD < Time.timeSinceLevelLoad)
        {
            this.interstitial.Show();
        }
        else
        {
            objectDelegate?.Invoke(false);
        }
    }

    #endregion

    #region Hide 

    public void HideBannerAd()
    {
            this.bannerView.Hide();
    }

    #endregion


    #region Destroy

    public void DestroyBannerAd()
    {
        bannerView.Destroy();
    }

    public void DestroyInterstitialAd()
    {
        interstitial.Destroy();
    }

    #endregion


}
