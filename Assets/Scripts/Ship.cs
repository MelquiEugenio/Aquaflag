using System.Collections;

public class Ship : Pieces
{
    private ArrayList possibleMoves = new ArrayList();
    private ArrayList impossibleMovesForShip = new ArrayList();
    
    public override ArrayList PossibleMoves(string[,] board, string fieldName)
    {
        //earth fields
        impossibleMovesForShip.Add("f2");
        impossibleMovesForShip.Add("d2");
        impossibleMovesForShip.Add("f4");
        impossibleMovesForShip.Add("h2");
        impossibleMovesForShip.Add("c3");
        impossibleMovesForShip.Add("e5");
        impossibleMovesForShip.Add("f6");
        impossibleMovesForShip.Add("g5");
        impossibleMovesForShip.Add("i3");
        impossibleMovesForShip.Add("b3");
        impossibleMovesForShip.Add("f7");
        impossibleMovesForShip.Add("j3");
        impossibleMovesForShip.Add("c6");
        impossibleMovesForShip.Add("e8");
        impossibleMovesForShip.Add("f8");
        impossibleMovesForShip.Add("g8");
        impossibleMovesForShip.Add("i6");
        impossibleMovesForShip.Add("d8");
        impossibleMovesForShip.Add("f10");
        impossibleMovesForShip.Add("h8");
        impossibleMovesForShip.Add("f12");

        FindNeighbors(board, fieldName);
        
        int firstNeighborsCount = possibleMoves.Count; //if we use 'possibleMoves.Count' to set the loop limit, it'll loop forever

        for (int i = 0; i < firstNeighborsCount; i++)
        {
            FindNeighbors(board, possibleMoves[i].ToString());
        }

        possibleMoves.Remove(fieldName);

        return possibleMoves;
    }

    private void FindNeighbors(string[,] board, string fieldName)
    {
        for (int i = 0; i<board.GetLength(0); i++)
        {
            for (int j = 0; j<board.GetLength(1); j++)
            {
                if (board[i, j] == fieldName)
                {
                    if (board[i, j + 1] != null && !impossibleMovesForShip.Contains(board[i, j + 1]) && !possibleMoves.Contains(board[i, j + 1]))
                        possibleMoves.Add(board[i, j + 1]);

                    if (board[i + 1, j + 1] != null && !impossibleMovesForShip.Contains(board[i + 1, j + 1]) && !possibleMoves.Contains(board[i + 1, j + 1]))
                        possibleMoves.Add(board[i + 1, j + 1]);

                    if (board[i + 1, j] != null && !impossibleMovesForShip.Contains(board[i + 1, j]) && !possibleMoves.Contains(board[i + 1, j]))
                        possibleMoves.Add(board[i + 1, j]);

                    if (board[i, j - 1] != null && !impossibleMovesForShip.Contains(board[i, j - 1]) && !possibleMoves.Contains(board[i, j - 1]))
                        possibleMoves.Add(board[i, j - 1]);

                    if (board[i - 1, j - 1] != null && !impossibleMovesForShip.Contains(board[i - 1, j - 1]) && !possibleMoves.Contains(board[i - 1, j - 1]))
                        possibleMoves.Add(board[i - 1, j - 1]);

                    if (board[i - 1, j] != null && !impossibleMovesForShip.Contains(board[i - 1, j]) && !possibleMoves.Contains(board[i - 1, j]))
                        possibleMoves.Add(board[i - 1, j]);

                    return;
                }
            }
        }
    }
}
