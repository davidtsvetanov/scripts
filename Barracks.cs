using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using Beamable.Common.Api.Inventory;
using Beamable.Examples.Services.ContentService;
using Beamable.Examples.Services.InventoryService.InventoryCurrencyExample;
namespace Beamable.Examples.Services.InventoryService.InventoryCurrencyExample {
    public class Barracks : MonoBehaviour
    {
        public Transform troopsInTranining;
        public GameObject Timer;
        public int TroopSpaces = 1;
        public float setTime;
        bool interactable;
        bool TrainingTroop;
        GameObject troop;
        LimitTroopsInTraining LimitTroopsInTraining;
        public changeCurrency changeCurrency;
        public int price;
        int level;
        getVariable canvasObject;

        private  void Start()
        {
            LimitTroopsInTraining = troopsInTranining.GetComponent<LimitTroopsInTraining>();
            canvasObject = GameObject.Find("GUI").transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<getVariable>();
        }

        void Update()
        {
            if (!interactable)
            {
                if (troop == null)
                {
                    GetComponent<Button>().interactable = true;
                    interactable = true;
                }
            }
        }

        public async void StartTraining()
        {
            if (!TrainingTroop)
            {
                if(int.Parse(CurrencyManager.Instance.Currency1.txtAmount.text) >= price) {
                    if (LimitTroopsInTraining.currentTroops < LimitTroopsInTraining.maxToops - TroopSpaces + 1)
                    {
                        await getBarracksValues();
                        changeCurrency.RemovePrimaryCurrency(price);
                        troop = Instantiate(gameObject);
                        GameObject TimerText = Instantiate(Timer);
                        troop.transform.SetParent(troopsInTranining);
                        troop.transform.localScale = new Vector3(1, 1, 1);
                        TimerText.transform.SetParent(troop.transform);
                        TimerText.transform.position = new Vector3(0, -75f, 0);
                        TimerText.GetComponent<Timer>().timeRemaining = setTime;
                        troop.GetComponent<Barracks>().TrainingTroop = true;
                        LimitTroopsInTraining.currentTroops += TroopSpaces;
                        GetComponent<Button>().interactable = false;
                        interactable = false;
                        Debug.Log(canvasObject.barracksBuilding.GetBeamableItemId());
                    }
                }
            }

        }
        public void GetBuilding(Building building)
        {
           
        }
        public void CancelOrder()
        {
            if (TrainingTroop)
            {
                changeCurrency.AddPrimaryCurrency(price/2);
                LimitTroopsInTraining.currentTroops -= TroopSpaces;
                Destroy(transform.gameObject);
            }
        }
        private async Task<bool> getBarracksValues()
        {
            IBeamableAPI beamableAPI = await Beamable.API.Instance;

            List<InventoryObject<BarracksData>> inventoryUserBuildings = await beamableAPI.InventoryService.GetItems<BarracksData>();
            InventoryObject<BarracksData> buildingID = inventoryUserBuildings.First(inventoryObject => inventoryObject.Id == canvasObject.barracksBuilding.GetBeamableItemId());
            Debug.Log(canvasObject.barracksBuilding.GetBeamableItemId());
            BarracksData buildingData = buildingID.ItemContent;

            bool isLevelParsed = int.TryParse(buildingID.Properties[PropertyConstants.propertyLevel], out level);
            setTime = (buildingData.levelBarracks[level-1].barracksVariables[0].SetTime);
            price = (buildingData.levelBarracks[level-1].barracksVariables[0].price);
            Debug.Log("Price for one troop:" + price);
            Debug.Log("Time to train one troop" + setTime);

            return true;


        }
    }
}
