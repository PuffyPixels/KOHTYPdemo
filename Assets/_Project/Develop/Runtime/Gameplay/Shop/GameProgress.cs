using Assets._Project.Develop.Runtime.Gameplay.Enemy.Consultant;
using Assets._Project.Develop.Runtime.Gameplay.Player.Inventory;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Shop
{
    public class GameProgress : IDisposable
    {
        private Inventory _inventory;
        private List<ConsultantFacade> _consultants;

        public GameProgress(Inventory inventory)
        {
            _inventory = inventory;
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
            _inventory.RemoveAll();
        }
    }
}
