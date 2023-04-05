using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class mouselook : MonoBehaviourPunCallbacks
{
    public GameObject Head;
    public float headLimiter;
    [Header("Look around settings")]
    public float sensitivity = 1f;
    public float vMinimum = -90f;
    public float vMaximum = 90f;
    public Camera cam;
    int currentPlayer;

    public AudioListener listener;
    Transform body;
    float horizontal;
    float vertical;
    private void Awake()
    {

    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            cam.GetComponent<Camera>().enabled = false;
            listener.enabled = false;
        }
        else
        {
            currentPlayer = gameObject.transform.root.GetComponent<PhotonView>().ViewID;
        }
        //gets the player. Done like that so it can work in multiplayer as well
        body = gameObject.transform.root;
        //lock the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //get the current player's starting horizontal direction
        horizontal = body.transform.localEulerAngles.y;

    }
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {

            //change horizontal and vertical direction based on mouse movement and sensitvity
            horizontal += Input.GetAxis("Mouse X") * sensitivity;
            vertical += -Input.GetAxis("Mouse Y") * sensitivity;
            //limit the vertical minumum and maximum movement to set values
            vertical = Mathf.Clamp(vertical, vMinimum, vMaximum);
            //apply the horizontal movement to the body(so the whole body rotates)

            body.transform.localEulerAngles = new Vector3(0, horizontal, 0);
            //apply the vertical movement to the camera(head)(so the body doesn't turn sideways)
            transform.localEulerAngles = new Vector3(vertical, 0, 0);
            photonView.RPC("HeadMove", RpcTarget.AllBuffered, currentPlayer, vertical);

        }
        else
        {

        }
    }
    void LateUpdate()
    {
        if (transform.localEulerAngles.x < 180)
        {
            headLimiter = transform.localEulerAngles.x;
        }
        else
        {
            headLimiter = transform.localEulerAngles.x - 360;
        }

        headLimiter = Mathf.Clamp(headLimiter, -40f, 60f);
        Head.transform.localEulerAngles = new Vector3(headLimiter, 0, 0);
        if (photonView.IsMine == false)
        {
            //  Debug.Log ( vertical);
        }

    }

}