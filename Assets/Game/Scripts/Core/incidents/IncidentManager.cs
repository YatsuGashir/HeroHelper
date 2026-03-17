using System.Collections.Generic;
using System.Linq;
using Core.Effects;
using Core.incidents;
using Data;
using GlobalSpace;
using UnityEngine;
using View;

namespace Game.Scripts.Core.incidents
{
    public class IncidentManager
    {
 private List<IncidentData> _allIncidents;
        private List<ActiveIncident> _activeLongEvents;
        private int _currentTurn;

        public List<ActiveIncident> ActiveLongEvents => _activeLongEvents;

        public IncidentManager(GameConfig config)
        {
            _allIncidents = config.allIncidents;
            _activeLongEvents = new List<ActiveIncident>();
            _currentTurn = 0;
        }

        public void StartRun()
        {
            _activeLongEvents.Clear();
            _currentTurn = 0;
        }

        /// <summary>
        /// Вызывается в начале каждого хода (после инкремента счета ходов)
        /// </summary>
        public void OnTurnStart(int turnNumber)
        {
            _currentTurn = turnNumber;

            // 1. Обработка ДОЛГОСРОЧНЫХ событий (уменьшение таймера)
            ProcessLongTermEvents();

            // 2. Проверка на КРАТКОСРОЧНЫЕ события (рандом)
            TryTriggerShortTermEvent();
        }

        private void ProcessLongTermEvents()
        {
            var eventsToResolve = new List<ActiveIncident>();

            foreach (var active in _activeLongEvents)
            {
                active.TurnsRemaining--;
                Debug.Log($"[ПРЕДУПРЕЖДЕНИЕ] Через {active.TurnsRemaining} ходов случится: {active.Data.incidentName}");
                G.Events.LongIncidentUpdated.OnNext(active);

                if (active.TurnsRemaining <= 0)
                {
                    eventsToResolve.Add(active);
                }
            }

            // Применяем последствия для тех, чей таймер истек
            foreach (var active in eventsToResolve)
            {
                ApplyConsequences(active.Data);
                _activeLongEvents.Remove(active);
                G.Events.LongIncidentResolved.OnNext(active);
            }
        }

        private void TryTriggerShortTermEvent()
        {
            var shortEvents = _allIncidents.Where(i => i.durationType == IncidentType.Instant).ToList();
            
            foreach (var evt in shortEvents)
            {
                if (UnityEngine.Random.value < evt.chancePerTurn)
                {
                    Debug.Log($"[СОБЫТИЕ] Мгновенное бедствие: {evt.incidentName}");
                    
                    ApplyConsequences(evt);
                    
                    G.Events.ShortIncidentOccurred.OnNext(evt);
                    break; 
                }
            }
        }


        public void StartLongTermEvent(IncidentData data)
        {
            if (data.durationType != IncidentType.Incoming) return;


            if (_activeLongEvents.Any(a => a.Data == data)) return;

            var active = new ActiveIncident(data, data.turnsUntilTrigger);
            _activeLongEvents.Add(active);

            Debug.Log($"[ПРЕДУПРЕЖДЕНИЕ] Через {data.turnsUntilTrigger} ходов случится: {data.incidentName}");
            G.Events.LongIncidentStarted.OnNext(active);
        }

        /// <summary>
        /// Единая логика нанесения урона (для обоих типов)
        /// </summary>
        private void ApplyConsequences(IncidentData data)
        {
            Debug.Log("Применяем дебафф");
            // 1. Потеря ресурсов
            if (data.resourceLoss != null && data.resourceLoss.Count > 0)
            {
                Debug.Log("Вычитаем ресурсы");
                // Создаем список изменений с отрицательными значениями
                var losses = new List<ResourceAmount>();
                foreach (var res in data.resourceLoss)
                {
                    losses.Add(new ResourceAmount(res.Type, -res.Amount));
                }
                G.ResourceManager.ApplyChanges(losses);
            }

            // 2. Поломка зданий (урон по времени жизни)
            if (data.buildingLifetimeDamage != 0)
            {
                DamageBuildings(data.buildingLifetimeDamage, data.maxBuildingsToDestroy);
            }

            // 3. Полное уничтожение зданий
            if (data.destroyRandomBuilding)
            {
                DestroyRandomBuildings(data.maxBuildingsToDestroy);
            }
        }

        private void DamageBuildings(int damage, int maxCount)
        {
            var grid = G.GridSystem.GetGridData();
            int count = 0;
            
            // Простой проход по всем зданиям
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (count >= maxCount) break;
                    
                    var cell = grid[x, y];
                    if (cell.building != null)
                    {
                        cell.building.remaingTime += damage; // damage отрицательный
                        if (cell.building.remaingTime <= 0)
                        {
                            cell.building = null; // Или пометить на удаление через Factory
                            G.Events.CellChanged.OnNext(new CellUpdateEventData { X = x, Y = y, State = cell });
                        }
                        count++;
                    }
                }
            }
        }

        private void DestroyRandomBuildings(int count)
        {
            // Логика выбора случайных зданий и их удаления через BuildingFactory
            // ... (реализация аналогична DamageBuildings, но с рандомным выбором координат)
        }
    }
}
