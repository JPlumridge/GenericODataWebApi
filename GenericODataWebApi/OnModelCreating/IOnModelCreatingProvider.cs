using System;
using System.Web.OData.Builder;

namespace GenericODataWebApi
{
    internal interface IOnModelCreatingProvider
    {
        /// <summary>
        /// Returns the OnModelCreating Action
        /// </summary>
        /// <returns>The OnModelCreating handler, or null</returns>
        Action<ODataConventionModelBuilder> GetOnModelCreating();
    }
}