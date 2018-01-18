using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
