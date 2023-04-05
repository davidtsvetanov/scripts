using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    static public CameraMovement Instance { get; private set; }
    public float panSmoothing;
    [SerializeField] private float TouchZoomSpeed = 0.01f;
    [SerializeField] private float ZoomMinBound = 2f;
    [SerializeField] private float ZoomMaxBound = 8f;
    public SpriteRenderer backgroundSprite;
    public float boundaryPadding = 0.1f;
    private float cameraLeftBoundary;
    private float cameraRightBoundary;
    private float cameraUpBoundary;
    private float cameraDownBoundary;
    public float maxCamaeraZoomOut = 20f;
    public float maxCamaeraZoomOut1 = 15f;
    public Camera cam;
    public bool cameraActive = true;
  
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }
    void Start()
    {
        cam = GetComponent<Camera>();
        float orthographicSize = cam.orthographicSize;
        float horzExtent = orthographicSize * Screen.width / Screen.height;
        cameraLeftBoundary = (float)(horzExtent - backgroundSprite.sprite.bounds.size.x / 2.0f) + boundaryPadding;
        cameraRightBoundary = (float)(backgroundSprite.sprite.bounds.size.x / 2.0f - horzExtent) - boundaryPadding;
        cameraDownBoundary = (float)(orthographicSize - backgroundSprite.sprite.bounds.size.y / 2.0f) + boundaryPadding;
        cameraUpBoundary = (float)(backgroundSprite.sprite.bounds.size.y / 2.0f - orthographicSize) - boundaryPadding;
    }
 
    void Update()
    {
        Zoom();
        if (BuildingPlacementManager.Instance.IsBuildingMovable())
        {
            return;
        }
        HandleInput();
    }

    public void HandleInput()
    {
        if (Input.touchCount == 1)
        {
            if (cameraActive)
            {
                OnMovement();
            }
            
        }
    }
    private void OnMovement()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 pos = touch.deltaPosition;
            Vector3 position = new Vector3(pos.x / panSmoothing * cam.orthographicSize, pos.y / panSmoothing * cam.orthographicSize, 0);
            Vector3 currentPosition = this.transform.position;
            currentPosition -= position;

            float clampedPosX = Mathf.Clamp(currentPosition.x, cameraLeftBoundary, cameraRightBoundary);
            float clampedPosY = Mathf.Clamp(currentPosition.y, cameraDownBoundary, cameraUpBoundary);

            if (pos != new Vector2(0, 0))
            {
                transform.position = new Vector3(clampedPosX, clampedPosY, transform.position.z);
            }
        }
    }

    void Zoom()
    {      
        // Pinch to zoom
        if (Input.touchCount == 2)
        {
            // get current touch positions
            Touch tZero = Input.GetTouch(0);
            Touch tOne = Input.GetTouch(1);
            // get touch position from the previous frame
            Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
            Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

            float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
            float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

            // get offset value
            float deltaDistance = oldTouchDistance - currentTouchDistance;
            Zoom(deltaDistance, TouchZoomSpeed);
            float orthographicSize = cam.orthographicSize;
            float horzExtent = orthographicSize * Screen.width / Screen.height;
            cameraLeftBoundary = (float)(horzExtent - backgroundSprite.sprite.bounds.size.x / 2.0f) + boundaryPadding;
            cameraRightBoundary = (float)(backgroundSprite.sprite.bounds.size.x / 2.0f - horzExtent) - boundaryPadding;
            cameraDownBoundary = (float)(orthographicSize - backgroundSprite.sprite.bounds.size.y / 2.0f) + boundaryPadding;
            cameraUpBoundary = (float)(backgroundSprite.sprite.bounds.size.y / 2.0f - orthographicSize) - boundaryPadding;
        }
        
    }

    void Zoom(float deltaMagnitudeDiff, float speed)
    {
        float ortSize = cam.orthographicSize + deltaMagnitudeDiff * speed;
        // set min and max value of Clamp function upon your requirement
        ortSize = Mathf.Clamp(ortSize, ZoomMinBound, ZoomMaxBound);
        float horzExtent = ortSize * Screen.width / Screen.height;
        cameraLeftBoundary = (float)(horzExtent - backgroundSprite.sprite.bounds.size.x / 2.0f) + boundaryPadding;
        cameraRightBoundary = (float)(backgroundSprite.sprite.bounds.size.x / 2.0f - horzExtent) - boundaryPadding;
        cameraDownBoundary = (float)(ortSize - backgroundSprite.sprite.bounds.size.y / 2.0f) + boundaryPadding;
        cameraUpBoundary = (float)(backgroundSprite.sprite.bounds.size.y / 2.0f - ortSize) - boundaryPadding;
     
        Vector3 currentPosition = this.transform.position;

        float clampedPosX = Mathf.Clamp(currentPosition.x, cameraLeftBoundary, cameraRightBoundary);
        float clampedPosY = Mathf.Clamp(currentPosition.y, cameraDownBoundary, cameraUpBoundary);

        transform.position = new Vector3(clampedPosX, clampedPosY, transform.position.z);
        cam.orthographicSize = ortSize;

    }
}

