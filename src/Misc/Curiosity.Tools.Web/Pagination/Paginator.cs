namespace Curiosity.Tools.Web.Pagination
{
    public class Paginator
    {
        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        private PaginationGetUrlForPageDelegate? _getUrlForPage;

        public PaginationGetUrlForPageDelegate GetUrlForPage
        {
            get { return _getUrlForPage ??= page => "#"; }
            set => _getUrlForPage = value;
        }
    }
}