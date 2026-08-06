using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Gameplay.Interactable.ItemsSpawner;
using Assets._Project.Develop.Runtime.Gameplay.Player;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    public class GameProgress : IDisposable
    {
        private Inventory _inventory;
        private ItemsSpawner _itemsSpawner;
        private Hero _hero;
        private List<ConsultantFacade> _consultants = new();

        public GameProgress(Inventory inventory, ItemsSpawner itemsSpawner, Hero hero)
        {
            _inventory = inventory;
            _itemsSpawner = itemsSpawner;
            _hero = hero;
        }

        public void AddConsultant(ConsultantFacade consultant)
        {
            consultant.PlayerCaptured += OnPlayerCaptured;
            _consultants.Add(consultant);
        }

        public void Dispose()
        {
            foreach (var consultant in _consultants)
                consultant.PlayerCaptured -= OnPlayerCaptured;
        }

        private void OnPlayerCaptured()
        {
            _itemsSpawner.Respawn(_inventory.Items);
            _inventory.RemoveAll();
            _hero.Stun();
        }
    }
}
