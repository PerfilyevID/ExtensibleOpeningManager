using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExtensibleOpeningManager.Forms
{
    public class WPFSource<T>
    {
        public ObservableCollection<T> Collection { get; private set; }
        public WPFSource(List<T> elements)
        {
            Collection = new ObservableCollection<T>();
            foreach (T element in elements)
            {
                Collection.Add(element);
            }

        }
    }
}
