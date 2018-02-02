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
		private static List<DbChangeset> LoadCsFromDb() {
			using (UserContext dbContext = new UserContext()) {
				return dbContext.Changesets.ToList();
			}
		}

		private static List<FileRevisionKey> GetChangesetFiles(DbChangeset changeset) {
			var feApi = new FishEyeApi();
			Console.ForegroundColor = ConsoleColor.Cyan;
			return changeset.LoadRevisionInfo(feApi);
		}

		private static void SaveChangesetFiles() {
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Getting changesets data from DB");
			var dbChangesets = LoadCsFromDb();
			dbChangesets.ForEach(cs => SaveChangesetFiles(GetChangesetFiles(cs), cs.RepositoryName, cs));
		}

		private static void SaveChangesetFiles(List<FileRevisionKey> fileRevisionKeys, string repositoryName, DbChangeset dbChangeset) {
			using (UserContext dbContext = new UserContext()) {
				fileRevisionKeys.ForEach(fReKey => dbContext.ChangesetFiles.Add(
					fReKey.ToChangesetFile()
				));
				dbContext.SaveChanges();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Changeset files saved");
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

		private static void SaveAllChanges(List<Changeset> changesets) {
			using (UserContext dbContext = new UserContext()) {
				Console.ForegroundColor = ConsoleColor.Yellow;
				foreach (var changeset in changesets) {
					var dbChangeset = dbContext.Changesets.FirstOrDefault(
						cs => cs.CsId.ToString() == changeset.Csid && cs.RepositoryName == changeset.RepositoryName);
					if (dbChangeset == null) {
						dbChangeset = changeset.ToDbChangeset();
						dbContext.Changesets.Add(dbChangeset);
						dbContext.CodeReviews
							.AddRange(changeset.ReviewsForChangeset.GetCodeReviewList(dbChangeset));
						dbContext.ChangesetFiles
							.AddRange(changeset.FileRevisionKey.ToChangesetFileList(dbChangeset));
						Console.WriteLine("Added new changeset with details");
					} else {
						foreach (var codeReview in changeset.ReviewsForChangeset.GetCodeReviewList(dbChangeset)) {
							var mCodeReview = dbContext.CodeReviews.FirstOrDefault(cr => cr.PermaId == codeReview.PermaId && cr.ChangesetId == dbChangeset.Id);
							if (mCodeReview != null) {
								mCodeReview.UpdateFilds(codeReview);
								dbContext.Entry(mCodeReview).State = EntityState.Modified;
								Console.WriteLine("CodeReview was updated");
							} else {
								dbContext.CodeReviews.Add(codeReview);
								Console.WriteLine("CodeReview was added");
							}
						}
					}
				}
				dbContext.SaveChanges();
				Console.WriteLine("All Changes was saved");
			}
		}

		private static void SaveChangesets(List<Changeset> changesets) {
			using (UserContext dbContext = new UserContext()) {
				changesets.ForEach(cs => dbContext.Changesets.Add(
					cs.ToDbChangeset()
				));
				dbContext.SaveChanges();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Changesets changes saved");
			}
		}
		private static void SaveCodeReviews(List<Review> reviews) {
			using (UserContext dbContext = new UserContext()) {
				reviews.ForEach(cr => dbContext.CodeReviews.Add(
					cr.ToCodeReview()));
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
				SaveAllChanges(changesets);
				start = end;
				end = end.AddDays(1);
			}
		}

		private static (DateTime startDate, DateTime endDate) GetInitDates() {
			DateTime startDate = DateTime.Today.AddDays(-3);
			DateTime endDate = DateTime.Today.AddDays(1);
			var sStartDate = ConfigurationManager.AppSettings["startDate"];
			var sEndDate = ConfigurationManager.AppSettings["endDate"];
			if (!(string.IsNullOrEmpty(sStartDate) || string.IsNullOrEmpty(sEndDate))) {
				IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
				startDate = DateTime.Parse(sStartDate, culture, System.Globalization.DateTimeStyles.AssumeLocal);
				endDate = DateTime.Parse(sEndDate, culture, System.Globalization.DateTimeStyles.AssumeLocal);
			}
			var daysCount = ConfigurationManager.AppSettings["daysCount"];
			if (!string.IsNullOrEmpty(daysCount)) {
				startDate = DateTime.Today.AddDays(-int.Parse(daysCount));
			}
			return (startDate, endDate);
		}


		static void Main(string[] args) {
			var repositories = ConfigurationManager.AppSettings["crucibleRepositories"].Split(';');
			var dates = GetInitDates();
			foreach (string repository in repositories) {
				SaveChangesetDayByDay(repository, dates.startDate, dates.endDate);
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Work's Done!!!");
			var isBackgroundMode = bool.Parse(ConfigurationManager.AppSettings["useBackgroundMode"]);
			if (!isBackgroundMode) {
				Console.ReadKey();
			}
		}
	}
}
