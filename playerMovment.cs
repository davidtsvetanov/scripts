using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Steamworks;



public class playerMovment : MonoBehaviourPunCallbacks
{
    //ladder movment
    public bool climbing;
    private Vector3 move;
    //setting up ragdoll vars
    public GameObject body;
    public GameObject body1;
    private Rigidbody[] ragdollRb;
    private Collider[] ragdollCol;
    private Vector3 SpawnPosition;
    public bool dead;
    public bool regenning;
    //text chat
    public Text ChatTextObject;
    public GameObject ChatManager;
    public string myTeam;
    public string team1;
    //pickup of flag
    public PickUp pickup;
    public GameObject Head;
    int doonce;
    //detect movment for the diffrent annimations
    bool isrunning1 = true;
    bool isrunning2 = true;
    bool isjumping1 = true;
    bool isjumping2 = true;
    //spawnpoints for the players
    private GameObject spawnpoint1;
    private GameObject spawnpoint2;
    public GameObject Player;
    //testing 
    public float shotsOnTarget = 0;
    private Scored scored;
    //diffrent magics
    public lookMagic lookmagic;
    public flamethrower flamethrower;
    public gun gun;
    //Networking
    public Camera camera1;
    //Health System
    [Header("Health")]
    public float health = 100;
    float healthCarry;
    public float maxHealth = 100;
    public Image healthbar;
    public Text healtText;

    //Mana System
    [Header("Mana")]
    public float mana = 100;
    public float maxMana = 100;
    public Image manabar;
    public Text manaText;

    //Character controller
    private CharacterController controller;
    //Gravity
    [Space(1)]
    [Header("Gravity")]
    public float speed = 12f;
    public float gravityMultiplier;
    Vector3 moveVector;
    float lengthOfFall = 0;
    public float jumpSpeed;
    float x;
    float z;
    float xAir;
    float zAir;
    GameObject Flag;
    GameObject Flag1;

    public Coroutine co_1;

    //Roomname
    [Space(1)]
    [Header("RoomInfo")]
    public Text RoomName;

    public GameObject projectile;
    public GameObject projectile1;

