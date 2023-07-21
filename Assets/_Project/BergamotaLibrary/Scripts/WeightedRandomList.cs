using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BergamotaLibrary
{
    [System.Serializable]
    public class WeightedRandomList<T>
    {
        //Variables
        [SerializeField] private List<Item> items = new List<Item>();
        private List<Entry> entries = new List<Entry>();
        private double accumulatedWeight;
        private System.Random rand = new System.Random();

        public int Count => items.Count;

        private void AddEntry(T item, double weight)
        {
            accumulatedWeight += weight;
            entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight });
        }
        
        public void AddItem(T item, double weight)
        {
            items.Add(new Item { item = item, weight = weight });
        }

        private void ClearList()
        {
            accumulatedWeight = 0;
            entries.Clear();
        }

        /// <summary>
        /// Retorna um item aleatorio da lista, considerando os pesos atribuidos a eles.
        /// </summary>
        /// <param name="itens">A lista de itens com seus pesos</param>
        /// <returns></returns>
        public T GetRandom(List<Item> itens)
        {
            ClearList();

            for (int i = 0; i < itens.Count; i++)
            {
                AddEntry(itens[i].item, itens[i].weight);
            }

            double r = rand.NextDouble() * accumulatedWeight;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].accumulatedWeight >= r)
                {
                    T item = entries[i].item;

                    ClearList();
                    return item;
                }
            }

            ClearList();

            return default(T); //Should only happen when there are no entries
        }

        public T GetRandom()
        {
            ClearList();
            
            for (int i = 0; i < items.Count; i++)
            {
                AddEntry(items[i].item, items[i].weight);
            }

            double r = rand.NextDouble() * accumulatedWeight;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].accumulatedWeight >= r)
                {
                    T item = entries[i].item;

                    ClearList();

                    return item;
                }
            }

            ClearList();

            return default(T); //Should only happen when there are no entries
        }

        public (T, double) GetRandomWithWeight()
        {
            ClearList();
            
            for (int i = 0; i < items.Count; i++)
            {
                AddEntry(items[i].item, items[i].weight);
            }

            double r = rand.NextDouble() * accumulatedWeight;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].accumulatedWeight >= r)
                {
                    T item = entries[i].item;

                    ClearList();

                    return (item, items[i].weight);
                }
            }

            ClearList();

            return (default(T), default(double)); //Should only happen when there are no entries
        }

        public void RemoveItem(T item)
        {
            items.Remove(items.First(element => EqualityComparer<T>.Default.Equals(element.item, item)));
        }

        [System.Serializable]
        public struct Item
        {
            public double weight;
            public T item;
        }

        private struct Entry
        {
            public double accumulatedWeight;
            public T item;
        }
    }
}
