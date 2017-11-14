namespace Theseus.EventArgs
{
    public class DuplicateEventArgs<T> : System.EventArgs
    {
        public T Object { get; set; }
    }
}
