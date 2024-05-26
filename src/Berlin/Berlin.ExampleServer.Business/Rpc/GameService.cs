using Berlin.Library;

namespace Berlin.ExampleServer.Business;

public class GameService
{
    [Rpc]
    public async Task MovePiece(string from, string to)
    {
    }
    
    [Rpc]
    public async Task DeclareCheckMate()
    {
    }
    
    [Rpc]
    public async Task DeclareDraw()
    {
    }
}

public class Game
{
    
}


public class Board
{
    
}