using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager
{
	public enum AdsStateType
	{
		None,
		Failed,
		Success
	}

	bool TEST_MODE = true;

	InterstitialAd _interstitialAd;
	RewardedAd _rewardedAd;
	Action _rewardedCallback;

	public void Init()
	{
		MobileAds.Initialize(initStatus => { });
		PrepareAds();
	}

	// 실제 출시하기 전에 테스트로 사용하는 ID들.
	// 출시 전에 실제 ID 박으면 정지 사유가 되니 조심하자.
	const string TEST_APP_ID = "ca-app-pub-3940256099942544~3347511713";
	const string TEST_ANDROID_INTERSTITIAL = "ca-app-pub-3940256099942544/1033173712";
	const string TEST_ANDROID_REWARDED = "ca-app-pub-3940256099942544/5224354917";
	const string TEST_IOS_INTERSTITIAL = "ca-app-pub-3940256099942544/4411468910";
	const string TEST_IOS_REWARDED = "ca-app-pub-3940256099942544/1712485313";

	// 실제 출시용 ID들
	const string ANDROID_APP_ID = "";
	const string IOS_APP_ID = "";

	public void PrepareAds()
	{
#if UNITY_ANDROID
		string interstitial = "ca-app-pub-4385483914896399/1780279477"; // Android_Interstitial
		string rewarded = ""; // Android_Rewarded
		string interstitialTest = TEST_ANDROID_INTERSTITIAL;
		string rewardedTest = TEST_ANDROID_REWARDED;
#else
        string interstitial = ""; // IOS_Interstitial
        string rewarded = ""; // IOS_Rewarded
		string interstitialTest = TEST_IOS_INTERSTITIAL;
		string rewardedTest = TEST_IOS_REWARDED;
#endif

		_interstitialAd = new InterstitialAd(TEST_MODE ? interstitialTest : interstitial);
		_interstitialAd.OnAdLoaded += HandleOnAdLoaded;
		_interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		_interstitialAd.OnAdOpening += HandleOnAdOpened;
		_interstitialAd.OnAdClosed += HandleOnAdClosed;
		_interstitialAd.LoadAd(new AdRequest.Builder().Build());

		_rewardedAd = new RewardedAd(TEST_MODE ? rewardedTest : rewarded);
		_rewardedAd.OnAdLoaded += HandleOnAdLoaded;
		_rewardedAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		_rewardedAd.OnAdOpening += HandleOnAdOpened;
		_rewardedAd.OnAdClosed += HandleOnAdClosed;
		_rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		_rewardedAd.LoadAd(new AdRequest.Builder().Build());
	}

	public void HandleOnAdLoaded(object sender, EventArgs args)
	{
		Debug.Log("HandleAdLoaded");
	}

	public void HandleOnAdFailedToLoad(object sender, EventArgs args)
	{
		Debug.Log($"HandleFailedToReceiveAd : {args}");
	}

	public void HandleOnAdOpened(object sender, EventArgs args)
	{
		Debug.Log("HandleAdOpened");
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		Debug.Log("HandleAdClosed");
		PrepareAds();
	}

	// Rewarded 광고 보상 처리
	public void HandleUserEarnedReward(object sender, EventArgs args)
	{
		Debug.Log("HandleUserEarnedReward");
		_rewardedCallback?.Invoke();
		_rewardedCallback = null;
	}

	public void ShowInterstitialAds()
	{
		if (_interstitialAd.IsLoaded())
			_interstitialAd.Show();
		else
			PrepareAds();
	}

	public void ShowRewardedAds(Action rewardedCallback)
	{
		_rewardedCallback = rewardedCallback;

		if (_rewardedAd.IsLoaded())
			_rewardedAd.Show();
		else
			PrepareAds();
	}
}
