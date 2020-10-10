using System;
using System.Collections.Generic;
using UnityEngine;


public class GameState
{
    public GameStateObserver Observer;

    /// <summary>
    /// I could lazy-subscribe a field the first time it's changed but I really like to try to divide logic in a way it's more predictable, to control when and where to initialize components and dependencies.
    /// </summary>
    public void SetObserver()
    {
        Observer.SubscribeObject("Coins");
        Observer.SubscribeObject("Stars");
        Observer.SubscribeObject("CharactersData");
    }

    public void AddOnCoinsChanged(Action a)
    {
        Observer.RegisterOnChangedAction("Coins", a);
    }
    
    private int _coins;
    public int Coins
    {
        get => _coins;
        set
        {
            var coins = _coins;
            _coins = value;

            if (value != coins)
            {
                Observer.OnValueChanged("Coins");
            }
        }
    }

    public void AddOnStarsChanged(Action a)
    {
        Observer.RegisterOnChangedAction("Stars", a);
    }

    private int _stars;
    public int Stars
    {
        get => _stars;
        set
        {
            var stars = _stars;
            _stars = value;

            if (value != stars)
            {
                Observer.OnValueChanged("Stars");
            }
        }
    }

    public class CharacterData
    {
        public int Id = 0;
        public int Level = 0;
    }

    public void AddOnCharactersDataChanged(Action a)
    {
        Observer.RegisterOnChangedAction("CharactersData", a);
    }

    private List<CharacterData> _charactersData = null;
    public List<CharacterData> CharactersData
    {
        get => _charactersData;
        set
        {
            if (value == null)
            {
                Debug.LogError("Character Data cannot be null!");
                return;
            }

            var data = _charactersData;
            _charactersData = value;

            if (data != null && !value.Equals(data))
            {
                Observer.OnValueChanged("CharactersData");
            }
        }
    }

    public void UnlockCharacter(int id)
    {
        var charIdx = _charactersData.FindIndex(c => c.Id == id);
        if (charIdx >= 0)
        {
            Debug.LogError("Repeated Character! id: " + id);
            return;
        }

        _charactersData.Add(new CharacterData
        {
            Id = id,
            Level = 1,
        });
        Observer.OnValueChanged("CharactersData");
    }

    public void UpgradeCharacter(int id)
    {
        var charIdx = _charactersData.FindIndex(c => c.Id == id);
        if (charIdx < 0)
        {
            Debug.LogError("No target character to upgrade! charId: " + id);
            return;
        }

        ++_charactersData[charIdx].Level;
        Observer.OnValueChanged("CharactersData");
    }
}