using System;
using System.Collections.Generic;

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

	public class Review
	{
		public string ProjectKey { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ReviewParticipant Author { get; set; }
		public ReviewParticipant Moderator { get; set; }
		public ReviewParticipant Creator { get; set; }
		public string Summary { get; set; }
		public string State { get; set; }
		public string Type { get; set; }
		public PermaId PermaId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime CloseDate { get; set; }
		public DateTime DueDate { get; set; }
		public string JiraIssueKey { get; set; }
		public string ChangesetId { get; set; }
		public List<ReviewParticipant> Reviewers { get; set; }

	}
}
