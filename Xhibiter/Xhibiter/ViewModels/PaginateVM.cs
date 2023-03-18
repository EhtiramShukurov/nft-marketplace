namespace Xhibiter.ViewModels
{
    public class PaginateVM<T>
    {
        public int MaxPageCount { get; set; }
        public int CurrentPage { get; set; }
        public IEnumerable<T> Items { get; set; }

    }
}