    bool test = false;
    #region Jumping and gravity variables
    private Vector3 moveDirection = Vector3.zero;
    public Animator animator;
    float currentY;
    float prevY;
    bool falling = true;
    float fallDamage;
    bool fallOnce = true;
   public bool attacking;
    public bool touchingEnemy;
    public slash slash;
    public bool flagTaken;
    #endregion
    private void Awake()
    {
        //initialise name and picture from steam
        if (!SteamManager.Initialized) { return; }
        string name = SteamFriends.GetPersonaName();
        PhotonNetwork.LocalPlayer.NickName = name;
    }
    void Start()
    {
        //assign values to variables 
        scored = GameObject.Find("Scored").GetComponent<Scored>();
        body1 = body.transform.parent.gameObject;
        ChatTextObject = GameObject.Find("TextChatManager").transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>();
        SpawnPosition = transform.position;
        Debug.Log("The player nickname is: " + PhotonNetwork.LocalPlayer.NickName);
        Flag = GameObject.Find("BannerFrame");
        Flag1 = GameObject.Find("BannerFrameRed");
        ragdollRb = body1.GetComponentsInChildren<Rigidbody>();
        ragdollCol = body1.GetComponentsInChildren<Collider>();
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
        //setting up spawnpoints
        spawnpoint1 = GameObject.Find("GameSet").gameObject.transform.GetChild(1).gameObject;
        spawnpoint2 = GameObject.Find("GameSet").gameObject.transform.GetChild(2).gameObject;
       
        //make all player's bodies kinematic
        foreach (Rigidbody rb in ragdollRb)
        {
            Debug.Log(rb.name);
            rb.isKinematic = true;

        }
        //Photon Scene Set Up
        controller = GetComponent<CharacterController>();
        if (photonView.IsMine)
        {
            myTeam = team1;
            shotsOnTarget = PlayerPrefs.GetFloat("Target", 0);
            mana = 100;
            health = 100;
            //Sets the current Y (will be compared to the previous one later)
            currentY = transform.position.y;
            //Gives the previous Y the current position, because it has not previous(will be compared to the current one later)
            prevY = transform.position.y;
            RoomName.text = "Roomname: " + PhotonNetwork.CurrentRoom.Name;
            Debug.Log("You are in room " + PhotonNetwork.CurrentRoom.Name);
        }
        else
        {
            myTeam = null;
        }
    }
    // melee attack and  animation stop
    private IEnumerator StopSlash()
    {
        yield return new WaitForSeconds(1);
        photonView.RPC("StopAnimation", RpcTarget.AllBuffered);
        attacking = false;
        slash.attacked = false;
    }
    void Update()
    {
        //teleportating the player to spawn after scoring point
        if (scored.scored)
        {
            scored.scored = false;
            controller.enabled = false;
            transform.position = SpawnPosition;
            controller.enabled = true;

        }
        //calling melle attack and annimation
        if (Input.GetMouseButtonDown(1))
        {
            if (attacking == false)
            {
                photonView.RPC("SlashAnimation", RpcTarget.AllBuffered);
                attacking = true;
            }
        }
        //check if the game is started
        if (SceneManager.GetActiveScene().name == "1V1 Arena")
        {
            //if you are dead
            if (regenning)
            {
                x = 0;
                z = 0;

                healthCarry += 0.3f;
                health = healthCarry;
                if (health >= 100)
                {
                    health = 100;
                    regenning = false;
                    dead = false;
                    healthCarry = 0;
                    camera1.GetComponent<mouselook>().enabled = true;
                    controller.enabled = false;
                    transform.position = SpawnPosition;
                    controller.enabled = true;
                }
            }
            // tell the other players you are dead
            if (health <= 0)
            {

                photonView.RPC("restarter", RpcTarget.AllBuffered, 1f);

                if (!photonView.IsMine)
                {
                    transform.position = spawnpoint1.transform.position;
                    PlayerPrefs.SetInt("dead1", 1);
                }
                else
                {
                    camera1.GetComponent<mouselook>().enabled = false;
                    transform.position = spawnpoint2.transform.position;
                    PlayerPrefs.SetInt("dead", 1);
                }
            }
        }
        if (photonView.IsMine)
        {

            // //Sets the current Y every frame
            currentY = transform.position.y;
            //Compares the previous and the current Y, to understand if the player is falling or not
            if (currentY < prevY - 0.01f)
            {
                falling = false;
            }
            else
            {
                falling = true;

            }
            //after they have been compared, the previous one get's a new position for next frame
            prevY = currentY;
            //getting the horizontal axis(a&d)
            if (!dead)
            {
                //player movment
                if (controller.isGrounded)
                {
                    x = Input.GetAxis("Horizontal");
                    //getting the vertical axis(w&s)
                    z = Input.GetAxis("Vertical");

                    xAir = 0;
                    zAir = 0;
                }
                else
                {
                    xAir = Input.GetAxis("Horizontal");
                    zAir = Input.GetAxis("Vertical");
                }
            }
            if (z != 0 || x != 0)
            {
                if (isrunning1)
                {
                    //setting annimations
                    photonView.RPC("animationStartRunning", RpcTarget.AllBuffered, 1f);
                    isrunning2 = true;
                    isrunning1 = false;
                }
            }
            // stop annimation
            else
            {
                if (isrunning2)
                {
                    photonView.RPC("animationStopRunning", RpcTarget.AllBuffered, 1f);
                    isrunning1 = true;
                    isrunning2 = false;
                }

            }
            //applying gravity
            moveVector += Physics.gravity * gravityMultiplier;
            //increasing the vlocity of the player when he jumps of a high place
            moveVector.y -= lengthOfFall * 0.02f;
            //storing the value above in 1 vector
            if (x != 0 && z != 0)
            {
                x = x / 2;

            }
            if (climbing == false)
            {
                move = (transform.right * Mathf.Clamp((x + xAir / 2), -1, 1) + (moveVector + moveDirection) + transform.forward * Mathf.Clamp((z + zAir / 2), -1, 1)) / 2.5f;
            }
            else
            {
                x = Input.GetAxis("Horizontal");
                //getting the vertical axis(w&s)
                z = Input.GetAxis("Vertical");
                move = (transform.right * x + transform.up * z) / 2.5f;

            }
            //using the move vector to move the character
            controller.Move(move * speed * Time.deltaTime);
            //reseting the gravity vector after it was applied for next time
            moveVector = Vector3.zero;
            //Health math (so it shows up correct in the User Interface)
            health = Mathf.Clamp(health, 0.0f, 100.0f);
            health = Mathf.Round(health);
            //healthbar & healtText display ("" is needed to make the text work)
            healtText.text = "" + health;
            healthbar.fillAmount = health / maxHealth;
            //manabar & manatext display ("" is needed to make the text work)
            manaText.text = "" + Mathf.Round((Mathf.Clamp(mana, 0.0f, 100.0f)));
            manabar.fillAmount = mana / maxMana;
            //If you press Ctrl player crouch
            if (Input.GetKey(KeyCode.LeftControl))
            {
                //make the height 1(smoothly)
                if (controller.center.y < 1)
                {
                    controller.center += new Vector3(0, 0.05f, 0);
                }
            }
            else
            {
                //make the height 3(smoothly)
                if (controller.center.y > .5f)
                {
                    controller.center -= new Vector3(0, 0.05f, 0);
                }
            }

        }
        if (Time.timeScale == 0 && doonce < 121)
        {
            // Debug.Log("1");
            doonce++;
            if (doonce >= 120)
            {
                Invoke("unpause", 2.0f);
                //  Debug.Log("2");
                Time.timeScale = 1;
                doonce = 0;
            }
        }

    }
    void FixedUpdate()
    {
        //setting up player
        if (photonView.IsMine)
        {
            if (health != 0 && regenning == false)
            {
                body1.GetComponent<Animator>().enabled = true;
                foreach (Rigidbody rb in ragdollRb)
                {
                    rb.isKinematic = true;
                }
            }
            // mana regeneration
            if (mana < 100)
            {
                mana += .2f;
            }
            //when the character is touching the ground
            if (climbing == false)
            {
                if (controller.isGrounded)
                {

                    //this checks if the player has fallen from a high enough position to take damage
                    if (fallDamage > 80)
                    {
                        photonView.RPC("fallDamage1", RpcTarget.AllBuffered, fallDamage, health);
                        //deals damage depending on the fall

                    }
                    //resets the lenght of fall 
                    lengthOfFall = 0;
                    //resets fallDamage;
                    fallDamage = 0;
                    //resets verical velocity of the jump
                    moveDirection.y = 0;
                }

                //if the player is in the air
                else
                {
                    //gravity that keeps increasing
                    lengthOfFall += 2;
                    //if the player is falling(although it doesn't make sense the way it is written)
                    if (!falling)
                    {
                        //do once
                        if (fallOnce)
                        {
                            fallOnce = false;
                            //set the gravity to 40(we need that amount to make the player fall, if it is 0, he just floats)
                            lengthOfFall = 40;
                        }
                        //keep increasing the fall damage, the longer tha fall is
                        fallDamage += 2;
                    }

                }
            }
            //if you press Space 
            if (controller.isGrounded && Input.GetButton("Jump"))
            {
                if (controller.isGrounded)
                {
                    photonView.RPC("animationRestartJumping", RpcTarget.AllBuffered, 1f);
                }
                if (isjumping1)
                {
                    //Jump
                    moveDirection.y = jumpSpeed;
                    photonView.RPC("animationStartJumping", RpcTarget.AllBuffered, 1f);
                    isjumping2 = true;
                }

            }
            else
            {

                if (controller.isGrounded && isjumping2)
                {
                    isjumping2 = false;
                    photonView.RPC("animationStopJumping", RpcTarget.AllBuffered, 1f);
                }
            }


        }

    }
    public void OnTriggerEnter(Collider collider)
    {
        //detect ladders and climb
        if (collider.tag == "Ladder")
            climbing = true;
    }
    public void OnTriggerExit(Collider collider)
    {
        //stop climbing
        if (collider.tag == "Ladder")
            climbing = false;
    }
    //display shooting accuracy
    public void shot()
    {
        if (PlayerPrefs.GetInt("shooting", 0) == 1)
        {
            PlayerPrefs.SetFloat("Target", shotsOnTarget);

            //  Debug.Log("shotsOnTarget: " + shotsOnTarget);
        }
    }
    // synchronising multiplayer functions
    [PunRPC]
    public void restarter(float test)
    {
        foreach (Rigidbody rb in ragdollRb)
        {
            body1.GetComponent<Animator>().enabled = false;
            rb.isKinematic = false;
        }
        controller.Move(new Vector3(0,0,0));
        dead = true;
        regenning = true;
    }


