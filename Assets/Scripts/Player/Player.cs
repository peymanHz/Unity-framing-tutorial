using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : SingletonMonobehavior<Player>
{
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds useToolAnimationPause;
    private AnimationOverride animationOverride;
    private GridCursor gridCursor;

    // Movment Parameters

    public float xInput;
    public float yInput;

    public bool isWalking;
    public bool isRunning;
    public bool isIdle;
    public bool isCarrying = false;

    public ToolEffect toolEffect = ToolEffect.none;

    public bool isUsingToolRight;
    public bool isUsingToolLeft;
    public bool isUsingToolUp;
    public bool isUsingToolDown;

    public bool isLiftingToolRight;
    public bool isLiftingToolLeft;
    public bool isLiftingToolUp;
    public bool isLiftingToolDown;

    public bool isSwingingToolRight;
    public bool isSwingingToolLeft;
    public bool isSwingingToolUp;
    public bool isSwingingToolDown;

    public bool isPickingRight;
    public bool isPickingLeft;
    public bool isPickingUp;
    public bool isPickingDown;

    private Camera mainCamera;
    private bool playerToolUseDisabled = false;

    private Rigidbody2D rb2d;
#pragma warning disable 414
    private Direction playerDirection;
#pragma warning restore 414

    private List<CharachterAttribute> characterAttributeCustomasationList;

    private float movmentSpeed;

    [Tooltip("should be populated in the prefab with the equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equipeditemSpriteRenderer = null;

    //player attribute that can be swapped
    private CharachterAttribute armCharacterAttribute;
    private CharachterAttribute toolCharacterAttribute;


    private bool _playerInputIsDisabled = false;

    public bool PlayerInputDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    protected override void Awake()
    {
        base.Awake();

        rb2d = GetComponent<Rigidbody2D>();

        animationOverride = GetComponentInChildren<AnimationOverride>();

        //initialise swapped character attribute 
        armCharacterAttribute = new CharachterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);

        //initialise charcter attribute list
        characterAttributeCustomasationList = new List<CharachterAttribute>();

        // get the main camera
        mainCamera = Camera.main;
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();

        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.AfterUseToolAnimationPause);
    }

    private void Update()
    {
        #region Player Input

        if (!PlayerInputDisabled) 
        { 
            ResetAnimationTriggers();

            PlayerMovmentInput();

            PlayerWalkInput();
            
            PlayerClickInput();

            PlayerTestInpur();

            //Send Event To Any Listeners For Player Movment Input
            EventHandler.CallMovemntEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);

        }

        #endregion
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement() 
    {
        Vector2 move = new Vector2(xInput * movmentSpeed * Time.deltaTime, yInput * movmentSpeed * Time.deltaTime);

        rb2d.MovePosition(rb2d.position + move);
    }

    private void ResetAnimationTriggers()
    {
        toolEffect = ToolEffect.none;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
    }

    private void PlayerMovmentInput()
    {
        yInput = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxisRaw("Horizontal");

        if (yInput != 0 && xInput != 0)
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
        }

        if (yInput != 0 || xInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movmentSpeed = Settings.runningSpped;

            // Capture Player Direction For Save Game
            if (xInput < 0)
            {
                playerDirection = Direction.left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.right;
            }
            else if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.up;
            }
        }
        else if (xInput == 0 && yInput == 0)
        {
            isRunning = false; 
            isWalking = false; 
            isIdle = true;
        }
    }

    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movmentSpeed = Settings.walkingSpeed;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movmentSpeed = Settings.runningSpped;
        }
    }

    private void PlayerClickInput()
    {
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {

                if (gridCursor.CursorIsEnabled)
                {
                    //get cursor grid position
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();

                    //get player grid position
                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovment();

        Vector3Int playerDirection = GetPlayerClickFirection(cursorGridPosition, playerGridPosition);

        //get grid property details at cursor position (the gridCursor validation routine ensures that grid property details are niot null)
        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        //get selecte item details
        ItemsDetails itemsDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemsDetails != null)
        {
            switch (itemsDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(itemsDetails);
                    }
                    break;

                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemsDetails);
                    }
                    break;

                case ItemType.Hoeing_Tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemsDetails, playerDirection); 
                    break;

                case ItemType.none: 
                    break;
                    
                case ItemType.count: 
                    break;

                default:
                    break;
            }
        }
    }

    private Vector3Int GetPlayerClickFirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else 
        { 
            return Vector3Int.down;
        }
    }


    private void ProcessPlayerClickInputSeed(ItemsDetails itemsDetails)
    {
        if (itemsDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputCommodity(ItemsDetails itemsDetails)
    {
        if (itemsDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemsDetails itemsDetails, Vector3Int playerDirection)
    {
        //switch on tool
        switch (itemsDetails.itemType)
        {
            case ItemType.Hoeing_Tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;

            default: 
                break;
        }
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        //trigger animation
        StartCoroutine(HoeGroundAtCursorCoroutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator HoeGroundAtCursorCoroutine(Vector3Int playerDirection,  GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputDisabled = true;
        playerToolUseDisabled = true;

        //set tool animatiion to hoe in override animation
        toolCharacterAttribute.partVariantType = PartVariantType.hoe;
        characterAttributeCustomasationList.Clear();
        characterAttributeCustomasationList.Add(toolCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeCustomasationList);

        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;

        //set grid property details for dug ground
        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        //set grid property to dug
        GridPropertyManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //display dug ground tiles
        GridPropertyManager.Instance.DisplayDugGround(gridPropertyDetails);

        //after animation pause
        yield return afterUseToolAnimationPause;

        PlayerInputDisabled = false;
        playerToolUseDisabled = false;
    }

    //TODO: Remove
    /// <summary>
    /// temp routine for test input
    /// </summary>
    private void PlayerTestInpur()
    {
        //trigger advance time
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }
        //trigger advance day
        if (Input.GetKey(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
        if (Input.GetKey(KeyCode.L))
        {
            SceneControllerManager.Instance.FadeAndloadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }
    }

    public void ResetMovment()
    {
        xInput = 0; 
        yInput = 0;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    public void DisablePlayerInputAndResetMovment()
    {
        DisablePlayerInput();
        ResetMovment();

        // send event to any listener for player movment input
        EventHandler.CallMovemntEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
            isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
            isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
            isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
            false, false, false, false);
    }

    public void DisablePlayerInput()
    {
        PlayerInputDisabled = true;
    }

    public void EnablePlayerInput()
    {
        PlayerInputDisabled = false;
    }

    public void ClearCarriedItem()
    {
        equipeditemSpriteRenderer.sprite = null;
        equipeditemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        //apply 'carry' character arms customisation
        armCharacterAttribute.partVariantType = PartVariantType.none;
        characterAttributeCustomasationList.Clear();
        characterAttributeCustomasationList.Add(armCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeCustomasationList);

        isCarrying = false;
    }

    public void ShowCarriedItem(int itemcode)
    {
        ItemsDetails itemsDetails = InventoryManager.Instance.GetItemDetails(itemcode);
        if (itemsDetails != null)
        {
            equipeditemSpriteRenderer.sprite = itemsDetails.itemSprite;
            equipeditemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            //apply 'carry' character arms customisation
            armCharacterAttribute.partVariantType = PartVariantType.carry;
            characterAttributeCustomasationList.Clear();
            characterAttributeCustomasationList.Add(armCharacterAttribute);
            animationOverride.ApplyCharacterCustomisationParameters(characterAttributeCustomasationList);

            isCarrying = true;
        }
    }

    public Vector3 GetPlayerViewPortPosition()
    {
        //vector3 viewPort for player ((0, 0) viowport bottom left, (1, 1) viewport top right
        return mainCamera.WorldToViewportPoint(transform.position);
    }

}
