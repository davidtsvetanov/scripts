using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using Beamable.Common.Api.Inventory;
using Beamable.Examples.Services.ContentService;
using Beamable.Examples.Services.InventoryService.InventoryCurrencyExample;

namespace Beamable.Examples.Services.InventoryService
{
    public class goldCoinGenerator : MonoBehaviour
    {
        public int currentLakeValue;

        public GameObject popingCoin;
        public bool DoOnce;
        
        private float timeForOneCoin = 10;
   
        int level;

        LakeData buildingData;
        InventoryObject<LakeData> buildingID;
        private Building building;

        private async void Start()
        {
            building = GetComponent<Building>();
          
            await getLakeValues();
        }

        private async Task<bool> getLakeValues()
        {
            IBeamableAPI beamableAPI = await Beamable.API.Instance;

            List<InventoryObject<LakeData>> inventoryUserBuildings = await beamableAPI.InventoryService.GetItems<LakeData>();
            buildingID = inventoryUserBuildings.First(inventoryObject => inventoryObject.Id == building.GetBeamableItemId());

            buildingData = buildingID.ItemContent;

            bool isLevelParsed = int.TryParse(buildingID.Properties[PropertyConstants.propertyLevel], out level);
            Debug.Log(building.GetBeamableItemId());
            timeForOneCoin = buildingData.lakeVariables[level - 1].setTime;
            return true;            
        }
        // Update is called once per frame
        void Update()
        {
            if (buildingData!=null)
            {
                if (BuildingPlacementManager.Instance.IsBuildingMovable())
                {
                    return;
                }
                if (currentLakeValue >= buildingData.popUpCoinValue && DoOnce == false)
                {
                    popingCoin.SetActive(true);
                }
                if (currentLakeValue >= buildingData.lakeVariables[level-1].maxLakeValue)
                {
                    currentLakeValue = buildingData.lakeVariables[level - 1].maxLakeValue;
                    return;
                }

                if (timeForOneCoin > 0)
                {
                    timeForOneCoin -= Time.deltaTime;
                }
                else
                {
                    currentLakeValue += buildingData.lakeVariables[level - 1].increaseBy;
                    timeForOneCoin = buildingData.lakeVariables[level - 1].setTime;
                }
            }
        }
    }
}