// -----------------------------------------------------------------------
// <copyright file="PaginatedListCollection.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoWebProjekt.Helper
{
    /// <summary>
    /// This class slice the list in different pages to provide loading all the contents.
    /// </summary>
    /// <typeparam name="T"> The type of the list content. </typeparam>
    public class PaginatedListCollection<T> : List<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedListCollection{T}"/> class.
        /// </summary>
        /// <param name="items"> The list which contains the items. </param>
        /// <param name="count"> The amount of items in List. </param>
        /// <param name="pageIndex"> The current page number. </param>
        /// <param name="pageSize"> THe amount of items at the list. </param>
        public PaginatedListCollection(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        /// <summary>
        /// Gets Page Inedex.
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Gets amount of pages.
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Gets a value indicating whether checks if there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// Gets a value indicating whether checks if ther is next page.
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Create the paginated list.
        /// </summary>
        /// <param name="source"> The Queryable which contains all the data. </param>
        /// <param name="pageIndex"> The page index. </param>
        /// <param name="pageSize"> The amount of items on a page. </param>
        /// <returns> The paginatedListCollection. </returns>
        public static PaginatedListCollection<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedListCollection<T>(items, count, pageIndex, pageSize);
        }
    }
}