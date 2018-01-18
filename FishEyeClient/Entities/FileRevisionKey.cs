using System.Text;

namespace FishEyeClient.Entities
{
	public class FileRevisionKey
	{
		public FileRevisionKey() {
			RevisionInfo = new FileRevision();
		}

		public string Path { get; set; }
		public string Rev { get; set; }
		public FileRevision RevisionInfo { get; set; }

		public override string ToString() {
			var sb = new StringBuilder();
			sb.AppendLine($"Revision: {Rev}");
			sb.AppendLine($"Path: {Path}");
			return sb.ToString();
		}
	}
}
