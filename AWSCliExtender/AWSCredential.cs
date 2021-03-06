﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace AWSCliExtender
{
    static class AWSCredential
    {
        public static AWSCredentials Profile;

        private static SharedCredentialsFile _sharedCredentialsFile;

        public static string Account;
        public static string UserId;
        public static string Arn;


        public static void Initialize(string ProfileName)
        {
            _sharedCredentialsFile = new SharedCredentialsFile();
            CredentialProfile _credentialProfile = GetAWSCredentialProfile(ProfileName);
            Profile = new SessionAWSCredentials(_credentialProfile.Options.AccessKey, _credentialProfile.Options.SecretKey, _credentialProfile.Options.Token);    
            GetUserIdentity();
        }

        public static void Initialize(string AccessKey, string SecretKey)
        {
            _sharedCredentialsFile = new SharedCredentialsFile();
            Profile = new BasicAWSCredentials(AccessKey, SecretKey);
            GetUserIdentity();
        }

        private static void GetUserIdentity()
        {
            using (AmazonSecurityTokenServiceClient _stsClient = new AmazonSecurityTokenServiceClient(Profile, RegionEndpoint.USEast1))
            {
                var _response = _stsClient.GetCallerIdentity(new GetCallerIdentityRequest { });
                Account = _response.Account;
                UserId = _response.UserId;
                Arn = _response.Arn;
            }
        }

        private static CredentialProfile GetAWSCredentialProfile(string ProfileName)
        {
            CredentialProfile _credentialProfile;
            if (_sharedCredentialsFile.TryGetProfile(ProfileName, out _credentialProfile))
            {
                return _credentialProfile;
            }
            else
                throw new Exception("There is no Profile name as " + ProfileName);
        }
    }



}
