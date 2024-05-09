using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model
{
    public class Bookshelf<T> : IEnumerable<T>
    {
        private readonly List<Predicate<T>> _predicates;
        private readonly List<Queue<T>> _shelves;
        private readonly Dictionary<string, int> _labels;

        public int Shelves => _shelves.Count;
        public bool IsEmpty => _shelves.Count == 0;

        public Bookshelf()
        {
            _shelves = new();
            _predicates = new();
            _labels = new();
        }

        public List<T> this[string label]
        {
            get
            {
                int shelf = _labels[label];
                return _shelves[shelf].ToList();
            }
        }

        public bool IsShelfEmpty(string label)
        {
            int shelf = _labels[label];
            return _shelves[shelf].Count == 0;
        }

        public void AddShelf(string shelfLabel, Predicate<T> predicate)
        {
            _labels[shelfLabel] = Shelves;
            _predicates.Add(predicate);
            _shelves.Add(new Queue<T>());
        }

        public bool Push(string label, T item)
        {
            int shelf = _labels[label];

            if (_predicates[shelf](item))
            {
                _shelves[shelf].Enqueue(item);
                return true;
            }

            return false;
        }

        public T Pop(string label)
        {
            int shelf = _labels[label];

            if (_shelves.Count == 0) throw new Exception("Shelf is empty.");

            T item = _shelves[shelf].Dequeue();
            return item;
        }

        public T Retrieve(string label)
        {
            int shelf = _labels[label];

            if (_shelves.Count == 0) throw new Exception("Shelf is empty.");

            return _shelves[shelf].Peek();
        }

        public void ClearShelf(string label)
        {
            int shelf = _labels[label];
            _shelves[shelf].Clear();
        }

        public ObservableCollection<T> ToObservableCollection()
        {
            var collection = new ObservableCollection<T>();

            foreach (Queue<T> shelf in _shelves)
            {
                foreach (T item in shelf)
                {
                    collection.Add(item);
                }
            }

            return collection;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var list = new List<T>();

            foreach (Queue<T> shelf in _shelves)
            {
                foreach (T item in shelf)
                {
                    list.Add(item);
                }
            }

            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
