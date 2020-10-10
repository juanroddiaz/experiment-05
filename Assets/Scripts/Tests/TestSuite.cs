using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class TestSuite
    {
        [Test]
        public void CanInitGameState()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);

            var gameState = gameStateService.State;

            Assert.That(gameState.Coins, Is.EqualTo(10));
            Assert.That(gameState.Stars, Is.EqualTo(0));
        }

        [Test]
        public void CanObserveGameStateChanges()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);

            var gameState = gameStateService.State;
            var stateObserverCalled = false;
            gameState.AddOnCoinsChanged(() =>
            {
                stateObserverCalled = true;
                Assert.That(gameState.Coins, Is.EqualTo(8));
            });

            ShopService.Get().UseCoins(2);
            gameState.Observer.ExecuteActions();
            Assert.That(stateObserverCalled, "Observer not called");
        }

        [Test]
        public void CanObserveConsistentGameStateChanges()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);

            var gameState = gameStateService.State;
            var stateObserverCalled = false;
            void StateValidator()
            {
                stateObserverCalled = true;
                Assert.That(gameState.Stars, Is.EqualTo(1));
                Assert.That(gameState.Coins, Is.EqualTo(9));
            }

            gameState.AddOnCoinsChanged(StateValidator);
            gameState.AddOnStarsChanged(StateValidator);

            var shopService = ShopService.Get();
            shopService.BuyStars(1, 1);
            gameState.Observer.ExecuteActions();
            Assert.That(stateObserverCalled, "Observer not called");
        }

        /// <summary>
        /// This tests:
        /// * consistent data after modifying coins and stars twice
        /// * only one Coins/Stars validator will be called as they are the same, even if data was modified twice and they are different fields
        /// </summary>
        [Test]
        public void CanUseSameObserverConsistentGameStateChanges()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);

            var gameState = gameStateService.State;
            var stateObserverCalled = 0;
            void StateValidator()
            {
                stateObserverCalled++;
                Assert.That(gameState.Stars, Is.EqualTo(2));
                Assert.That(gameState.Coins, Is.EqualTo(8));
            }

            gameState.AddOnCoinsChanged(StateValidator);
            gameState.AddOnStarsChanged(StateValidator);

            var shopService = ShopService.Get();
            shopService.BuyStars(1, 1);
            shopService.BuyStars(1, 1);
            gameState.Observer.ExecuteActions();
            Assert.That(stateObserverCalled == 1,  "Observer should called once as it's the same one.");
        }

        /// <summary>
        /// This tests:
        /// * consistent data after modifying coins and stars
        /// * only calling Coins and Starts unique validators and not calling Character validator as it wasn't modified.
        /// </summary>
        [Test]
        public void CanSelectivelyObserveConsistentGameStateChanges()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);

            var gameState = gameStateService.State;
            var coinsObserverCalled = false;
            void CoinsValidator()
            {
                coinsObserverCalled = true;
                Assert.That(gameState.Coins, Is.EqualTo(9));
                Assert.That(gameState.Stars, Is.EqualTo(1));
            }

            var starsObserverCalled = false;
            void StarsValidator()
            {
                starsObserverCalled = true;
                Assert.That(gameState.Stars, Is.EqualTo(1));
                Assert.That(gameState.Coins, Is.EqualTo(9));
            }

            void CharactersValidator()
            {
                Assert.That(false, "This must not be called! No operation related to Characters.");
            }

            gameState.AddOnCoinsChanged(CoinsValidator);
            gameState.AddOnStarsChanged(StarsValidator);
            gameState.AddOnCharactersDataChanged(CharactersValidator);

            var shopService = ShopService.Get();
            shopService.BuyStars(1, 1);
            gameState.Observer.ExecuteActions();
            Assert.That(coinsObserverCalled && starsObserverCalled, "A Observer wasn't called");
        }

        /// <summary>
        /// This tests:
        /// * consistent data after modifying coins, stars and character data
        /// * only calling unique validators once per field modification
        /// </summary>
        [Test]
        public void CanObserveConsistentCharacterGameStateChanges()
        {
            var gameStateService = GameStateService.Get();
            gameStateService.Init(10, 0);

            var gameState = gameStateService.State;
            var coinsObserverCalled = 0;
            void CoinsValidator()
            {
                coinsObserverCalled++;
                Assert.That(gameState.Coins, Is.EqualTo(0));
            }

            var starsObserverCalled = 0;
            void StarsValidator()
            {
                starsObserverCalled++;
                Assert.That(gameState.Stars, Is.EqualTo(1));
            }

            var charObserverCalled = 0;
            void CharactersValidator()
            {
                charObserverCalled++;
                Assert.That(gameState.CharactersData[0].Id, Is.EqualTo(1));
                Assert.That(gameState.CharactersData[0].Level, Is.EqualTo(3));
            }

            gameState.AddOnCoinsChanged(CoinsValidator);
            gameState.AddOnStarsChanged(StarsValidator);
            gameState.AddOnCharactersDataChanged(CharactersValidator);

            var shopService = ShopService.Get();
            shopService.BuyStars(5, 5);
            shopService.UnlockCharacter(1, 5);
            shopService.UpgradeCharacter(1, 2);
            shopService.BuyStars(1, 1);
            shopService.UpgradeCharacter(1, 2);
            gameState.Observer.ExecuteActions();
            Assert.That(coinsObserverCalled == 1 && starsObserverCalled == 1 && charObserverCalled == 1, "Wrong amount of Observers called");
        }
    }
}