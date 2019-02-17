using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.Tests.Helper
{
    /// <summary>
    /// Quick helper classes.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Quick helper to return a JSON value from a ActionResult.
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static List<TEntity> ToJson<TEntity>(this IActionResult result)
        {
            var json = (JsonResult)result;  
            return (List<TEntity>)json.Value;

            
        }
    }
}