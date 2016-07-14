using System.Collections.Generic;

namespace GenericODataWebApi
{
    public class EnabledOperations
    {
        private static List<ODataOperations> Operations = new List<ODataOperations> {ODataOperations.Get};

        public void EnableOperations(params ODataOperations[] operations)
        {
            foreach (var operation in operations)
            {
                EnableOperation(operation);
            }
        }

        public void EnableOperation(ODataOperations operation)
        {
            if (!OperationIsEnabled(operation))
                Operations.Add(operation);
        }

        public void DisableOperation(ODataOperations operation)
        {
            if (OperationIsEnabled(operation))
                Operations.Remove(operation);
        }

        public bool OperationIsEnabled(ODataOperations operation)
        {
            return Operations.Contains(operation);
        }
    }

    public static class EnabledOperationsExtensions
    {
        public static bool IsEnabled(this ODataOperations operation)
        {
            return GenericODataConfig.Operations.OperationIsEnabled(operation);
        }
    }
}