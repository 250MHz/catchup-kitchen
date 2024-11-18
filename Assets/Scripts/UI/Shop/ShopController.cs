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
    };

    public event Action OnStart;
    public event Action OnFinish;

    [SerializeField] private ShopUI shopUI;
    [SerializeField] private CountSelectorUI countSelectorUI;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private float cameraMoveXOffset;
    [SerializeField] private UsableObjectSO tableSO;
    [SerializeField] private UsableObjectSO plateSO;

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
            UsableObjectSO selectedItem = shopUI.GetSelectedItem();
            if (selectedItem == tableSO)
            {
                dialogManager.ShowDialogText(
                    $"Unlocking this table will cost "
                    + $"${TableManager.Instance.GetNextTablePrice()}. OK?",
                    choices: new List<string>() { "Yes", "No" },
                    onChoiceSelected: (choiceIndex) =>
                    {
                        if (choiceIndex == 0)
                        {
                            // Yes
                            // This shop item may still show up even if all
                            // tables are bought if multiple players have the
                            // shop UI open
                            if (TableManager.Instance.HasInactiveTable())
                            {
                                // Don't store totalPrice as a variable to let players
                                // cheat the price (if two people open the menu and one
                                // pays, the other still shows $30, but if they try to
                                // buy it'll still cost $60.)
                                Wallet.Instance.TakeMoney(TableManager.Instance.GetNextTablePrice());
                                TableManager.Instance.ActivateNextTable();
                            }
                            shopUI.RemoveItemsIfPossible();
                            dialogManager.ShowDialogText("Here you are!\nThank you!");
                            state = ShopState.Dialog;
                        }
                        else
                        {
                            dialogManager.CloseDialog();
                            state = ShopState.Browsing;
                        }
                    },
                    onChoiceCancel: (choiceIndex) =>
                    {
                        state = ShopState.Browsing;
                    }
                );
                state = ShopState.ConfirmPurchase;
            }
            else if (selectedItem == plateSO)
            {
                int platePrice = plateSO.GetPrice();
                dialogManager.ShowDialogText(
                    $"Plate, and you want one.\nThat will be ${platePrice}. OK?",
                    choices: new List<string>() { "Yes", "No" },
                    onChoiceSelected: (choiceIndex) =>
                    {
                        if (choiceIndex == 0)
                        {
                            // Yes
                            // This shop item may still show up even if all
                            // plates are bought if multiple players have the
                            // shop UI open
                            if (PlateManager.Instance.TryIncreasePlateCount())
                            {
                                ingredientShop.HandleSingleItemPurchase(player, plateSO);
                                Wallet.Instance.TakeMoney(platePrice);
                            }
                            shopUI.RemoveItemsIfPossible();
                            dialogManager.ShowDialogText("Here you are!\nThank you!");
                            state = ShopState.Dialog;
                        }
                        else
                        {
                            dialogManager.CloseDialog();
                            state = ShopState.Browsing;
                        }
                    },
                    onChoiceCancel: (choiceIndex) =>
                    {
                        state = ShopState.Browsing;
                    }
                );
                state = ShopState.ConfirmPurchase;
            }
            else
            {
                ShowCountSelector(selectedItem);
            }
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
                        ingredientShop.HandleIngredientBoxPurchase(
                            player,
                            shopUI.GetSelectedItem(),
                            countSelectorUI.GetCurrentCount()
                        );
                        Wallet.Instance.TakeMoney(totalPrice);
                        dialogManager.ShowDialogText("Here you are!\nThank you!");
                        state = ShopState.Dialog;
                    }
                    else
                    {
                        dialogManager.CloseDialog();
                        state = ShopState.Browsing;
                    }
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
        }
        else if (state == ShopState.Dialog)
        {
            dialogManager.OnUse();
            dialogManager.CloseDialog();
            state = ShopState.Browsing;
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
        else if (state == ShopState.ConfirmPurchase)
        {
            dialogManager.OnCancel();
            dialogManager.CloseDialog();
            state = ShopState.Browsing;
        }
        else if (state == ShopState.Dialog)
        {
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