    [PunRPC]
    public void animationStartRunning(float x)
    {
        animator.SetBool("isRunning", true);
    }
    [PunRPC]
    public void animationStopRunning(float x)
    {
        animator.SetBool("isRunning", false);
    }
    [PunRPC]
    public void animationRestartJumping(float x)
    {
        animator.Play("Jump", -1, 0f);
    }
    [PunRPC]
    public void animationStartJumping(float x)
    {
        animator.SetBool("isJumping", true);


    }
    [PunRPC]
    public void animationStopJumping(float x)
    {
        animator.SetBool("isJumping", false);

    }
    // use third magic when you have full mana and set it to 0
    [PunRPC]
    public void InstLookMagic(Vector3 x)
    {
        if (mana >= 100)
        {
            Instantiate(lookmagic.loader, x, new Quaternion(-90, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z, 0));
            if (photonView.IsMine)
            {
                mana -= 100;
            }
        }
    }
   
    [PunRPC]
    public void Inst2(float x)
    {
        Debug.Log("Instantiating");
        flamethrower.activated = true;
        flamethrower.Inst1(camera1.transform);
    }
    //instantiate fireball
    [PunRPC]
    public void fireball(Vector3 pos, Quaternion rot)
    {
        Instantiate(projectile, pos, rot);
    }
    //instantiate sparkball
    [PunRPC]
    public void sparkball(Vector3 pos, Quaternion rot)
    {
        Instantiate(projectile1, pos, rot);
    }
    // sync health between players
    [PunRPC]
    public void healthSync(int ID, float maxDamage)
    {

        if (gameObject.GetComponent<PhotonView>().ViewID == ID)
        {
            Debug.Log(maxDamage);
            health -= maxDamage;
        }

    }
    //set fall damage for all players
    [PunRPC]
    public void fallDamage1(float fallDamage1, float health1)
    {
        health = health1;
        health -= ((fallDamage1 - 80) * .5f);
    }
    //sync head movement of player
    [PunRPC]
    public void HeadMove(int playerID, float vertical)
    {
        if (gameObject.GetComponent<PhotonView>().ViewID == playerID)
        {
            camera1.transform.localEulerAngles = new Vector3(vertical, 0, 0);
        }
    }
    //picking up flag
    [PunRPC]
    public void clickedOnFlag(bool clicked)
    {
        GameObject.Find("BannerFrame").GetComponent<PickUp>().clicked = clicked;
    }
    //when flag is delivered restart and teleport the player
    [PunRPC]
    public void teleportPlayer()
    {
        scored.scored = true;
    }
    //flag pickup
    [PunRPC]
    public void FlagTeleporter()
    {
        Flag.GetComponent<PickUp>().clicked = false;
        Flag.GetComponent<PickUp>().doOnce2 = true;
        Flag.transform.position = new Vector3(100, 1.3f, 77);
        Flag.GetComponent<PickUp>().release = false;
        Flag.transform.parent = null;
        Flag.GetComponent<Rigidbody>().isKinematic = false;
        Flag1.GetComponent<PickUp>().clicked = false;
        Flag1.GetComponent<PickUp>().doOnce2 = true;
        Flag1.transform.position = new Vector3(252, 1.3f, 59.8f);
        Flag1.GetComponent<PickUp>().release = false;
        Flag1.transform.parent = null;
        Flag1.GetComponent<Rigidbody>().isKinematic = false;


    }
    //text chat
    [PunRPC]
    public void SendMessage(string message, string name)
    {
        ChatTextObject.text += "\n" + name + ": " + message;
        Debug.Log("hi1");
        co_1 = StartCoroutine(GameObject.Find("TextChatManager").GetComponent<ChatManager>().Remove());
    }
    //sync slash annimation
    [PunRPC]
    public void SlashAnimation()
    {
        animator.SetBool("isAttacking", true);
        StartCoroutine(StopSlash());
    }
    [PunRPC]
    public void StopAnimation()
    {
        animator.SetBool("isAttacking", false);
        
    }
    //element choosing sync
    [PunRPC]
    public void Fire()
    {
        gun.inHandProjectileSparks.SetActive(false);
        gun.inHandProjectile.SetActive(true);
        gun.FireCh = true;
        gun.IceCh = false;
        gun.EarthCh = false;
        gun.LightningCh = false;        
    }
    [PunRPC]
    public void Ice()
    {
        gun.inHandProjectileSparks.SetActive(false);
        gun.inHandProjectile.SetActive(false);

        gun.FireCh = false;
        gun.IceCh = true;
        gun.EarthCh = false;
        gun.LightningCh = false;
            
        
    }
    [PunRPC]
    public void Earth()
    {
        gun.inHandProjectileSparks.SetActive(false);
        gun.inHandProjectile.SetActive(false);

        gun.FireCh = false;
        gun.IceCh = false;
        gun.EarthCh = true;
        gun.LightningCh = false;
           
        
    }
    [PunRPC]
    public void Lighting()
    {

        gun.inHandProjectile.SetActive(false);
        gun.inHandProjectileSparks.SetActive(true);

        gun.FireCh = false;
        gun.IceCh = false;
        gun.EarthCh = false;
        gun.LightningCh = true;
    }
    }