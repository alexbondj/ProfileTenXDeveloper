using System;

namespace DBClient.Entities
{
	public class ChangesetFile
	{
		public int Id { get; set; }
		public string FileRevisionState { get; set; }
		public string Revision { get; set; }
		public string Path { get; set; }
		public int TotalLines { get; set; }
		public int LinesRemoved { get; set; }
		public int LinesAdded { get; set; }
		public DateTime? Date { get; set; }
		public int CsId { get; set; }
		public string RepositoryName { get; set; }
		public string Author { get; set; }
		public string Comment { get; set; }
		public Changeset Changeset { get; set; }
		public int? ChangesetId { get; set; }
	}
}
