using System;
using System.Collections.Generic;

namespace DBClient.Entities
{
	public class Changeset
	{
		public int Id { get; set; }
		public int CsId { get; set; }
		public string Author { get; set; }
		public DateTime? Date { get; set; }
		public string CsComment { get; set; }
		public string CsUrl { get; set; }
		public string RepositoryName { get; set; }
		public List<ChangesetFile> ChangesetFiles { get; set; }
		public List<CodeReview> CodeReviews { get; set; }

		public Changeset()
		{
			ChangesetFiles = new List<ChangesetFile>();
			CodeReviews = new List<CodeReview>();
		}
	}
}
