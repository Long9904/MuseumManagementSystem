using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Abstractions
{
    public class BasePaginatedList<T>
    {
        public IReadOnlyCollection<T> Items { get; set; } 
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public BasePaginatedList()
        {
            Items = new List<T>();
        }
        public BasePaginatedList(IReadOnlyCollection<T> items, int totalItems, int currentPage, int pageSize)
        {
            
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            Items = items;
        }

    }
}
