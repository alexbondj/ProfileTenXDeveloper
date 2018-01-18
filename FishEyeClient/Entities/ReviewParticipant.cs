using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishEyeClient.Entities
{
	/*
	 * {
	  "author" : {
	   "userName" : "admin",
	   "displayName" : "A. D. Ministrator",
	   "avatarUrl" : ""
	   }
	 */
	public class ReviewParticipant
	{
		public string UserName { get; set; }
		public string DisplayName { get; set; }
		public string AvatarUrl { get; set; }
		public bool Completed { get; set; }
	}
}
