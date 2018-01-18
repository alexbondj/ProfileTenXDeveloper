using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishEyeClient.Entities
{
	/*
	 * {
	   "changesetId" : "aa26d45eea2d259678b801c135cde82fe6518473",
	   "reviews" : [ {
	   "projectKey" : "CR-SAMPLE",
	   "name" : "Sample Review",
	   "description" : "Description",
	   "author" : {
	   "userName" : "admin",
	   "displayName" : "A. D. Ministrator",
	   "avatarUrl" : ""
	   },
	   "moderator" : {
	   "userName" : "admin",
	   "displayName" : "A. D. Ministrator",
	   "avatarUrl" : ""
	   },
	   "creator" : {
	   "userName" : "admin",
	   "displayName" : "A. D. Ministrator",
	   "avatarUrl" : ""
	   },
	   "permaId" : {
	   "id" : "CR-SAMPLE-1"
	   },
	   "permaIdHistory" : [ "CR-SAMPLE-1" ],
	   "summary" : "Review summary",
	   "state" : "Review",
	   "type" : "REVIEW",
	   "allowReviewersToJoin" : true,
	   "metricsVersion" : 1,
	   "createDate" : "2017-11-27T14:12:31.219+0000",
	   "jiraIssueKey" : "SAMPLE-1"
	   } ]
	   }
	 */

	public class ReviewsForChangeset
	{
		public string ChangesetId { get; set; }
		public List<Review> Reviews { get; set; }
	}
}
