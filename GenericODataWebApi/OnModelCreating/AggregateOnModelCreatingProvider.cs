using System;
using System.Collections.Generic;
using System.Web.OData.Builder;

namespace GenericODataWebApi
{
    internal class AggregateOnModelCreatingProvider : IOnModelCreatingProvider
    {
        private ODataConventionModelBuilder Builder { get; }
        private IEnumerable<IOnModelCreatingProvider> Providers { get; }

        public AggregateOnModelCreatingProvider(ODataConventionModelBuilder builder, IEnumerable<IOnModelCreatingProvider> providers)
        {
            this.Builder = builder;
            this.Providers = providers;
        }

        public Action<ODataConventionModelBuilder> GetOnModelCreating()
        {
            return b =>
            {
                foreach (var provider in Providers)
                {
                    provider.GetOnModelCreating()?.Invoke(b);
                }
            };
        }
    }
}