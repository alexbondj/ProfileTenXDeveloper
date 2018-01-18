using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using DBClient;
using DBClient.Entities;
using FishEyeClient;
using FishEyeClient.Entities;
using Changeset = FishEyeClient.Entities.Changeset;
using DbChangeset = DBClient.Entities.Changeset;

namespace CrucibleClient
{


	public class Program
	{
		public static List<DbChangeset> LoadCsFromDb() {
			using (UserContext dbContext = new UserContext()) {
				return dbContext.Changesets.ToList();
			}
		}

		private static List<Changeset> GetChangesetList(string repository, DateTime start, DateTime end) {
			var feApi = new FishEyeApi();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"Getting data from repository {repository}");
			var list = feApi.GetChangesetList(repository, start, end);
			Console.WriteLine($"Getting changesets data from repository {repository} for changesetList");
			List<Changeset> changesets = new List<Changeset>();
			list.CsidList.ForEach(e => changesets.Add(feApi.GetChangesetInfo(repository, e)));
			changesets
				.LoadRevisionInfo(repository, feApi)
				.LoadReviewInfo(repository, feApi)
				.LoadReviews(feApi)
				.LoadReviewers(feApi)
				.LinkCsToReviews();
			return changesets;
		}

		private static List<FileRevisionKey> GetChangesetFiles(DbChangeset changeset) {
			var feApi = new FishEyeApi();
			Console.ForegroundColor = ConsoleColor.Cyan;
			return changeset.LoadRevisionInfo(feApi);
		}

		private static void SaveChangesetFiles(List<FileRevisionKey> fileRevisionKeys, string repositoryName, DbChangeset dbChangeset) {
			using (UserContext dbContext = new UserContext()) {
				fileRevisionKeys.ForEach(fReKey => dbContext.ChangesetFiles.Add(
					new ChangesetFile {
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
						RepositoryName = repositoryName,
						Changeset = dbChangeset
					}
				));
				dbContext.SaveChanges();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Changeset files saved");
			}
		}

		private static void SaveAllChanges(List<Changeset> changesets) {
			using (UserContext dbContext = new UserContext()) {
				foreach (var changeset in changesets) {
					var dbChangeset = new DbChangeset {
						Author = changeset.Author,
						CsId = int.Parse(changeset.Csid),
						Date = changeset.Date.GetDateTimeOrNull(),
						CsComment = changeset.Comment,
						RepositoryName = changeset.RepositoryName,
						CsUrl = changeset.Csid.GetCsUrl(changeset.RepositoryName),
					};
					dbContext.Changesets.Add(dbChangeset);
					var listReview = new List<CodeReview>();
					changeset.ReviewsForChangeset.Reviews.ForEach(cr => listReview.Add(new CodeReview {
						Name = cr.Name,
						Description = cr.Description,
						Author = cr.Author.UserName,
						CreateDate = cr.CreateDate.GetDateTimeOrNull(),
						DueDate = cr.DueDate.GetDateTimeOrNull(),
						CloseDate = cr.CloseDate.GetDateTimeOrNull(),
						PermaId = cr.PermaId.Id,
						CsId = int.Parse(cr.ChangesetId),
						JiraUrl = cr.JiraIssueKey.GetJiraUrl(),
						CrUrl = cr.PermaId.Id.GetReviewUrl(),
						ReviewersCount = cr.Reviewers.Count,
						State = cr.State,
						Summary = cr.Summary,
						Changeset = dbChangeset
					}));
					dbContext.CodeReviews.AddRange(listReview);
					if (changeset.FileRevisionKey.Count < 500) {
						var listFileRev = new List<ChangesetFile>();
						changeset.FileRevisionKey.ForEach(fReKey => listFileRev.Add(
							new ChangesetFile {
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
								RepositoryName = changeset.RepositoryName,
								Changeset = dbChangeset
							}
						));
						dbContext.ChangesetFiles.AddRange(listFileRev);
					}
				}
				dbContext.SaveChanges();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("All Changes was saved");
			}
		}

