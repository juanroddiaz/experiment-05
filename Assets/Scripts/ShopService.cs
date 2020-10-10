public class ShopService
{
    private static readonly ShopService _instance = new ShopService();

    public static ShopService Get()
    {
        return _instance;
    }

    public void BuyStars(int stars, int forCoins)
    {
        GameStateService.Get().State.Stars += stars;
        UseCoins(forCoins);
    }

    public void UseCoins(int coins)
    {
        GameStateService.Get().State.Coins -= coins;
    }

    public void UnlockCharacter(int charIdx, int forStars)
    {
        GameStateService.Get().State.Stars -= forStars;
        GameStateService.Get().State.UnlockCharacter(charIdx);
    }

    public void UpgradeCharacter(int charIdx, int forCoins)
    {
        GameStateService.Get().State.Coins -= forCoins;
        GameStateService.Get().State.UpgradeCharacter(charIdx);
    }
}