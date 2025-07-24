using Dapper;
using Npgsql;
using System.Data;

namespace WorkflowMgmt.Infrastructure.TypeHandlers
{
    public class IntArrayTypeHandler : SqlMapper.TypeHandler<int[]>
    {
        public override void SetValue(IDbDataParameter parameter, int[]? value)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.Value = value ?? new int[0];
                npgsqlParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer;
            }
            else
            {
                parameter.Value = value ?? new int[0];
            }
        }

        public override int[] Parse(object value)
        {
            if (value == null || value == DBNull.Value)
                return new int[0];

            if (value is int[] intArray)
                return intArray;

            // Handle PostgreSQL array types - they come as System.Array
            if (value is Array array)
            {
                // Handle multi-dimensional arrays by flattening them
                var flatList = new List<int>();
                FlattenArray(array, flatList);
                return flatList.ToArray();
            }

            // Handle string representation like "{1,2,3}"
            if (value is string stringValue)
            {
                stringValue = stringValue.Trim('{', '}');
                if (string.IsNullOrEmpty(stringValue))
                    return new int[0];

                return stringValue.Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => int.Parse(s.Trim()))
                    .ToArray();
            }

            throw new InvalidCastException($"Unable to cast object of type {value.GetType()} to int[]. Value: {value}");
        }

        private void FlattenArray(Array array, List<int> result)
        {
            for (int i = 0; i < array.Length; i++)
            {
                var element = array.GetValue(i);
                if (element is Array nestedArray)
                {
                    FlattenArray(nestedArray, result);
                }
                else if (element != null)
                {
                    result.Add(Convert.ToInt32(element));
                }
            }
        }
    }
}
