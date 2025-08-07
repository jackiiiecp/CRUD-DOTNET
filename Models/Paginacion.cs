﻿namespace CRUDJACKIE.Models
{

        public class PagedViewModel<T>
        {
            public List<T> Items { get; set; } = new List<T>();
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalItems { get; set; }

            public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        }
    
}
