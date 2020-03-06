using System.Collections;

public class Captain : Pieces
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override ArrayList PossibleMoves(string[,] board, string fieldName)
    {
        ArrayList possibleMoves = new ArrayList();

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == fieldName)
                {
                    if (board[i, j + 1] != null)
                        possibleMoves.Add(board[i, j + 1]);

                    if (board[i + 1, j + 1] != null)
                        possibleMoves.Add(board[i + 1, j + 1]);

                    if (board[i + 1, j]  != null)
                        possibleMoves.Add(board[i + 1, j]);

                    if (board[i, j - 1] != null)
                        possibleMoves.Add(board[i, j - 1]);

                    if (board[i - 1, j - 1] != null)
                        possibleMoves.Add(board[i - 1, j - 1]);

                    if (board[i - 1, j] != null)
                        possibleMoves.Add(board[i - 1, j]);

                    break;
                }
            }
        }

        return possibleMoves;
    }
}
