using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPManager : IStoreListener
{
	IStoreController _controller;
	IExtensionProvider _extensions;

	public bool IsNoAds { get; private set; }
	bool _init = false;

	const string BLOCK = "ant_block";
	const string MONEY = "ant_money";
	const string NO_ADS = "ant_noads";

	Action<Product, PurchaseFailureReason> _onPurchased;

	public void Init()
	{
		if (_init)
			return;

		Debug.Log("Init IAPManager");

		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		// 제품을 여기서 등록한다
		builder.AddProduct(id: BLOCK, ProductType.Consumable, new IDs()
		{
			{ BLOCK, AppleAppStore.Name },
			{ BLOCK, GooglePlay.Name }
		});

		builder.AddProduct(id: MONEY, ProductType.Consumable, new IDs()
		{
			{ MONEY, AppleAppStore.Name },
			{ MONEY, GooglePlay.Name }
		});

		builder.AddProduct(id: NO_ADS, ProductType.NonConsumable, new IDs() 
		{
			{ NO_ADS, AppleAppStore.Name },
			{ NO_ADS, GooglePlay.Name }
		});

		UnityPurchasing.Initialize(this, builder);
	}

	public void Purchase(string productID, Action<Product, PurchaseFailureReason> onPurchased)
	{
		if (_init == false)
			return;

		_onPurchased = onPurchased;

		try
		{
			Product product = _controller.products.WithID(productID);

			if (product != null && product.availableToPurchase)
			{
				Debug.Log($"IAPManager Purchase OK : {productID}");
				_controller.InitiatePurchase(product);
			}
			else
			{
				Debug.Log($"IAPManager Purchase FAIL : {productID}");
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex);
		}
	}

	public bool HadPurchased(string productID)
	{
		if (_init == false)
			return false;

		var product = _controller.products.WithID(productID);

		if (product != null)
			return product.hasReceipt;

		return false;
	}

	#region IStoreListener 인터페이스 구현

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		Debug.Log("IAPManager OnInitialized");
		_controller = controller;
		_extensions = extensions;

		_init = true;

		IsNoAds = HadPurchased("ant_noads");
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.LogError($"IAPManager OnInitializedFailed : {error}");
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
		var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
		try
		{
			// On Google Play, result has a single product ID.
			// On Apple stores, receipts contain multiple products.
			var result = validator.Validate(args.purchasedProduct.receipt);

			Debug.Log("IAPManager Valid Receipt");
			foreach (IPurchaseReceipt productReceipt in result)
			{
				Debug.Log(productReceipt.productID);
				Debug.Log(productReceipt.purchaseDate);
				Debug.Log(productReceipt.transactionID);

				GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
				if (null != google)
				{
					// This is Google's Order ID.
					// Note that it is null when testing in the sandbox
					// because Google's sandbox does not provide Order IDs.
					Debug.Log(google.orderID);
					Debug.Log(google.purchaseState);
					Debug.Log(google.purchaseToken);
					ProceedGoogle(args.purchasedProduct, google);
				}

				AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
				if (null != apple)
				{
					Debug.Log(apple.originalTransactionIdentifier);
					Debug.Log(apple.subscriptionExpirationDate);
					Debug.Log(apple.cancellationDate);
					Debug.Log(apple.quantity);
					ProceedApple(args.purchasedProduct, apple);
				}
			}

			return PurchaseProcessingResult.Complete;
		}
		catch (IAPSecurityException ex)
		{
			Debug.Log($"IAPManager Invalid Receipt {ex}");
		}
#endif

		return PurchaseProcessingResult.Pending;
	}

	public void ProceedGoogle(Product product, GooglePlayReceipt google)
	{
		Debug.Log($"IAPManager ProceedGoogle : {product.definition.id}");

		if (product.definition.id == NO_ADS)
			IsNoAds = true;

		_onPurchased?.Invoke(product, PurchaseFailureReason.Unknown);
	}

	public void ProceedApple(Product product, AppleInAppPurchaseReceipt apple)
	{
		Debug.Log($"IAPManager ProceedApple : {product.definition.id}");
		if (product.definition.id == NO_ADS)
			IsNoAds = true;

		_onPurchased?.Invoke(product, PurchaseFailureReason.Unknown);
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.LogWarning($"IAPManager OnPurchaseFailed : {product.definition.id}, {failureReason}");

		if (failureReason == PurchaseFailureReason.DuplicateTransaction)
		{
			if (product.definition.id == NO_ADS)
				IsNoAds = true;
		}

		_onPurchased?.Invoke(product, failureReason);
	}

	#endregion
}
