using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using WebMatrix.Security.Clients;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Runtime.Serialization;

namespace Demo.Models
{
  public class GoogleClient : OAuth2Client
  {
    private readonly string appId;
    private readonly string appSecret;

    public GoogleClient(string appId, string appSecret) : base("google_oauth")
    {
      this.appId = appId;
      this.appSecret = appSecret;
    }

    protected override Uri GetServiceLoginUrl(Uri returnUrl)
    {
      UriBuilder builder = new UriBuilder("https://accounts.google.com/o/oauth2/auth");
      builder.Query = string.Format("response_type=code&client_id={0}&redirect_uri={1}&scope={2}",
        this.appId,
        returnUrl,
        "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email");
      return builder.Uri;
    }

    protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
    {
      UriBuilder builder = new UriBuilder("https://accounts.google.com/o/oauth2/token");
      WebRequest request = WebRequest.Create(builder.Uri.AbsoluteUri);
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";
      using (Stream requestStream = request.GetRequestStream())
      {
        using (StreamWriter wr = new StreamWriter(requestStream))
        {
          wr.Write(string.Format(
              "code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code",
              authorizationCode,
              appId,
              appSecret,
              returnUrl));
          wr.Flush();
        }
      }

      HttpWebResponse response = (HttpWebResponse)request.GetResponse();
      if (response.StatusCode == HttpStatusCode.OK)
      {
        using (Stream responseStream = response.GetResponseStream())
        {
          DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(OAuth2AccessTokenData));
          OAuth2AccessTokenData data = serializer.ReadObject(responseStream) as OAuth2AccessTokenData;
          if (data != null)
          {
            return data.AccessToken;
          }
        }
      }
      return null;
    }

    protected override IDictionary<string, string> GetUserData(string accessToken)
    {
      GoogleUserData userData;
      using (WebResponse response = WebRequest.Create("https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + Uri.EscapeDataString(accessToken)).GetResponse())
      {
        using (Stream stream = response.GetResponseStream())
        {
          DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GoogleUserData));
          userData = serializer.ReadObject(stream) as GoogleUserData;
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("id", userData.Id);
      dictionary.Add("name", userData.Name);
      dictionary.Add("email", userData.Email);
      return dictionary;
    }
  }

  [DataContract]
  public class GoogleUserData
  {
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "email")]
    public string Email { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }
  }
}