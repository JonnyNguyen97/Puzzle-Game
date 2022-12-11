using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Set size board")]
    [SerializeField] int widthBoard = 8;
    [SerializeField] int heightBoard = 8;

    [Header("General")]
    [SerializeField] GameObject cellBGPrefab;
    [SerializeField] int numberTypeGem;
    [SerializeField] Gem[] gemTypeList;
    [SerializeField] Gem[,] allGemBoard;
    int clickCount = 0;
    Dictionary<string, Gem> highlightGemsList;
    [SerializeField] Gem firstGem;
    //Move speed gem when they swapping
    [SerializeField] float swappingSpeed = 1f;  
    //Gem Checker 
    Checker checker;
    

    //Prevent player input when gems are moving
    public enum BoarState {waiting, moving};
    public BoarState currentState = BoarState.waiting;

    //Create bomb item
    [Header("Bomb item")]
    [SerializeField] Gem bomb;
    public float bombChance = 2f;
    bool bombAppear = false;
    Vector2Int posBroken;
    Gem currentGemBroken;
    int brokenContinueCount = 0;
    int matchedColumn = -1;
    int matchedRow = -1;

    [Header("Score")]
    ScoreKeeper scoreKeeper;

    void Awake()
    {
        checker = FindObjectOfType<Checker>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initializaton allGemListBoard 2D array
        allGemBoard = new Gem[widthBoard, heightBoard];
        //Initialization board game
        Initialization();
        checker.CollectMatchedGem();
        //Debug.Log("Current position gem broken is"+ posBroken.x + ", " + posBroken.y);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }
    }

    //Create Board Game
    private void Initialization()
    {
        for (int x = 0; x < widthBoard; x ++)
        {
            for (int y = 0; y < heightBoard; y++)
            {
                //Create position of cell
                Vector2 posCell = new Vector2(x, y);
                
                //Create cell with cordinate x, y
                GameObject instance = Instantiate(cellBGPrefab, posCell, Quaternion.identity);
                
                //Set name with cordinate for the cell background
                instance.name = "CellBG_" + x + "_" + y;
                
                //Put cell object into the parent object on hierarchy
                instance.transform.parent = transform;
                
                //Get random index of gem List
                //int randomIndexGem = Random.Range(0, gemTypeList.Length);
                int randomIndexGem = Random.Range(0, Mathf.Clamp(numberTypeGem, 0, gemTypeList.Length));
                
                //Check gems match when initial game
                //Debug.Log("Random index is"+ randomIndexGem);
                int interations = 0;
                while(isMatchWhenStart(new Vector2Int(x, y), gemTypeList[randomIndexGem]) && interations < 100)
                {
                    randomIndexGem = Random.Range(0, Mathf.Clamp(numberTypeGem, 0, gemTypeList.Length));
                    interations++;
                    //Debug.Log($"interation:{interations}, randomIndex:{randomIndexGem}, cordinate: {x},{y}");
                    //break;
                }

                //Spawn gem by random index
                GemSpawner(new Vector2Int(x, y), randomIndexGem);
            }
        }
    }

    bool isMatchWhenStart(Vector2Int posCheck, Gem gemCheck)
    {
        //Check rows
        if (posCheck.x >= 2)
        {
            //If leftofleftGemCheck  - leftgemCheck - GemCheck have the same type when initial game
            if (allGemBoard[posCheck.x - 1, posCheck.y].GetGemType() == gemCheck.GetGemType()
            && allGemBoard[posCheck.x - 2, posCheck.y].GetGemType() == gemCheck.GetGemType())
            {
                return true;
            }
        }

        //If bottomOfbottomgemCheck - bottomgemCheck - gemcheck have the same type when initial game
        if (posCheck.y >= 2)
        {
            if (allGemBoard[posCheck.x, posCheck.y - 1].GetGemType() == gemCheck.GetGemType()
            && allGemBoard[posCheck.x, posCheck.y - 2].GetGemType() == gemCheck.GetGemType())
            {
                return true;
            }
        }

        return false;
    }

    private void GemSpawner(Vector2Int posInt, int indexGem)
    {
        //Create gem - At postInt.y + heighboard to sliding from above to the board
        Gem instGem = Instantiate(gemTypeList[indexGem]
                                        , new Vector3 (posInt.x, posInt.y + heightBoard, 0)
                                        , Quaternion.identity);
        //Put gem object into the parent object on hierarchy
        instGem.transform.parent = this.transform;
        
        //Set name with cordinate for the gem
        instGem.name = "Gem_" + posInt.x + "_" + posInt.y;
        
        //Adding gem was created in allGemListBoard
        allGemBoard[posInt.x, posInt.y] = instGem;

        //Setup inital value of gem was created
        instGem.SetupGem(posInt, this, widthBoard, heightBoard);
    }

    public Gem GetGemOnBoard(int x, int y)
    {
        return allGemBoard[x, y];
    }

    public int GetClickCount()
    {
        return clickCount;
    }

    public void RSClickCount()
    {
        clickCount = 0;
    }

    public void IncrClickCount()
    {
        clickCount++;
    }

    public void InitHLGemsList()
    {
        highlightGemsList = new Dictionary<string, Gem>();
        highlightGemsList.Add("left", null);
        highlightGemsList.Add("right", null);
        highlightGemsList.Add("bottom", null);
        highlightGemsList.Add("top", null);
    }

    public Dictionary<string, Gem> GetHLGemsList()
    {
        return highlightGemsList;
    }

    public void SetHLGemsList(string dir, Gem gemdir)
    {
        highlightGemsList[dir] = gemdir;
    }

    public void SetFirstChosenGem(Gem firstChosenGem)
    {
        firstGem = firstChosenGem;
    }

    public void SetNullFirstChosenGem()
    {
        firstGem = null;
    }

    public float GetSwappingSpeed()
    {
        return swappingSpeed;
    }
    
    public Vector2Int GetSizeBoard()
    {
        return new Vector2Int(widthBoard, heightBoard);
    }
    public void ClearHLGemsList()
    {
        if (highlightGemsList != null)
            highlightGemsList.Clear();
    }

    //Blur all gems when user first click one of gems in board
    private void BlurAllGems()
    {
        for (int x = 0; x < widthBoard; x ++)
        {
            for (int y = 0; y < heightBoard; y++)
            {
                if (GetGemOnBoard(x, y) != null)
                {
                    SpriteRenderer spRenderBlur = GetGemOnBoard(x, y).GetComponent<SpriteRenderer>();
                    spRenderBlur.color = new Color(spRenderBlur.color.r, spRenderBlur.color.g
                                                    , spRenderBlur.color.b, 0.14f);
                }
            }
        }
    }

    // De-blur all gems when user second click one of gems in board
    private void DeBlurAllGems()
    {
        for (int x = 0; x < widthBoard; x ++)
        {
            for (int y = 0; y < heightBoard; y++)
            {
                if (GetGemOnBoard(x, y) != null)
                {
                    SpriteRenderer spRenderBlur = GetGemOnBoard(x, y).GetComponent<SpriteRenderer>();
                    spRenderBlur.color = new Color(spRenderBlur.color.r, spRenderBlur.color.g
                                                    , spRenderBlur.color.b, 1f);
                }
            }
        }
    }

    // Hight light gems around that gem was first clicked by user
    private void HightLightGems(int x, int y)
    {
        //Hight light gem was clicked
        SpriteRenderer spRenderClicked = GetGemOnBoard(x, y).GetComponent<SpriteRenderer>();
        spRenderClicked.color = new Color(spRenderClicked.color.r, spRenderClicked.color.g
                                        , spRenderClicked.color.b, 1f);

        //Hight light & Scale In gems on list
        foreach(Gem child in GetHLGemsList().Values)
        {
            if (child != null)
            {
                SpriteRenderer spRenderHL = child.GetComponent<SpriteRenderer>();
                spRenderHL.color = new Color(spRenderHL.color.r, spRenderHL.color.g, spRenderHL.color.b, 1f);
                Vector3 addScale = child.transform.localScale * 0.25f;

                child.transform.localScale += addScale;
            }
        }
    }

    //De highlight gems on board
    private void DeHightLightGems()
    {
        if (GetHLGemsList() != null)
        {
            foreach(Gem child in GetHLGemsList().Values)
            {
                if (child != null)
                {
                    Vector3 addScale = child.transform.localScale * 0.2f;
                    child.transform.localScale -= addScale;
                }
            }
        }
    }

    public void SetGems(int x, int y)
    {
        BlurAllGems();
        HightLightGems(x, y);
    }

    public void RSGemsOnBoard()
    {
        DeHightLightGems();
        DeBlurAllGems();
        ClearHLGemsList();
    }

    public void SwappingGem(Gem secondGem)
    {
        //Set state of Board when gems are moving
        currentState = BoarState.moving;
        //Debug.Log("State board is moving");

        //Change the position, value of two after we click choose second gem
        ChangeValueTwoGem(firstGem, secondGem);
        
        //We need use Coroutine to waiting time 2 gems swap position
        StartCoroutine(CheckerCoroutine(firstGem, secondGem));
    }

    IEnumerator CheckerCoroutine(Gem gem1, Gem gem2)
    {
        //Firstly, we need check 2 two gem were swapped
        checker.CollectMatchedGem();

        //Wait after 0.5s -> Then check they are match or not
        yield return new WaitForSeconds(0.35f);

        //If they not match -> return previou value!
        if (!gem1.GetIsMatch() && !gem2.GetIsMatch())
        {
            ChangeValueTwoGem(gem1, gem2);
            yield return new WaitForSeconds(0.35f);
            currentState = BoarState.waiting;
            //Debug.Log("1 -> State board is waiting");
        }
        else
            DestroyCurrentGemMatches();

        
    }

    private void ChangeValueTwoGem(Gem gemA, Gem gemB)
    {
        //Firstly, we need save values of gemA
        string nameOfGemA= gemA.name;
        Vector2Int posIntGemA = gemA.GetPostIntGem();
        Vector3 posFirst = gemA.transform.position;

        //Save name value for gamB
        string nameofGemB = gemB.name;
        gemB.name = "Gemsecond";

        //Gem A change values
        gemA.name = nameofGemB;
        gemA.SetPostInstGem(gemB.GetPostIntGem());
        allGemBoard[gemB.GetPostIntGem().x, gemB.GetPostIntGem().y] = gemA;

        //Gem B change values
        gemB.name = nameOfGemA;
        gemB.SetPostInstGem(posIntGemA);
        allGemBoard[posIntGemA.x, posIntGemA.y] = gemB;
    }

    public void DestroyMatchedGemAtPos(Vector2Int pos)
    {
        if (allGemBoard[pos.x, pos.y] != null)
        {
            if (allGemBoard[pos.x, pos.y].GetIsMatch())
            {
                Gem gemDestroy = allGemBoard[pos.x, pos.y];

                //Debug.Log("Current position gem broken is"+ pos.x + ", " + pos.y);
                CheckMoreThan4BlockSameType(pos, gemDestroy);
 
                if (bombAppear)
                {
                    bombAppear = false;
                    CreateBomb(pos, gemDestroy);
                }
                else
                {
                    Destroy(gemDestroy.gameObject);
                    allGemBoard[pos.x, pos.y] = null;
                }

                //
                scoreKeeper.IncrScore(scoreKeeper.GetAddingNormal());

                ParticleSystem instEffect = Instantiate(gemDestroy.GetBrokenParticle()
                                                    , gemDestroy.transform.position
                                                    , Quaternion.identity);
                //Destroy(instEffect, gemDestroy.GetBrokenParticle().main.duration + 2f);
            }
        }
    }

    public void CheckMoreThan4BlockSameType(Vector2Int pos, Gem matchedgem)
    {
        if (currentGemBroken == null)
        {
            currentGemBroken = matchedgem;
            posBroken = pos;

            //If column has matched gem
            if (matchedColumn != posBroken.x && posBroken.y < heightBoard - 1)
            {
                matchedColumn = posBroken.x;
                //If column has matched gem
                if (allGemBoard[matchedColumn, posBroken.y + 1].GetIsMatch())
                {
                    for (int i = posBroken.y; i < heightBoard; i++)
                    {
                        if (allGemBoard[matchedColumn, i].GetIsMatch())
                        {
                            if (allGemBoard[matchedColumn, i].GetGemType() == currentGemBroken.GetGemType())
                            {
                                brokenContinueCount++;
                                Debug.Log($"Matched column: {matchedColumn}, {i} ->>>>>>>>> with count {brokenContinueCount}");
                            }
                        }
                    }
                }
            }

            //If row has matched gem
            if (matchedRow != posBroken.y && posBroken.x < widthBoard - 1)
            {
                matchedRow = posBroken.y;
                //If row has matched gem
                if (allGemBoard[posBroken.x + 1, matchedRow].GetIsMatch())
                {
                    for (int i = posBroken.x; i < widthBoard; i++)
                    {
                        if (allGemBoard[i, matchedRow].GetIsMatch())
                        {
                            if (allGemBoard[i, matchedRow].GetGemType() == currentGemBroken.GetGemType())
                            {
                                brokenContinueCount++;
                                Debug.Log($"Matched row: {i}, {matchedRow} ->>>>>>>>> with count {brokenContinueCount}");
                            }
                        }
                    }
                }
            }

            
        }
        if (brokenContinueCount > 3)
        {
            bombAppear = true;
            brokenContinueCount = 1;
        }
    }

    public void RSCurrentGemBroken()
    {
        currentGemBroken = null;
        posBroken = new Vector2Int();
        brokenContinueCount = 0;
        matchedColumn = -1;
        matchedRow = -1;
        Debug.Log("Reset broken gem@!!!");
    }

    private void CreateBomb(Vector2Int pos, Gem gemRemove)
    {
        Destroy(gemRemove.gameObject);
        //Create bomb
        Gem instBomb = Instantiate(bomb, new Vector3 (pos.x, pos.y, 0)
                                        , Quaternion.identity);
        //Put gem object into the parent object on hierarchy
        instBomb.transform.parent = this.transform;
        
        //Set name with cordinate for the gem
        instBomb.name = "Gem_" + pos.x + "_" + pos.y;
        
        //Adding gem was created in allGemListBoard
        allGemBoard[pos.x, pos.y] = instBomb;
        //Debug.Log("Is bomb ->" + allGemBoard[pos.x, pos.y].GetGemType());

        //Setup inital value of gem was created
        instBomb.SetupGem(pos, this, widthBoard, heightBoard);
    }

    public void DestroyCurrentGemMatches()
    {
        //Get all gem match
        for (int i = 0; i < checker.GetCurrentGemsMatchCount(); i++)
        {
            if (checker.GetGemMatch(i) != null)
            {
                currentState = BoarState.moving;
                //Destroy gem by dertermine position
                DestroyMatchedGemAtPos(checker.GetGemMatch(i).GetPostIntGem());
            }
        }
        checker.ClearGemMatch();
        RSCurrentGemBroken();
        
        //Use start coroutine for fall gems
        StartCoroutine(FallGems());
    }

    private IEnumerator FallGems()
    {
        //They will be fall after 0.1s
        yield return new WaitForSeconds(0.1f);
        int nullCount = 0;
        for (int x = 0; x < widthBoard; x ++)
        {
            for (int y = 0; y < heightBoard; y++)
            {
                if (allGemBoard[x,y] == null)
                    nullCount++;
                else if (nullCount > 0)
                {
                    //Set position gem -> position null
                    allGemBoard[x, y].DecrPost_Y(nullCount);

                    //Replace index of gem -> index of null in list gem
                    allGemBoard[x, y - nullCount] = allGemBoard[x, y];

                    //Set new name for gem was fell
                    allGemBoard[x, y - nullCount].name = "Gem_" + x + "_" + (y -nullCount);
                    
                    //Set null object when it's done!
                    allGemBoard[x, y] = null;
                }
            }
            //Reset null when next column
            nullCount = 0;
        }

        StartCoroutine(FillBoard());
    }

    private IEnumerator FillBoard()
    {
        //Refill gems in board after 0.3s
        yield return new WaitForSeconds(0.3f);
        ReFillGemInBoard();

        //Collect matched gems in board to listmatch after 0.3s
        yield return new WaitForSeconds(0.3f);
        checker.CollectMatchedGem();

        //If listmatch has matched gems -> Destroy them were fell after 0.7s
        if (checker.GetCurrentGemsMatchCount() > 0)
        {
            yield return new WaitForSeconds(0.7f);
            DestroyCurrentGemMatches();
        }
        else
        {
            yield return new WaitForSeconds(0.35f);
            currentState = BoarState.waiting;
            Debug.Log("2 -> State board is waiting");
        }

    }

    private void ReFillGemInBoard()
    {
        for (int x = 0; x < widthBoard; x ++)
        {
            for (int y = 0; y < heightBoard; y++)
            {
                if (allGemBoard[x, y] == null)
                {
                    //Get random index
                    int randomIndexG = Random.Range(0, Mathf.Clamp(numberTypeGem, 0, gemTypeList.Length));
                    //Spawn gem by random index
                    GemSpawner(new Vector2Int(x, y), randomIndexG);
                }
            }
        }

    }

    public void ShuffleBoard()
    {

        if (currentState == BoarState.waiting)
        {
            //Reset click count, hight gem if first gem is choosing by player
            RSClickCount();
            RSGemsOnBoard();
            
            currentState = BoarState.moving;

            //Firsty, we create a copy gem on Board
            List<Gem> gemsFromBoard = new List<Gem>();
            for (int x = 0; x < widthBoard; x ++)
            {
                for (int y = 0; y < heightBoard; y++)
                {
                    gemsFromBoard.Add(allGemBoard[x, y]);
                    allGemBoard[x, y] = null;
                }
            }

            //Second, we get random index a copy board insert to original board
            for (int x = 0; x < widthBoard; x ++)
            {
                for (int y = 0; y < heightBoard; y++)
                {
                    //Prvent matched gem when shuffing
                    int interations = 0;
                    int randomIndexNewGem = Random.Range(0, gemsFromBoard.Count);
                    while(isMatchWhenStart(new Vector2Int(x, y),  gemsFromBoard[randomIndexNewGem]) 
                    && interations < 100 && gemsFromBoard.Count > 1)
                    {
                        randomIndexNewGem = Random.Range(0, gemsFromBoard.Count);
                        interations++;
                    }

                    //Resetup new position
                    gemsFromBoard[randomIndexNewGem].SetupGem(new Vector2Int(x, y), this, widthBoard, heightBoard);
                    allGemBoard[x, y] = gemsFromBoard[randomIndexNewGem];

                    //Remove index of gemsFromBoard just add to original board
                    gemsFromBoard.RemoveAt(randomIndexNewGem);
                }
            }
        }
        //Waiting a second to change state of board
        StartCoroutine(SetWaitingState());
    }

    IEnumerator SetWaitingState()
    {
        yield return new WaitForSeconds(0.5f);
        currentState = BoarState.waiting;
    }
}
