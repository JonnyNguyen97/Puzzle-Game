using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    //[HideInInspector]
    Vector2Int posInt;
    //[HideInInspector]
    Board board;

    int widthBoard, heightBoard;
    public enum GemType {Blue, Green, Orange, Pink, Red, Turquoise, Yellow, Bomb};
    [SerializeField] GemType type;

    [SerializeField] bool isMatch = false;

    [SerializeField] ParticleSystem brokenGemEffect;

    [SerializeField] int blastRadius = 1;

    // Update is called once per frame
    void Update()
    {
        if (transform.position != new Vector3(posInt.x, posInt.y, 0))
            transform.position = Vector2.Lerp(transform.position, posInt, board.GetSwappingSpeed() * Time.deltaTime);
    }

    public void SetupGem(Vector2Int argPos, Board argBoard, int withB, int heighB)
    {
        posInt = argPos;
        board = argBoard;
        widthBoard = withB;
        heightBoard = heighB;
    }

    private void OnMouseDown()
    {
        if (board.currentState == Board.BoarState.waiting)
        {
            board.IncrClickCount();
            //Debug.Log("With click count " + board.GetClickCount());
            
            if (board.GetClickCount() == 1)
            {
                //Debug.Log("Press - " + name + "Cordinate" + posInt.x + ", " + posInt.y);
                board.SetFirstChosenGem(this);
                board.InitHLGemsList();

                //###################################################
                //##  We define cordinate x = width && y = height  ##
                //###################################################
                //If we click one of the leftmost row cells
                //      _______
                //     |.(3)   |
                //  => |.(2)   |
                //     |.(1)___|
                if (posInt.x == 0)
                {
                    //(1) If we click bottom left corner
                    if (posInt.y == 0)
                    {
                        //Debug.Log("0,0");
                        board.SetHLGemsList("right", board.GetGemOnBoard(posInt.x + 1, posInt.y));
                        board.SetHLGemsList("top", board.GetGemOnBoard(posInt.x, posInt.y + 1));
                    }
                    //(3) If we click top left corner
                    else if (posInt.y == heightBoard - 1)
                    {
                        //Debug.Log("0,->");
                        board.SetHLGemsList("right", board.GetGemOnBoard(posInt.x + 1, posInt.y));
                        board.SetHLGemsList("bottom", board.GetGemOnBoard(posInt.x, posInt.y - 1));
                    }
                    //(2) If we click other left corner
                    else if (posInt.y > 0 && posInt.y < heightBoard - 1)
                    {
                        //Debug.Log("0,y");
                        board.SetHLGemsList("right", board.GetGemOnBoard(posInt.x + 1, posInt.y));
                        board.SetHLGemsList("top", board.GetGemOnBoard(posInt.x, posInt.y + 1));
                        board.SetHLGemsList("bottom", board.GetGemOnBoard(posInt.x, posInt.y - 1));
                    }
                }  
                
                //If we click one of the rightmost row cells
                //      _______
                //     |___(4).|
                //     |___(5).|  <=
                //     |___(6).|
                else if (posInt.x == widthBoard - 1)
                {
                    //Debug.Log($"{widthBoard - 1},y");
                    //(4) If we click top right corner
                    if (posInt.y == heightBoard - 1)
                    {
                        board.SetHLGemsList("left", board.GetGemOnBoard(posInt.x - 1, posInt.y));
                        board.SetHLGemsList("bottom", board.GetGemOnBoard(posInt.x, posInt.y - 1));
                    }

                    //(6) If we click bottom right corner
                    else if (posInt.y == 0)
                    {
                        board.SetHLGemsList("left", board.GetGemOnBoard(posInt.x - 1, posInt.y));
                        board.SetHLGemsList("top", board.GetGemOnBoard(posInt.x, posInt.y + 1));
                    }

                    //(5) If we click  other right corner
                    else if (posInt.y > 0 && posInt.y < heightBoard - 1)
                    {
                        board.SetHLGemsList("left", board.GetGemOnBoard(posInt.x - 1, posInt.y));
                        board.SetHLGemsList("top", board.GetGemOnBoard(posInt.x, posInt.y + 1));
                        board.SetHLGemsList("bottom", board.GetGemOnBoard(posInt.x, posInt.y - 1));
                    }
                }

                // If we click one of the bottommost row cells
                //      _______
                //     |       |
                //     |  (7)  |
                //     |___.___|
                //         ^
                //         |
                else if (posInt.y == 0 && posInt.x > 0 && posInt.x < widthBoard - 1)
                {
                    board.SetHLGemsList("left", board.GetGemOnBoard(posInt.x - 1, posInt.y));
                    board.SetHLGemsList("right", board.GetGemOnBoard(posInt.x + 1, posInt.y));
                    board.SetHLGemsList("top", board.GetGemOnBoard(posInt.x, posInt.y + 1));
                }

                // If we click one of the topmost row cells
                //         |
                //         v
                //      ___.___
                //     |  (8)  |
                //     |       |
                //     |_______|
                else if (posInt.y == heightBoard - 1 && posInt.x > 0 && posInt.x < widthBoard - 1)
                {
                    board.SetHLGemsList("left", board.GetGemOnBoard(posInt.x - 1, posInt.y));
                    board.SetHLGemsList("right", board.GetGemOnBoard(posInt.x + 1, posInt.y));
                    board.SetHLGemsList("bottom", board.GetGemOnBoard(posInt.x, posInt.y - 1));
                }

                // If we click one of the inside cell of board
                //         |
                //      ___|___
                //     |   v   |
                //     |  (9)  |
                //     |_______|
                else
                {
                    board.SetHLGemsList("left", board.GetGemOnBoard(posInt.x - 1, posInt.y));
                    board.SetHLGemsList("right", board.GetGemOnBoard(posInt.x + 1, posInt.y));
                    board.SetHLGemsList("top", board.GetGemOnBoard(posInt.x, posInt.y + 1));
                    board.SetHLGemsList("bottom", board.GetGemOnBoard(posInt.x, posInt.y - 1));
                }

                //Blur and highlight gems on board 
                board.SetGems(posInt.x, posInt.y);
            }
            else if (board.GetClickCount() == 2)
            {
                //Debug.Log("Press - " + name + "Cordinate" + posInt.x + ", " + posInt.y);
                //We need check the second gem was clicked that was on gem list hightlight or not!
                CheckAroundGemsList();
                //Then reset position gem in gem list on the board
                board.RSGemsOnBoard();
                //Set click count = 0
                board.RSClickCount();
            }
        }
    }

    public Vector2Int GetPostIntGem()
    {
        return posInt;
    }

    public void DecrPost_Y(int value)
    {
        posInt.y -= value;
    }

    public void SetPostInstGem(Vector2Int argPostInt)
    {
        posInt = argPostInt;
    }

    public GemType GetGemType()
    {
        return type;
    }

    public void SetIsMatch(bool val)
    {
        isMatch = val;
    }

    public bool GetIsMatch()
    {
        return isMatch;
    }

    public ParticleSystem GetBrokenParticle()
    {
        return brokenGemEffect;
    }

    public int GetBlastRadius()
    {
        return blastRadius;
    }

    public void CheckAroundGemsList()
    {
        foreach(Gem secondChosenGem in board.GetHLGemsList().Values)
        {
            if (secondChosenGem != null)
            {
                //If second gem was clicked that was on gem list hightlight
                if (posInt == secondChosenGem.GetPostIntGem())
                {
                    //Swapping it!
                    board.SwappingGem(secondChosenGem);
                }
            }
        }
    }
}
