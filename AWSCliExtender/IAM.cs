using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;

namespace AWSCliExtender
{
    static class IAM
    {
        private static AmazonIdentityManagementServiceClient _iamClient;

        /// <summary>
        /// Function is combining CreatePolicy and CreatePolicyVersion commands to the one and extend their functionalities.
        /// 
        /// <para>Creates a new managed policy for your AWS account. This operation creates a policy version with a version identifier of v1 and sets v1 as the policy's default version. For more information about policy versions, see Versioning for Managed Policies in the IAM User Guide . 
        /// https://docs.aws.amazon.com/cli/latest/reference/iam/create-policy.html
        /// </para>
        /// 
        /// <para>Creates a new version of the specified managed policy. To update a managed policy, you create a new policy version. A managed policy can have up to five versions. If the policy has five versions, you must delete an existing version using DeletePolicyVersion before you create a new version.
        /// https://docs.aws.amazon.com/cli/latest/reference/iam/create-policy-version.html
        /// </para>
        /// 
        /// Extention:
        /// <para>Policy JSON string can be read from file path with --policy-document-file-path argument</para>
        /// <para>Automatically remove the oldest policy when you reach five policy versions. if argument --no-remove-oldest-policy-version is set, this feature will be disabled and you have to manually delete the policy version before add a new one.</para>
        /// <para>Automatically create the policy if there is not already created.</para>
        /// </summary>
        public static void CreatePolicyExt(string PolicyName, string PolicyFilePath, string Path = null, bool NoSetAsDefault = false, bool NoRemoveOldestPolicyVersion = false)
        {
            try
            {
                _iamClient = new AmazonIdentityManagementServiceClient(AWSCredential.Profile, RegionEndpoint.USEast1);

                var policy = System.IO.File.ReadAllText(PolicyFilePath);

                try
                {
                    _iamClient.CreatePolicy(new CreatePolicyRequest { PolicyName = PolicyName, PolicyDocument = policy, Path = Path ?? null });
                }
                catch (Amazon.IdentityManagement.Model.EntityAlreadyExistsException ex)
                {
                    // Policy is already created. Create a new policy version and set.
                    Console.WriteLine(ex.Message + " Trying to create a new policy version.");

                    string _policyARN = "arn:aws:iam::" + AWSCredential.Account + ":policy" + (Path ?? "/") + PolicyName;

                    Console.WriteLine(_policyARN);

                    var _policyList = _iamClient.ListPolicyVersions(new ListPolicyVersionsRequest { PolicyArn = _policyARN });
                    if ((_policyList.Versions.Count == 5) && (!(NoRemoveOldestPolicyVersion)))
                    {
                        /// Policy has maximum policy versions. Oldest one has to be delete to continue.
                        var _oldestPolicyVersion = _policyList.Versions.Where(q => (!(q.IsDefaultVersion))).OrderBy(q => q.CreateDate).Select(q => q.VersionId).First();
                        _iamClient.DeletePolicyVersion(new DeletePolicyVersionRequest { PolicyArn = _policyARN, VersionId = _oldestPolicyVersion });
                        Console.WriteLine("The oldest policy version is deleted: " + _oldestPolicyVersion);
                    }
                    _iamClient.CreatePolicyVersion(new CreatePolicyVersionRequest { PolicyArn = _policyARN, PolicyDocument = policy, SetAsDefault = (!(NoSetAsDefault)) });
                    Console.WriteLine("Policy is created.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("********ERR: " + ex.Message);
            }
            finally
            {
                _iamClient.Dispose();
            }
        }


        private static List<ManagedPolicy> AWSListPolicies()
        {
            var _return = new List<ManagedPolicy>();

            var _getPolicies = _iamClient.ListPolicies(new ListPoliciesRequest { });
            while (_getPolicies.IsTruncated)
            {
                _getPolicies = _iamClient.ListPolicies(new ListPoliciesRequest { Marker = _getPolicies.Marker });
                _return.AddRange(_getPolicies.Policies);
            }

            return _return;
        }
    }
}
