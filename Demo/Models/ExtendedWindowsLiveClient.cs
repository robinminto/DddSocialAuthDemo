using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using WebMatrix.Security.Clients;

namespace Demo.Models
{
  public class ExtendedWindowsLiveClient : OAuth2Client
  {
    private readonly string appId;
    private readonly string appSecret;

    public ExtendedWindowsLiveClient(string appId, string appSecret) : base("windowslive")
    {
      this.appId = appId;
      this.appSecret = appSecret;
    }

    protected override Uri GetServiceLoginUrl(Uri returnUrl)
    {
      UriBuilder builder = new UriBuilder("https://oauth.live.com/authorize");
      builder.Query = string.Format("client_id={0}&scope=wl.basic wl.emails&response_type=code&redirect_uri={1}",
        appId,
        returnUrl
        );
      return builder.Uri;
    }

    protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
    {
      UriBuilder builder = new UriBuilder("https://oauth.live.com/token");
      builder.Query = string.Format("client_id={0}&client_secret={1}&code={2}&redirect_uri={3}&grant_type=authorization_code",
        appId,
        appSecret,
        authorizationCode,
        returnUrl
        );
      WebRequest request = WebRequest.Create(builder.Uri.AbsoluteUri);
      request.Method = "GET";
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
      ExtendedWindowsLiveUserData userData;
      using (WebResponse response = WebRequest.Create("https://apis.live.net/v5.0/me?access_token=" + Uri.EscapeDataString(accessToken)).GetResponse())
      {
        using (Stream stream = response.GetResponseStream())
        {
          DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ExtendedWindowsLiveUserData));
          userData = serializer.ReadObject(stream) as ExtendedWindowsLiveUserData;
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("id", userData.Id);
      dictionary.Add("username", userData.Name);
      dictionary.Add("name", userData.Name);
      dictionary.Add("email", userData.Emails.Preferred);
      return dictionary;
    }
  }

  [DataContract]
  [KnownType(typeof(ExtendedWindowsLiveUserDataEmails))]
  public class ExtendedWindowsLiveUserData : WindowsLiveUserData
  {
    [DataMember(Name = "locale")]
    public string Locale { get; set; }

    [DataMember(Name = "emails")]
    public ExtendedWindowsLiveUserDataEmails Emails { get; set; }
  }

  [DataContract]
  public class ExtendedWindowsLiveUserDataEmails : WindowsLiveUserData
  {
    [DataMember(Name = "preferred")]
    public string Preferred { get; set; }

    [DataMember(Name = "account")]
    public string Account { get; set; }

    [DataMember(Name = "personal")]
    public string Personal { get; set; }

    [DataMember(Name = "business")]
    public string Business { get; set; }
  }
}