namespace Lia.LinqToCrm.Provider
{
	internal sealed class EntityColumn
	{
		public string ParameterName { get; }

		public string Column { get; }

		public bool AllColumns { get; }

		public EntityColumn() { }

		public EntityColumn(string parameterName, string column)
		{
			ParameterName = parameterName;
			Column = column;
		}

		public EntityColumn(string parameterName, bool allColumns)
		{
			ParameterName = parameterName;
			AllColumns = allColumns;
		}
	}
}