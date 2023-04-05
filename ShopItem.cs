using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// Addressable libraries
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Beamable.Examples.Services.InventoryService.InventoryCurrencyExample
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtAmount;
        [SerializeField] private Image imageItem;

        // private functions to locally save some data to put in the UI
        protected string title;
        protected string description;
        protected Sprite sprite;
        protected int amount;
        protected string itemId;
        protected string itemContentId;

        //Call instance when an specify item is opened and show this item-data.
        public void OpenItemUI()
        {
                InventoryManager.Instance.OpenShopItemUI(title, description, sprite, amount, itemId, itemContentId);            
        }

        // Functions sets values from API to the item
        public void SetValues(string title, string description, string image, int amount, string itemId, string itemContentId)
        {
            this.title = title;
            this.description = description;
            this.amount = amount;
            this.itemId = itemId;
            this.itemContentId = itemContentId;
            txtAmount.text = $"X{amount}";
            Addressables.LoadAssetAsync<Sprite>(image).Completed += SpriteLoaded;
        }

        // Function to load sprite correctly to the right image space
        private void SpriteLoaded(AsyncOperationHandle<Sprite> obj)
        {
            switch (obj.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    sprite = obj.Result;
                    imageItem.sprite = sprite;
                    break;
                case AsyncOperationStatus.Failed:
                    Debug.LogError("Sprite load failed");
                    break;
                default:
                    //case AsyncOperationStatus.None:
                    break;
            }
        }
    }
}
