using System.Collections.Generic;
using System.Xml.Serialization;
using RestSharp.Deserializers;

namespace FishEyeClient.Entities
{
	public class ChangesetList
	{
		public bool ResultsTruncated { get; set; }

		[DeserializeAs(Name = "csid")]
		public List<string> CsidList { get; set; }
	}
}
