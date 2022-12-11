using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Checker : MonoBehaviour
{
    Board board;
    [SerializeField] List<Gem> currentGemsMatch = new List<Gem>();


    void Awake()
    {
        board = FindObjectOfType<Board>();
    }

    public int GetCurrentGemsMatchCount()
    {
        return currentGemsMatch.Count;
    }

    public Gem GetGemMatch(int index)
    {
        return currentGemsMatch[index];
    }

    public void ClearGemMatch()
    {
        currentGemsMatch.Clear();
    }

    public void CollectMatchedGem()
    {
        //Get width & height of board game
        int widthBoard = board.GetSizeBoard().x;
        int heightBoard = board.GetSizeBoard().y;
        for (int x = 0; x < widthBoard; x ++)
        {
            for (int y = 0; y < heightBoard; y++)
            {
                //Get gem on Board with cordinate x,y
                Gem currentGem = board.GetGemOnBoard(x, y);
                if (currentGem != null)
                {
                    //Check rows
                    if (x > 0 && x < widthBoard - 1)
                    {
                       Gem leftGem = board.GetGemOnBoard(x - 1, y);
                       Gem rightGem = board.GetGemOnBoard(x + 1, y);
                       if (leftGem != null && rightGem != null)
                       {
                            //If they are have a same type
                            if(leftGem.GetGemType() == currentGem.GetGemType() 
                            && currentGem.GetGemType() == rightGem.GetGemType())
                            {
                                leftGem.SetIsMatch(true);
                                currentGem.SetIsMatch(true);
                                rightGem.SetIsMatch(true);

                                //Add gems match to the list matched gem
                                currentGemsMatch.Add(leftGem);
                                currentGemsMatch.Add(currentGem);
                                currentGemsMatch.Add(rightGem);
                            }
                       }
                    }

                    //Check columm
                    if (y > 0 && y < heightBoard - 1)
                    {
                       Gem bottomGem = board.GetGemOnBoard(x, y - 1);
                       Gem topGem = board.GetGemOnBoard(x, y + 1);
                       if (bottomGem != null && topGem != null)
                       {
                            //If they are have a same type
                            if(bottomGem.GetGemType() == currentGem.GetGemType() 
                            && currentGem.GetGemType() == topGem.GetGemType())
                            {
                                bottomGem.SetIsMatch(true);
                                currentGem.SetIsMatch(true);
                                topGem.SetIsMatch(true);

                                //Add gems match to the list matched gem
                                currentGemsMatch.Add(bottomGem);
                                currentGemsMatch.Add(currentGem);
                                currentGemsMatch.Add(topGem);
                            }
                       }
                    }

                }
            }
        }
        
        //Make sure elements in list are not duplicate
        if (currentGemsMatch.Count > 0)
            currentGemsMatch = currentGemsMatch.Distinct().ToList();

        CheckForBombs();
    }

    public void CheckForBombs()
    {
        for (int i = 0; i < currentGemsMatch.Count; i++)
        {
            int x = currentGemsMatch[i].GetPostIntGem().x;
            int y = currentGemsMatch[i].GetPostIntGem().y;

            //If matched gem has cordinate X > 0 -> CHECK TOP MATCHED   GEM
            if (x > 0)
            {
                if (board.GetGemOnBoard(x - 1, y) != null)
                {
                    if (board.GetGemOnBoard(x - 1, y).GetGemType() == Gem.GemType.Bomb)
                    {
                        MarkBombArea(new Vector2Int(x-1, y), board.GetGemOnBoard(x-1, y));
                    }
                }
            }

            //If matched gem has cordinate X < with board - 1 -> CHECK BOTTOM MATCHED GEM
            if (x < board.GetSizeBoard().x - 1)
            {
                if (board.GetGemOnBoard(x + 1, y) != null)
                {
                    if (board.GetGemOnBoard(x + 1, y).GetGemType() == Gem.GemType.Bomb)
                    {
                        MarkBombArea(new Vector2Int(x + 1, y), board.GetGemOnBoard(x + 1, y));
                    }
                }
            }

            //If matched gem has cordinate Y > 0 -> CHECK BOTTOM MATCHED GEM
            if (y > 0)
            {
                if (board.GetGemOnBoard(x, y - 1) != null)
                {
                    if (board.GetGemOnBoard(x, y - 1).GetGemType() == Gem.GemType.Bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y - 1), board.GetGemOnBoard(x, y - 1));
                    }
                }
            }

            //If matched gem has cordinate Y < height board - 1 -> CHECK TOP MATCHED GEM
            if (y < board.GetSizeBoard().y - 1)
            {
                if (board.GetGemOnBoard(x, y + 1) != null)
                {
                    if (board.GetGemOnBoard(x, y + 1).GetGemType() == Gem.GemType.Bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y + 1), board.GetGemOnBoard(x, y + 1));
                    }
                }
            }
        }
    }

    public void MarkBombArea(Vector2Int bombPos, Gem bomb)
    {
        for (int x = bombPos.x - bomb.GetBlastRadius(); 
        x <= bombPos.x + bomb.GetBlastRadius(); x++)
        {
            for (int y = bombPos.y - bomb.GetBlastRadius(); 
            y <= bombPos.y + bomb.GetBlastRadius(); y++)
            {
                if (x >= 0 && x < board.GetSizeBoard().x && y >= 0 && y < board.GetSizeBoard().y)
                {
                    if (board.GetGemOnBoard(x, y) != null)
                    {
                        board.GetGemOnBoard(x, y).SetIsMatch(true);
                        currentGemsMatch.Add(board.GetGemOnBoard(x, y));
                    }
                }
            }
        }

        currentGemsMatch = currentGemsMatch.Distinct().ToList();
    }
    
}
