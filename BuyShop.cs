using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyShop : MonoBehaviour
{
    private bool canBuy;
    [SerializeField] GameObject buyMenu;
    [SerializeField] TextMeshProUGUI buyButtonTxt;
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            buyButtonTxt.gameObject.SetActive(true);
            canBuy = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player")
        {
            buyButtonTxt.gameObject.SetActive(false);
            canBuy = false;
            buyMenu.GetComponent<ShopManager>().infoMenu.gameObject.SetActive(false);
            buyMenu.SetActive(false);
            CharController_Motor.Instance.inMenu = false;
        }


    }

    private void Update()
    {
        if(canBuy == false)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))

        {
            buyButtonTxt.gameObject.SetActive(false);
            buyMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            CharController_Motor.Instance.inMenu = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buyButtonTxt.gameObject.SetActive(true);
            buyMenu.GetComponent<ShopManager>().infoMenu.gameObject.SetActive(false);
            buyMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            CharController_Motor.Instance.inMenu = false;
        }
    }


}
