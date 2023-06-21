namespace FeatureClient2
{
	internal class LaunchDarklyDto
	{
		public class LdPatchSegmentPayload
		{
			public IEnumerable<LdPatchOperation> Patch { get; set; }
			public string Comment { get; set; }
		}

		public class LdPatchOperation
		{
			/// <summary>
			/// Operation type requested, example `remove`
			/// </summary>
			public string Op { get; set; }

			public string Path { get; set; }
		}

		public class Clause
		{
			public IList<string> Values { get; set; }
		}

		public class Rule
		{
			public IList<Clause> Clauses { get; set; }
		}

		public class Segment
		{
			public IEnumerable<Rule> Rules { get; set; }
		}
	}
}