		private static void SaveChangesets(List<Changeset> changesets) {
			using (UserContext dbContext = new UserContext()) {
				changesets.ForEach(cs => dbContext.Changesets.Add(
					new DbChangeset {
						Author = cs.Author,
						CsId = int.Parse(cs.Csid),
						Date = cs.Date.GetDateTimeOrNull(),
						CsComment = cs.Comment,
						RepositoryName = cs.RepositoryName,
						CsUrl = cs.Csid.GetCsUrl(cs.RepositoryName),
					}
				));
				dbContext.SaveChanges();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Changesets changes saved");
			}
		}
		private static void SaveCodeReviews(List<Review> reviews) {
			using (UserContext dbContext = new UserContext()) {
				reviews.ForEach(cr => dbContext.CodeReviews.Add(
					new CodeReview {
						Name = cr.Name,
						Description = cr.Description,
						Author = cr.Author.UserName,
						CreateDate = cr.CreateDate.GetDateTimeOrNull(),
						DueDate = cr.DueDate.GetDateTimeOrNull(),
						CloseDate = cr.CloseDate.GetDateTimeOrNull(),
						PermaId = cr.PermaId.Id,
						CsId = int.Parse(cr.ChangesetId),
						JiraUrl = cr.JiraIssueKey.GetJiraUrl(),
						CrUrl = cr.PermaId.Id.GetReviewUrl(),
						ReviewersCount = cr.Reviewers.Count,
						State = cr.State,
						Summary = cr.Summary
					}
				));
				dbContext.SaveChanges();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Reviews changes  saved");
			}
		}

		private static List<Changeset> GetChangesetDayByDay(string repository, DateTime startDate, DateTime endDate) {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Getting changesets data from repository {repository} from:{startDate} to:{endDate}");
			DateTime start = startDate;
			DateTime end = start.AddDays(1);
			List<Changeset> changesets = new List<Changeset>();
			while (end <= endDate) {
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine($"Getting changesets data from repository {repository} from:{start} to:{end}");
				changesets.AddRange(GetChangesetList(repository, start, end));
				start = end;
				end = end.AddDays(1);
			}
			return changesets;
		}

		private static void SaveChangesetDayByDay(string repository, DateTime startDate, DateTime endDate) {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Getting changesets data from repository {repository} from:{startDate} to:{endDate}");
			DateTime start = startDate;
			DateTime end = start.AddDays(1);
			while (end <= endDate) {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"Getting changesets data from repository {repository} from:{start} to:{end}");
				var changesets = GetChangesetList(repository, start, end);
				//SaveChangesets(changesets);
				//var reviews = new List<Review>();
				//changesets.ForEach(cs => reviews.AddRange(cs.ReviewsForChangeset.Reviews));
				//SaveCodeReviews(reviews);
				SaveAllChanges(changesets);
				start = end;
				end = end.AddDays(1);
			}
		}

		private static void SaveChangesetFiles() {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Getting changesets data from DB");
			var dbChangesets = LoadCsFromDb();
			dbChangesets.ForEach(cs => SaveChangesetFiles(GetChangesetFiles(cs), cs.RepositoryName, cs));
		}

		static void Main(string[] args) {
			var repositories = ConfigurationManager.AppSettings["crucibleRepositories"].Split(';');
			DateTime startDate = new DateTime(2017, 10, 01, 00, 00, 00);
			////DateTime endDate = new DateTime(2018, 01, 01, 00, 00, 00);
			//DateTime startDate = new DateTime(2018, 01, 10, 00, 00, 00);
			DateTime endDate = DateTime.Today.AddDays(1);
			////List<Changeset> changesets = new List<Changeset>();
			foreach (string repository in repositories) {
				SaveChangesetDayByDay(repository, startDate, endDate);
			}
			//SaveChangesetFiles();
			//var authorList = changesets.GroupBy(ch => ch.Author).OrderBy(ch => ch.Count());
			//authorList.ToList().ForEach(ch => Console.WriteLine($"{ch.Key}: {ch.Count()}"));
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Work's Done!!!");
			Console.ReadKey();
		}
	}
}
