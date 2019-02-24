# AWSCliExtender
Sometimes AWS CLI was not enough to do jobs easily. You have to execute a few commands before to do your real operation. With this extention, i am wrapping the SDK to achieve basic operations much more easily and aiming to do with a single command.

In Alpha stage, there is only one command: 

create-policy-ext: An extention method for create policy.

Extention:
```
Policy JSON string can be read from file path with --policy-document-file-path argument
Automatically remove the oldest policy when you reach five policy versions. if argument --no-remove-oldest-policy-version is set, this feature will be disabled and you have to manually delete the policy version before add a new one.
Automatically create the policy if there is not already created.
```   

Usage:
```
AWSCliExtender.exe iam create-policy-ext --policy-name "policy-tool-test" --policy-document-file-path "D:\\policy.json" --profile AWSPROFILE
```
