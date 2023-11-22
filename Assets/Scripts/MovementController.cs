using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class MovementController : MonoBehaviour
{
    public GameController gameController;

    [Header("Player")]
    public float playerScore;
    public TMP_Text playerScoreText;
    public float playerSpeed;
    public float health;
    public Transform spawnPoint;
    public GameObject closestFlag;
    public Color playerColour;
    public BoxCollider2D snakeCollider;
    public Camera playerCamera;
    public string snakeTag;
    public string buildingTag;
    public string wallTag;
    public string wallLayer;
    public bool combatModeCheck = false;
    public bool buildModeCheck = true;
    public BuildingPlacement buildingController;
    public CombatMode combatMode;
    //public Color playerTrailMod;
    [Header("Resources")]
    public float food = 100.0f;
    public float stone = 100.0f;
    public float gold = 100.0f;

    [Header("Player Input")]
    public float deadZone = 0.1f;
    public string horizontalInputAxis = "Horizontal";
    public string verticalInputAxis = "Vertical";
    public string sprintInputButton = "Sprint";
    public string placeFarm = "Farm";
    public string placeQuarry = "Quarry";
    public string placeMine = "Mine";
    public string flagCharge = "Flag";
    public string placeWall = "Wall";
    public string fire = "Fire";
    public string landmine = "Landmine";
    public string mode = "Mode";
    public string map = "Map";
    public KeyCode debugButton;
    public KeyCode instaFood;
        
    public float horizontalInput;
    public float verticalInput;
    public bool wallInput;
    public bool sprintInput;   
    public bool foodCollectorPress;
    public bool mineInput;
    public bool quarryInput;
    public bool flagInput;
    public bool modeInput;
    public bool fireInput;
    public bool landmineInput;
    public bool mapInput;

    [Header("Other Player")]
    public MovementController otherPlayer;
    public string otherPlayerWallTag;
    public LayerMask otherPlayerWall;
    public Color otherSnakeColour;
    public Color otherSnakeTrailColour;
     

    [Header("Gameplay Modifers")]
    public int normalTailSize = 1;
    public int invadingTailSize = 1;
    public int activeTailSize;
    public float firstClaimdelay = 10;
    public float sprintSpeed;
    public float sprintFoodUseRate = 2.0f;
    public float foodUseRate = 1.0f;
    public string foodTag;
    public float foodPileAmount;
    public float resourceMin;
    public float foodMax;
    public float goldMax;
    public float stoneMax;  
    public float wallCheckDistance;
    public float buildingCheckDistance;
    public float respawnTimer;
    public float foodFlashTextAmount;

    [Header("Juice Stuff")]
    public AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip claimSound;


    [Header("UI")]
    public Slider healthSlider;

    public TMP_Text foodText;
    public TMP_Text stoneText;
    public TMP_Text goldText;
    public Image modeIcon;       
    public GameObject bigMap;
    public Image southIcon;
    public Image westIcon;
    public Image eastIcon;
    public Image northIcon;

    [Header("UI Sprites")]
    public Sprite buildModeIcon;
    public Sprite combatModeIcon;
    public Sprite farmIcon;
    public Sprite quarryIcon;
    public Sprite mineIcon;
    public Sprite flagIcon;
    public Sprite projectileIcon;
    public Sprite combatmodeIcon2;
    public Sprite landmineIcon;
    public Sprite combatModeIcon4;

    [Header("TileMap")]
    public Tilemap tilemap;
    public float gridSize;
    public Vector3Int snakeEntryPOS;
    public Vector3Int snakeExitPOS;
    public TileBase entryTile;
    public Color baseTileColour;
    public Color entryTileColour;
    public Color surroundingTileColour;
     
    
    [Header("Tile POS Lists")]
    public List<Vector3Int> tilesToChange = new List<Vector3Int>();
    public List<Vector3Int> surroundingTiles = new List<Vector3Int>();
    public List<Vector3Int> playerTiles = new List<Vector3Int>();
    public List<Vector3Int> occupiedTiles = new List<Vector3Int>();

   
    public float timer = 0f;
    Color snakeTrailColour;    
    private Vector2 lastVelocity;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection = Vector2.right;
    Vector3Int[] surroundingTilePOS = new Vector3Int[]
  {
        new Vector3Int(-1, 1, 0),  // Top-left
        new Vector3Int(0, 1, 0),   // Top
        new Vector3Int(1, 1, 0),   // Top-right
        new Vector3Int(-1, 0, 0),  // Left
        new Vector3Int(1, 0, 0),   // Right
        new Vector3Int(-1, -1, 0), // Bottom-left
        new Vector3Int(0, -1, 0),  // Bottom
        new Vector3Int(1, -1, 0)   // Bottom-right
  };
    private bool hungerOn;
    private bool sprinting;
    private float speed = 5.0f;
    private bool wallPlacing;
    private bool isCharging;
    private float chargeTimer = 0f;
    public bool isRespawning;
    private float startingHealth;
    private void Awake()
    {
        //Setting up Movement/Input   
        rb = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
        speed = playerSpeed;
        
        //Get other Controller Scripts
        buildingController = GetComponent<BuildingPlacement>();
        combatMode = GetComponent<CombatMode>();

       
    }


    void Start()
    {



        startingHealth = health;
        modeIcon.sprite = buildModeIcon;
        snakeCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = playerColour;       
        snakeTrailColour = playerColour + Color.gray;
        bigMap.gameObject.SetActive(false);
        //Debug.Log("Snake Colour:" + playerColour);
        GetInfo();        
               
    }
    
    void FixedUpdate()
    {
        if (!isRespawning)
        {
            // Apply force to move the snake
            rb.velocity = moveDirection * speed;
            //lastVelocity = rb.velocity;
            CheckTileCollision(transform.position);
            UpdateUI();

        }


    }
  

    void GetInfo()
    {
        //Get Other Player Info
        MovementController[] players = FindObjectsOfType<MovementController>();
        foreach (MovementController player in players)
        {
            if (player.gameObject != this.gameObject)
            {
                otherPlayer = player;
            }
        }
        otherSnakeColour = otherPlayer.playerColour;
        otherSnakeTrailColour = otherPlayer.playerColour + Color.gray;

        //Get Tilemap Info
        tilemap = FindObjectOfType<Tilemap>();
        baseTileColour = tilemap.color;
    }   

    void Update()
    {       
        //Resource Clamping
        food = Mathf.Clamp(food, resourceMin, foodMax);
        stone = Mathf.Clamp(stone, resourceMin, stoneMax);
        gold = Mathf.Clamp(gold, resourceMin, goldMax);
        playerScore = playerTiles.Count;
        //Movement Input
        horizontalInput = Input.GetAxis(horizontalInputAxis);
        if (Mathf.Abs(horizontalInput) < deadZone)
        {
            horizontalInput = 0f;
        }
        verticalInput = Input.GetAxis(verticalInputAxis);
        if (Mathf.Abs(verticalInput) < deadZone)
        {
            verticalInput = 0f;
        }

        //Wall & Sprint Input
        wallInput = Input.GetButton(placeWall);
        sprintInput = Input.GetButton(sprintInputButton);

        //Combat Input        
        fireInput = Input.GetButtonDown(fire);
        landmineInput = Input.GetButtonDown(landmine);

        //Build Input
        foodCollectorPress = Input.GetButtonDown(placeFarm);        
        mineInput = Input.GetButtonDown(placeMine);
        quarryInput = Input.GetButtonDown(placeQuarry);
        flagInput = Input.GetButtonDown(flagCharge);

        //Misc Input
        modeInput = Input.GetButtonDown(mode);
        mapInput = Input.GetButtonDown(map);

        //Debug Tools
        if (Input.GetKeyDown(debugButton))
        {
            ChangeTiles();
        }
        if (Input.GetKeyDown(instaFood))
        {
            food = 9999;
        }

        //Mode Swap Logic and UI
        if ((modeInput == true) && (combatModeCheck == false) && (buildModeCheck == true))
        {
            buildModeCheck = false;
            combatModeCheck = true;
            modeIcon.sprite = combatModeIcon;
            southIcon.sprite = projectileIcon;
            westIcon.sprite = combatmodeIcon2;
            eastIcon.sprite = landmineIcon;
            northIcon.sprite = combatModeIcon4;
            Debug.Log("Combat Mode");
        }
        else if ((modeInput == true) && (combatModeCheck == true) && (buildModeCheck == false))
        {
            buildModeCheck = true;
            combatModeCheck = false;
            modeIcon.sprite = buildModeIcon;
            southIcon.sprite = farmIcon;
            westIcon.sprite = mineIcon;
            eastIcon.sprite = quarryIcon;
            northIcon.sprite = flagIcon;
            Debug.Log("Build Mode");
        }
       
        //Mode Swapped Funcions
        if (combatModeCheck == false)
        {
            if (foodCollectorPress == true)
            {
                Debug.Log("Farm Button Pressed");
                buildingController.PlaceFoodCollector();
                             
            }
            if (quarryInput == true)
            {
                buildingController.PlaceStoneCollector();
            }
            if (mineInput == true)
            {
                buildingController.PlaceGoldCollector();
            }
            if (flagInput == true)
            {
                buildingController.PlaceFlag();
            }
        }
        if (combatModeCheck == true)
        {            
            if (fireInput == true)
            {
                combatMode.FireProjectile();
            }
            if (landmineInput)
            {
                combatMode.SpawnMine();
            }
        }
        
        if (sprintInput == true)
        {
            speed = sprintSpeed;
            food -= sprintFoodUseRate * Time.deltaTime;
        }
        if (sprintInput != true)
        {
            speed = playerSpeed;
        }
        if (wallInput == true)
        {
            buildingController.PlaceWall();
        }
        if (mapInput == true)
        {
            if (bigMap.gameObject.activeSelf)
            {
                bigMap.gameObject.SetActive(false);
            }
            else
            {
                bigMap.gameObject.SetActive(true);
            }
            
        }
       


        if (hungerOn == true)
        {
            // Reduce food over time at the specified rate
            food -= foodUseRate * Time.deltaTime;

            // Clamp food to stay within a reasonable range (0 to 100, for example)
            food = Mathf.Clamp(food, resourceMin, foodMax);
            if (food <= foodFlashTextAmount)
            {
                foodText.color = Color.red;
            }
            else if (food > foodFlashTextAmount)
            {
                foodText.color = Color.black;
            }
            if ((food <= 0f) && (!isRespawning))
            {
                Starve();
            }
        }
        else if (!hungerOn)
        {
            foodText.color = Color.black;
        }

        // Set the movement direction based on player input
        Vector3 inputDirection = new Vector3(horizontalInput, verticalInput, 0);
        Vector3 relativeDirection = playerCamera.transform.TransformDirection(inputDirection);
        moveDirection = relativeDirection.normalized;

        // Rotate the snake to face the movement direction with sharp turns
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (isRespawning)
        {       
            timer += Time.deltaTime;
            if (timer >= respawnTimer)
            {
                Debug.Log(gameObject.name + " is respawning.");
                Respawn();
            }
        }
        if (health <= 0)
        {
            Starve();
            health = gameController.startingHealth;
        }        
    }

   

   
    public void CheckTileCollision(Vector2 worldPosition)
    {
        snakeEntryPOS = tilemap.WorldToCell(worldPosition);
        entryTile = tilemap.GetTile(snakeEntryPOS);

        if (entryTile != null)
        {

            entryTileColour = tilemap.GetColor(snakeEntryPOS);

            if (entryTileColour != playerColour)
            {
                if (entryTileColour == baseTileColour)
                {
                    activeTailSize = normalTailSize;
                    CreateSnakeTile();
                }               
                if (entryTileColour == otherSnakeColour)
                {
                    occupiedTiles.Add(snakeEntryPOS);
                    otherPlayer.UpdateUI();
                    activeTailSize = invadingTailSize;
                    CreateSnakeTile();
                }
                if (entryTileColour == otherSnakeTrailColour)
                {
                    activeTailSize = normalTailSize;
                    if (otherPlayer.isRespawning == false)
                    {
                        otherPlayer.Starve();
                    }
                    
                    CreateSnakeTile();
                }
                
            }
            if (entryTileColour == playerColour)
            {               
                activeTailSize = normalTailSize;                
                hungerOn = false;                
                ChangeTiles();
               
            }


        }
        else if (entryTile == null)
        {
            Debug.LogError("Player is off Map");
        }
    }
    public void UpdateUI()
    {
        healthSlider.value = health / 100;
        foodText.text = food.ToString("F0");
        stoneText.text = stone.ToString("F0");
        goldText.text = gold.ToString("F0");
        playerScoreText.text = playerScore.ToString();
        
    }    
    public void ChangeTiles()
    {
        foreach (Vector3Int tilePosition in tilesToChange)
        {            
            tilemap.SetColor(tilePosition, playerColour);
            playerTiles.Add(tilePosition);            
            occupiedTiles.RemoveAll(Vector3Int => otherPlayer.playerTiles.Contains(Vector3Int));
            otherPlayer.UpdateUI();
            UpdateUI();

        }
        tilesToChange.Clear();
        occupiedTiles.Clear();        
    }

    public void CreateSnakeTile()
    {
        tilesToChange.Add(snakeEntryPOS);
        hungerOn = true;
        tilemap.SetTileFlags(snakeEntryPOS, TileFlags.None);
        tilemap.SetColor(snakeEntryPOS, snakeTrailColour);
        FatSnakeMaker(snakeEntryPOS);

    }
    public void FatSnakeMaker(Vector3Int surroundingTilePOS)
    {
        for (int xOffset = -activeTailSize; xOffset <= activeTailSize; xOffset++)
        {
            for (int yOffset = -activeTailSize; yOffset <= activeTailSize; yOffset++)
            {
                if (xOffset == 0 && yOffset == 0) // Skip the center tile (current tile)
                    continue;

                int surroundingX = surroundingTilePOS.x + xOffset;
                int surroundingY = surroundingTilePOS.y + yOffset;
                int surroundingZ = surroundingTilePOS.z;

                //Adds tiles to the following foreach loop
                surroundingTiles.Add(new Vector3Int(surroundingX, surroundingY, surroundingZ));

                //Adds tiles to list to change to player colour
                tilesToChange.Add(new Vector3Int(surroundingX, surroundingY, surroundingZ));
            }
        }
        foreach(Vector3Int tilePOS in surroundingTiles)
        {
            surroundingTileColour = tilemap.GetColor(tilePOS);
            if (surroundingTileColour == playerColour)
            {
               
            }
            if (surroundingTileColour == snakeTrailColour)
            {
                tilemap.SetTileFlags(tilePOS, TileFlags.None);
                tilemap.SetColor(tilePOS, snakeTrailColour);
            }
            if (surroundingTileColour == otherSnakeColour)
            {
                tilemap.SetTileFlags(tilePOS, TileFlags.None);
                tilemap.SetColor(tilePOS, snakeTrailColour);
            }
            if (surroundingTileColour == otherSnakeTrailColour)
            {
                tilemap.SetTileFlags(tilePOS, TileFlags.None);
                tilemap.SetColor(tilePOS, snakeTrailColour);
            }
            
            else if (surroundingTileColour == baseTileColour)
            {
                tilemap.SetTileFlags(tilePOS, TileFlags.None);
                tilemap.SetColor(tilePOS, snakeTrailColour);
            }            
           
        }
        surroundingTiles.Clear();

    }
    public void Starve()
    {
        Debug.Log(gameObject.name + " has died.");
        audioSource.clip = deathSound;
        audioSource.Play();
        timer = 0;        
        foreach (Vector3Int tilePosition in tilesToChange)
        {
            if (!playerTiles.Contains(tilePosition))
            {
                tilemap.SetColor(tilePosition, baseTileColour);
            }          
            
        }       
        foreach(Vector3Int __tilePosition in occupiedTiles)
        {
            if (!playerTiles.Contains(__tilePosition))
            {
                tilemap.SetColor(__tilePosition, otherSnakeColour);
            }
            
        }

        tilesToChange.Clear();
        surroundingTiles.Clear();
        occupiedTiles.Clear();
        if (buildingController.flags.Count != 0)
        {
            isRespawning = true;
            closestFlag = null;
            float closestDistance = Mathf.Infinity;
            foreach (GameObject flag in buildingController.flags)
            {
                float distance = Vector3.Distance(gameObject.transform.position, flag.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFlag = flag;
                }
                
            }           
            rb.isKinematic = true;
            spriteRenderer.enabled = false;
            snakeCollider.enabled = false;           
        }
        else if (buildingController.flags.Count == 0)
        {
            Defeat();
        }
           

    }
    public void Respawn()
    {
        isRespawning = false;
        rb.isKinematic = false;
        spriteRenderer.enabled = true;
        snakeCollider.enabled = true;
        Debug.Log(gameObject.name + " has respawned.");
        hungerOn = false;
        food = 10;
        health = gameController.startingHealth;
        gameObject.transform.position = closestFlag.transform.position;
       
        
    }
    public void Defeat()
    {
        gameController.defeatedPlayer = gameObject.GetComponent<MovementController>();
        gameController.DefeatEndGame();
    }
  
   
}


    










  



