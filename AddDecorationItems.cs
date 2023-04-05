using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Examples.Services.ContentService;
using TMPro;

namespace Beamable.Examples.Services.InventoryService
{
    public class AddDecorationitems : MonoBehaviour
    {
        // Unity fields for References
        [SerializeField] private ContentLink<PremiumShopData> refPremiumShopData;
        [SerializeField] private GameObject prefabShopItem;
        [SerializeField] private Transform contentTransformShopItems;
        [SerializeField] private Transform contentTransformShopMinions;

        // Private variables to locally save the item-data
        private IBeamableAPI beamableAPI;
        private PremiumShopData premiumShopData;
        private ShopItems[] shopItems;
        private ShopMinions[] shopMinions;
        // See what tab to open
        private WitchShopTabs witchShopTabToOpen = WitchShopTabs.TabItems;

        public void SetWitchShopTabToOpen(WitchShopTabs tabToOpen)
        {
            witchShopTabToOpen = tabToOpen;
            UpdateTabItemsData();
        }

        private async void UpdateTabItemsData()
        {
            // Enable the correct content object (each content object is it's own tab).
            switch (witchShopTabToOpen)
            {
                case WitchShopTabs.TabMinions:
                    // If shopMinions not yet cached locally, get the items and instantiate them.
                    if (shopMinions == null)
                    {
                        await GetItemsData();
                        InstantiateItems();
                    }
                    break;
                // The default will also be to open the TabItems
                case WitchShopTabs.TabItems:
                default:
                    // If shopItems not yet cached locally, get the items and instantiate them.
                    if (shopItems == null)
                    {
                        await GetItemsData();
                        InstantiateItems();
                    }
                    break;
            }
        }

        // With this function we will get the data from beamable through API.
        // The data we get is based on the witchshoptab that we want to open.
        private async Task<bool> GetItemsData()
        {
            if (beamableAPI == null)
            {
                beamableAPI = await Beamable.API.Instance;
            }

            if (premiumShopData == null)
            {
                premiumShopData = (PremiumShopData)(await beamableAPI.ContentService.GetContent(refPremiumShopData));
            }

            switch (witchShopTabToOpen)
            {
                case WitchShopTabs.TabMinions:
                    shopMinions = new ShopMinions[premiumShopData.shopMinions.Length];
                    //Loop through shopMinions and put in array.
                    for (int i = 0; i < premiumShopData.shopMinions.Length; i++)
                    {
                        shopMinions[i] = (ShopMinions)(await beamableAPI.ContentService.GetContent(premiumShopData.shopMinions[i]));
                    }
                    break;
                case WitchShopTabs.TabItems:
                default:
                    shopItems = new ShopItems[premiumShopData.shopItems.Length];
                    //Loop through shopItems and put in array.
                    for (int i = 0; i < premiumShopData.shopItems.Length; i++)
                    {
                        shopItems[i] = (ShopItems)(await beamableAPI.ContentService.GetContent(premiumShopData.shopItems[i]));
                    }
                    break;
            }

            return true;
        }

        //With this function we will put the array data from before and put it in the prefab. It does this for each item in beamable content.
        private void InstantiateItems()
        {
            // Instantiate the object in the correct content object.
            prefabShopItem.SetActive(false);
            switch (witchShopTabToOpen)
            {
                // Use the minion content transform and the shopMinions array.
                case WitchShopTabs.TabMinions:
                    for (int i = 0; i < shopMinions.Length; i++)
                    {
                        GameObject newItem = Instantiate(prefabShopItem, contentTransformShopMinions);
                        PremiumShopItemUI shopItemUI = newItem.GetComponent<PremiumShopItemUI>();

                        if (shopItemUI)
                        {
                            shopItemUI.SetValues(premiumShopData.shopMinions[i].GetId(), shopMinions[i], shopMinions[i].refBuyMinion.GetId());
                        }

                        newItem.SetActive(true);
                    }
                    break;
                // Use the minion content transform and the shopMinions array.
                case WitchShopTabs.TabItems:
                default:
                    for (int i = 0; i < shopItems.Length; i++)
                    {
                        GameObject newItem = Instantiate(prefabShopItem, contentTransformShopItems);
                        PremiumShopItemUI shopItemUI = newItem.GetComponent<PremiumShopItemUI>();

                        if (shopItemUI)
                        {
                            shopItemUI.SetValues(premiumShopData.shopItems[i].GetId(), shopItems[i]);
                        }

                        newItem.SetActive(true);
                    }
                    break;
            }

            prefabShopItem.SetActive(true);
        }
    }
}