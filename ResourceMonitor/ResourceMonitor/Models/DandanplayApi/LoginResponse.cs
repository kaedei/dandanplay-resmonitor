using System;

namespace ResourceMonitor.Models.DandanplayApi
{
    /*
{
  "registerRequired": true,
  "userId": 0,
  "userName": "string",
  "legacyTokenNumber": 0,
  "token": "string",
  "tokenExpireTime": "2020-06-21T09:30:21.927Z",
  "userType": "string",
  "screenName": "string",
  "profileImage": "string",
  "appScope": "string",
  "errorCode": 0,
  "success": true,
  "errorMessage": "string"
}
     *
     * 
     */
    public class LoginResponse : ResponseBase
    {
        public bool registerRequired { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public string token { get; set; }
        public DateTime tokenExpireTime { get; set; }
        public string userType { get; set; }
        public string screenName { get; set; }
        public string appScope { get; set; }
        
        public UserPrivileges privileges { get; set; }
    }
}