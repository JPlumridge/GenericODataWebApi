using System;
using System.Web.OData.Builder;

namespace GenericODataWebApi
{
    internal class DefaultUserSpecifiedOnModelCreatingProvider : IOnModelCreatingProvider
    {
        private Action<ODataConventionModelBuilder> CapturedAction { get; }

        public DefaultUserSpecifiedOnModelCreatingProvider(ODataConventionModelBuilder builder)
        {
            this.CapturedAction = builder.OnModelCreating;
        }

        public Action<ODataConventionModelBuilder> GetOnModelCreating()
        {
            return CapturedAction;
        }
    }
}