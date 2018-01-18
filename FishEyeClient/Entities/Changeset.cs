using System;
using System.Collections.Generic;
using System.Text;

namespace FishEyeClient.Entities
{
	public class Changeset
	{
		public string RepositoryName { get; set; }
		public DateTime Date { get; set; }
		public string Csid { get; set; }
		public string Branch { get; set; }
		public string Author { get; set; }
		public string Comment { get; set; }
		public List<FileRevisionKey> FileRevisionKey { get; set; }
		public ReviewsForChangeset ReviewsForChangeset { get; set; }

		public override string ToString() {
			var sb = new StringBuilder();
			sb.AppendLine($"Comment: {Comment}");
			sb.AppendLine($"Author: {Author}");
			sb.AppendLine($"Repository: {RepositoryName} branch: {Branch}");
			foreach (var fileRevisionKey in FileRevisionKey) {
				sb.AppendLine($"\t {fileRevisionKey}");
			}
			return sb.ToString();
		}

	}
}
