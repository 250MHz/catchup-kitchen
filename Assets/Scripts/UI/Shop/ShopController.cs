using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    private enum ShopState
    {
        Browsing,
        Dialog,
        UsingCountSelector,
        ConfirmPurchase,
        Busy,
    };

    public event Action OnStart;
    public event Action OnFinish;

    [SerializeField] private ShopUI shopUI;
    [SerializeField] private CountSelectorUI countSelectorUI;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private float cameraMoveXOffset;

    private IngredientShop ingredientShop;
    private ShopState state;
    private DialogManager dialogManager;

    public void StartShopping(IngredientShop ingredientShop)
    {
        this.ingredientShop = ingredientShop;
        OnStart?.Invoke();
        StartCoroutine(cameraController.MoveCameraToPosition(
            cameraController.transform.position
                + new Vector3(cameraMoveXOffset, 0f, 0f),
            0.2f
        ));
        StartInitialState();
    }

    private void StartInitialState()
    {
        state = ShopState.Browsing;
        shopUI.Show(ingredientShop.AvailableItems);
    }


    public void OnMove(Vector2 moveAmount)
    {
        if (state == ShopState.Browsing)
        {
            shopUI.OnMove(moveAmount);
        }
        else if (state == ShopState.UsingCountSelector)
        {
            countSelectorUI.OnMove(moveAmount);
        }
        else if (state == ShopState.ConfirmPurchase)
        {
            if (dialogManager == null)
            {
                Debug.LogError("Dialog Manager should not be null when state == ShopState.ConfirmPurchase");
            }
            dialogManager.OnMove(moveAmount);
        }
    }

    public void OnUse(Player player)
    {
        // Return early when still typing
        dialogManager = player.GetDialogManager();
        if (dialogManager.GetIsTyping())
        {
            return;
        }

        if (state == ShopState.Browsing)
        {
            ShowCountSelector(shopUI.GetSelectedItem());
        }
        else if (state == ShopState.UsingCountSelector)
        {
            // Ask to confirm the selected count
            int totalPrice = countSelectorUI.GetTotalPrice();
            dialogManager.ShowDialogText(
                $"{shopUI.GetSelectedItem().GetObjectName()}, and you want "
                + $"{countSelectorUI.GetCurrentCount()}.\nThat will be "
                + $"${totalPrice}. OK?",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (choiceIndex) =>
                {
                    if (choiceIndex == 0)
                    {
                        // Yes
                        ingredientShop.HandlePurchase(
                            player,
                            shopUI.GetSelectedItem(),
                            countSelectorUI.GetCurrentCount()
                        );
                        Wallet.Instance.TakeMoney(totalPrice);
                    }
                    state = ShopState.Browsing;
                },
                onChoiceCancel: (choiceIndex) =>
                {
                    state = ShopState.Browsing;
                }
            );
            countSelectorUI.Close();
            state = ShopState.ConfirmPurchase;
        }
        else if (state == ShopState.ConfirmPurchase)
        {
            dialogManager.OnUse();
            dialogManager.CloseDialog();
        }
    }

    public void OnCancel(Player player)
    {
        if (state == ShopState.Browsing)
        {
            shopUI.Close();
            StartCoroutine(cameraController.MoveCameraToPosition(
                cameraController.transform.position
                    - new Vector3(cameraMoveXOffset, 0f, 0f),
                0.2f,
                newFollowingPlayer: true
            ));
            OnFinish?.Invoke();
            player.GetDialogManager().ShowDialogText(
                "Please come again!",
                invokeOnShowDialog: true,
                invokeOnCloseDialog: true
            );
        }
        else if (state == ShopState.UsingCountSelector)
        {
            dialogManager.OnCancel();
            countSelectorUI.Close();
            state = ShopState.Browsing;
        }
        else if (state == ShopState.ConfirmPurchase) {
            dialogManager.OnCancel();
            dialogManager.CloseDialog();
            state = ShopState.Browsing;
        }
    }

    private void ShowCountSelector(UsableObjectSO item)
    {
        state = ShopState.UsingCountSelector;

        dialogManager.ShowDialogText(
            $"{item.GetObjectName()}? Certainly.\nHow many would you like?"
        );

        countSelectorUI.ShowSelector(item.GetPrice());
    }
}