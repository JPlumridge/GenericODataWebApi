using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Web.OData.Query;

namespace GenericODataWebApi
{
    public static class GenericODataConfig
    {
        private static Dictionary<Type, ODataValidationSettings> Settings = new Dictionary<Type, ODataValidationSettings>();
        internal static ODataValidationSettings GlobalSettings { get; set; }

        public static EnabledOperations Operations = new EnabledOperations();
        public static CustomTypeMapping TypeMapping = new CustomTypeMapping();

        public static void AddValidationSettingsFor<TEdmEntity>(ODataValidationSettings settings)
        {
            Settings[typeof (TEdmEntity)] = settings;
        }

        public static void AddValidationSettingsForAll(ODataValidationSettings settings)
        {
            GlobalSettings = settings;
        }

        internal static ODataValidationSettings SettingsFor(Type edmType)
        {
            return Settings.ContainsKey(edmType) ? Settings[edmType] : null;
        }
    }
}