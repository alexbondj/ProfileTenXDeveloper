using System;

namespace DBClient.Entities
{
	public class CodeReview
	{
		public int Id { get; set; }
		public string Author { get; set; }
		public DateTime? CreateDate { get; set; }
		public DateTime? CloseDate { get; set; }
		public DateTime? DueDate { get; set; }
		public string PermaId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Summary { get; set; }
		public string State { get; set; }
		public string JiraUrl { get; set; }
		public string CrUrl { get; set; }
		public int ReviewersCount { get; set; }
		public int CsId { get; set; }
		public Changeset Changeset { get; set; }
		public int? ChangesetId { get; set; }

	}
}
