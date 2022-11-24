namespace SCICT.Utility.Converters
{
    public class Entry<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public Entry()
        {
        }

        public Entry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
