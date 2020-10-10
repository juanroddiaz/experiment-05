using System.Collections.Generic;

public class GameStateService
{
    private static readonly GameStateService _instance = new GameStateService();

    public static GameStateService Get()
    {
        return _instance;
    }

    public GameState State { get; private set; }

    public void Init(int coins, int stars)
    {
        State = new GameState()
        { 
            Observer = new GameStateObserver(),
            Coins = coins,
            Stars = stars,
            CharactersData = new List<GameState.CharacterData>(),
        };

        // Ensure to add new variables' registration inside this method.
        State.SetObserver();
    }
}