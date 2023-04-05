using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// Addressable libraries
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Beamable.Examples.Services.ContentService;
using Beamable.Common.Inventory;
using Beamable.Common.Api.Inventory;

namespace Beamable.Examples.Services.InventoryService.InventoryCurrencyExample
{
    public class InventoryItemUI : MonoBehaviour
    {
        // Unity fields for References
        [SerializeField] private Image imgButton;
        [SerializeField] private TMP_Text txtAmount;
        [SerializeField] private TMP_Text txtBoostSeconds;
        


        // private functions to locally save some data to put in the UI
        protected string title;
        protected string description;
        protected Sprite sprite;
        protected int amount;
        protected int boostSeconds;
        Image buttonColor;

        protected string itemId;
        private string itemContentId;
        public GameObject chooseButtonPrefab;
        public GameObject chooseGenericPrefab;
        private const string presetAmount = "X";
        private void Start()
        {

        }
        //Get image 
        public Image GetImgButton()
        {
            return imgButton;
        }

        //Call instance when an specify item is opened and show this item-data.
        public async virtual void OpenItemUI()
        {
          
           
            if (itemContentId == BeamableReferenceConstants.refBuildingBoosterLink)
            {
                GameObject chooseBuilding = Instantiate(chooseButtonPrefab,BuildingFunctionalityReferences.Instance.InventoryUI.transform);
                chooseBuilding.transform.localPosition = new Vector3(0, 0, 0);
                chooseBuilding.GetComponentInChildren<SpawnChoosingButtons>().id = this.itemId;
                chooseBuilding.GetComponentInChildren<SpawnChoosingButtons>().selectedAmount = amount;

            }
            else if(itemContentId == BeamableReferenceConstants.refGenericBoosterLink)
            {
                GameObject chooseBuilding = Instantiate(chooseGenericPrefab, BuildingFunctionalityReferences.Instance.InventoryUI.transform);
                chooseBuilding.transform.localPosition = new Vector3(0, 0, 0);
                chooseBuilding.GetComponentInChildren<spawnGenericBoosterButtons>().id = this.itemId;
                chooseBuilding.GetComponentInChildren<spawnGenericBoosterButtons>().selectedAmount = amount;
            }
            else
            {
                InventoryManager.Instance.OpenItemUI(title, description, sprite, amount, itemId);
            }

            
        }

        // Functions sets values from API to the item
        public void SetValues(InventoryObject<InventoryItems> inventoryItem, int selectedSprite) //(string title, string description, string image, int amount, string itemId,string itemContentId, int boostSeconds)
        {
            this.title = inventoryItem.ItemContent.title;
            this.description = inventoryItem.ItemContent.description;
            string image = inventoryItem.ItemContent.spriteAddress[selectedSprite];
            this.amount = int.Parse(inventoryItem.Properties[PropertyConstants.propertyItemAmount]);
            int level = int.Parse(inventoryItem.Properties[PropertyConstants.propertyLevel]);
            this.itemId = inventoryItem.Id.ToString();
            this.boostSeconds = inventoryItem.ItemContent.itemStats[level - 1].boostTimeForSeconds;
            this.itemContentId = inventoryItem.ItemContent.Id;
            DisplayTime(this.boostSeconds);
            txtAmount.text = presetAmount + amount;
            
            Addressables.LoadAssetAsync<Sprite>(image).Completed += SpriteLoaded;
            

        }

        public void SetValues(InventoryObject<ShieldData> shopItem)
        {
            this.title = shopItem.ItemContent.title;
            this.description = shopItem.ItemContent.description;
            string image = shopItem.ItemContent.spriteAddress;
            this.amount = int.Parse(shopItem.Properties[PropertyConstants.propertyItemAmount]);
            int level = 0;// int.Parse(shopItem.Properties[PropertyConstants.propertyLevel]);
            this.itemId = shopItem.Id.ToString();
            this.boostSeconds = (int)shopItem.ItemContent.shieldDuration.GetTimeSpan().TotalSeconds;
            this.itemContentId = shopItem.ItemContent.Id;
            DisplayTime(this.boostSeconds);
            txtAmount.text = presetAmount + amount;

            Addressables.LoadAssetAsync<Sprite>(image).Completed += SpriteLoaded;
        }

        void DisplayTime(int timeToDisplay)
        {
            timeToDisplay += 1;

            int minutes = Mathf.FloorToInt(timeToDisplay / 60);
            int hours = 0;
            if (minutes >= 60)
            {
                hours = Mathf.FloorToInt(minutes / 60);
                minutes -= hours * 60;
            }


            txtBoostSeconds.text = string.Format("{0:00}h:{1:00}m", hours, minutes);
        }
        // Function to load sprite correctly to the right image space
        private void SpriteLoaded(AsyncOperationHandle<Sprite> obj)
        {
            switch (obj.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    imgButton.sprite = obj.Result;
                    sprite = obj.Result;
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
