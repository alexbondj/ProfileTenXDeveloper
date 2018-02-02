using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using DBClient.Entities;
using FishEyeClient;
using FishEyeClient.Entities;
using Changeset = FishEyeClient.Entities.Changeset;
using DbChangeset = DBClient.Entities.Changeset;

namespace CrucibleClient
{
	public static class FishEyeExtension
	{
		public static DateTime? GetDateTimeOrNull(this DateTime dateTime) {
			return dateTime == DateTime.MinValue ? (DateTime?)null : dateTime;
		}

		public static string GetCsUrl(this string csid, string repository) {
			var baseUrl = ConfigurationManager.AppSettings["crucibleServerUrl"];
			return $"{baseUrl}/changelog/{repository}?cs={csid}";
		}

		public static string GetReviewUrl(this string cr) {
			var baseUrl = ConfigurationManager.AppSettings["crucibleServerUrl"];
			return $"{baseUrl}/cru/{cr}";
		}

		public static string GetJiraUrl(this string jiraKey) {
			var baseUrl = ConfigurationManager.AppSettings["jiraServerUrl"];
			return string.IsNullOrEmpty(jiraKey) ? null : $"{baseUrl}/browse/{jiraKey}";
		}

		public static List<Changeset> LoadRevisionInfo(this List<Changeset> changesets, string repository, FishEyeApi feApi) {
			Console.WriteLine($"Getting changesets fileRevision info for repository {repository}");
			changesets.Where(cs => cs.FileRevisionKey.Count < 500).ToList()
				.ForEach(changeset => changeset.FileRevisionKey.ForEach(revision => revision
					.RevisionInfo = feApi.GetRevInfo(repository, revision.Rev, revision.Path))
				);
			return changesets;
		}

		public static List<FileRevisionKey> LoadRevisionInfo(this DbChangeset changeset, FishEyeApi feApi) {
			Console.WriteLine($"Getting changesets fileRevision info for repository {changeset.RepositoryName}");
			var cs = feApi.GetChangesetInfo(changeset.RepositoryName, changeset.CsId.ToString());
			var result = new List<FileRevisionKey>();
			if (cs.FileRevisionKey.Count > 500) {
				return result;
			}
			cs.FileRevisionKey.ForEach(frk => frk.RevisionInfo = feApi.GetRevInfo(changeset.RepositoryName, frk.Rev, frk.Path));
			return cs.FileRevisionKey;
		}

		public static DbChangeset ToDbChangeset(this Changeset changeset) {
			var dbChangeset = new DbChangeset {
				Author = changeset.Author,
				CsId = int.Parse(changeset.Csid),
				Date = changeset.Date.GetDateTimeOrNull(),
				CsComment = changeset.Comment,
				RepositoryName = changeset.RepositoryName,
				CsUrl = changeset.Csid.GetCsUrl(changeset.RepositoryName),
			};
			return dbChangeset;
		}

		public static CodeReview ToCodeReview(this Review review, DbChangeset dbChangeset = null) {
			var codeReview = new CodeReview {
				Name = review.Name,
				Description = review.Description,
				Author = review.Author.UserName,
				CreateDate = review.CreateDate.GetDateTimeOrNull(),
				DueDate = review.DueDate.GetDateTimeOrNull(),
				CloseDate = review.CloseDate.GetDateTimeOrNull(),
				PermaId = review.PermaId.Id,
				CsId = int.Parse(review.ChangesetId),
				JiraUrl = review.JiraIssueKey.GetJiraUrl(),
				CrUrl = review.PermaId.Id.GetReviewUrl(),
				ReviewersCount = review.Reviewers.Count,
				State = review.State,
				Summary = review.Summary,
				Changeset = dbChangeset
			};
			return codeReview;
		}

		public static CodeReview UpdateFilds(this CodeReview oldReview, CodeReview newReview) {
			oldReview.Name = newReview.Name;
			oldReview.Description = newReview.Description;
			oldReview.Author = newReview.Author;
			oldReview.CreateDate = newReview.CreateDate;
			oldReview.DueDate = newReview.DueDate;
			oldReview.CloseDate = newReview.CloseDate;
			oldReview.ReviewersCount = newReview.ReviewersCount;
			oldReview.State = newReview.State;
			oldReview.Summary = newReview.Summary;
			return oldReview;
		}
		public static ChangesetFile ToChangesetFile(this FileRevisionKey fReKey, DbChangeset dbChangeset = null) {
			if (fReKey.RevisionInfo.Csid == null) {
				return null;
			}
			var changesetFile = new ChangesetFile {
				Author = fReKey.RevisionInfo.Author,
				CsId = int.Parse(fReKey.RevisionInfo.Csid),
				Date = fReKey.RevisionInfo.Date.GetDateTimeOrNull(),
				TotalLines = fReKey.RevisionInfo.TotalLines,
				LinesAdded = fReKey.RevisionInfo.LinesAdded,
				LinesRemoved = fReKey.RevisionInfo.LinesRemoved,
				Path = fReKey.RevisionInfo.Path,
				Comment = fReKey.RevisionInfo.Comment,
				FileRevisionState = fReKey.RevisionInfo.FileRevisionState,
				Revision = fReKey.RevisionInfo.Rev,
				RepositoryName = dbChangeset?.RepositoryName,
				Changeset = dbChangeset
			};
			return changesetFile;
		}

		public static List<CodeReview> GetCodeReviewList(this ReviewsForChangeset reviewsForChangeset, DbChangeset dbChangeset) {
			var listReview = new List<CodeReview>();
			reviewsForChangeset.Reviews.ForEach(
				cr => listReview.Add(cr.ToCodeReview(dbChangeset))
			);
			return listReview;
		}

		public static List<ChangesetFile> ToChangesetFileList(this List<FileRevisionKey> fileRevisionKeys, DbChangeset dbChangeset) {
			var listFileRev = new List<ChangesetFile>();
			foreach (var fReKey in fileRevisionKeys) {
				var cf = fReKey.ToChangesetFile(dbChangeset);
				if (cf != null) {
					listFileRev.Add(cf);
				}
			}
			return listFileRev;
		}
		public static List<Changeset> LoadReviewInfo(this List<Changeset> changesets, string repository, FishEyeApi feApi) {
			Console.WriteLine($"Getting changesets Reviews info for repository {repository}");
			changesets.ForEach(
				changeset => changeset
					.ReviewsForChangeset = feApi.GetReviewInfo(repository, changeset.Csid)
			);
			return changesets;
		}

		public static List<Changeset> LoadReviews(this List<Changeset> changesets, FishEyeApi feApi) {
			Console.WriteLine($"Getting Reviews full data info");
			changesets.ForEach(changeset => changeset
				.ReviewsForChangeset.Reviews.ForEach(r => changeset
					.ReviewsForChangeset.Reviews = feApi.GetReviewFullInfo(r.PermaId.Id).ReviewData)
			);
			return changesets;
		}

		public static List<Changeset> LoadReviewers(this List<Changeset> changesets, FishEyeApi feApi) {
			Console.WriteLine($"Getting Reviewers data");
			changesets.ForEach(changeset => changeset
				.ReviewsForChangeset.Reviews.ForEach(r => r
					.Reviewers = feApi.GetReviewersInfo(r.PermaId.Id).Reviewer)
			);
			return changesets;
		}

		public static List<Changeset> LinkCsToReviews(this List<Changeset> changesets) {
			Console.WriteLine($"Link changesets to reviews");
			changesets.ForEach(changeset => changeset
				.ReviewsForChangeset.Reviews.ForEach(r => r.ChangesetId = changeset.Csid)
			);
			return changesets;
		}


	}
}
