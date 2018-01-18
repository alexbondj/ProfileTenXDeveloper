using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishEyeClient.Entities;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace FishEyeClient
{
	public class FishEyeApi
	{
		const string DefBaseUrl = "http://tscore-review:8060";

		private string _baseUrl;
		private string _username;
		private string _userpassword;

		public FishEyeApi() {
			ReadDefaultSettings();
		}

		public FishEyeApi(string username, string userpassword) {
			_username = username;
			_userpassword = userpassword;
			_baseUrl = DefBaseUrl;
		}

		public FishEyeApi(string serverUrl, string username, string userpassword) {
			_username = username;
			_userpassword = userpassword;
			_baseUrl = serverUrl;
		}

		private void ReadDefaultSettings() {
			_baseUrl = ConfigurationManager.AppSettings["crucibleServerUrl"];
			_username = ConfigurationManager.AppSettings["crucibleUserName"];
			_userpassword = ConfigurationManager.AppSettings["crucibleUserPassword"];
		}

		public T Execute<T>(RestRequest request) where T : new() {
			var client = new RestClient {
				BaseUrl = new Uri(_baseUrl),
				Authenticator = new HttpBasicAuthenticator(_username, _userpassword)
			};
			var response = client.Execute<T>(request);
			if (response.ErrorException != null) {
				const string message = "Error retrieving response.  Check inner details for more info.";
				var fishEyeException = new ApplicationException(message, response.ErrorException);
				throw fishEyeException;
			}
			return response.Data;
		}

		public ChangesetList GetChangesetList(string repository, DateTime start, DateTime end) {
			var request = new RestRequest { Resource = $"rest-service-fe/revisionData-v1/changesetList/{repository}" };
			request.AddParameter("start", start.ToString("yyyy-MM-dd"), ParameterType.QueryString);
			request.AddParameter("end", end.ToString("yyyy-MM-dd"), ParameterType.QueryString);
			return Execute<ChangesetList>(request);
		}

		public Changeset GetChangesetInfo(string repository, string csid) {
			var request = new RestRequest { Resource = $"/rest-service-fe/revisionData-v1/changeset/{repository}/{csid}" };
			return Execute<Changeset>(request);
		}

		public FileRevision GetRevInfo(string repository, string rev, string path) {
			var request = new RestRequest { Resource = $"/rest-service-fe/revisionData-v1/revisionInfo/{repository}" };
			request.AddParameter("path", path, ParameterType.QueryString);
			request.AddParameter("revision", rev, ParameterType.QueryString);
			return Execute<FileRevision>(request);
		}

		public ReviewsForChangeset GetReviewInfo(string repository, string csid) {
			var request = new RestRequest {
				Resource = $"/rest-service-fe/search-v1/reviewsForChangeset/{repository}",
				Method = Method.POST
			};
			request.AddParameter("cs", csid, ParameterType.GetOrPost);
			return Execute<ReviewsForChangeset>(request);
		}

		public List<ReviewsForChangeset> GetReviewsInfo(string repository, List<string> csids) {
			var request = new RestRequest {
				Resource = $"/rest-service-fe/search-v1/reviewsForChangesets/{repository}",
				Method = Method.POST
			};
			request.AddParameter("cs", string.Join(",",csids), ParameterType.GetOrPost);
			return Execute<List<ReviewsForChangeset>>(request);
		}

		public ReviewDataResp GetReviewFullInfo(string id) {
			var request = new RestRequest {
				Resource = $"/rest-service/search-v1/reviews",
				Method = Method.GET
			};
			request.AddParameter("term", id, ParameterType.QueryString);
			request.AddParameter("maxReturn", 50, ParameterType.QueryString);
			return Execute<ReviewDataResp>(request);
		}

		public ReviewerResp GetReviewersInfo(string id) {
			var request = new RestRequest {
				Resource = $"/rest-service/reviews-v1/{id}/reviewers",
				Method = Method.GET
			};
			return Execute<ReviewerResp>(request);
		}

	}
}
