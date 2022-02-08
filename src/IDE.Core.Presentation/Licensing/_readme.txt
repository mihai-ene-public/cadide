https://www.codeproject.com/Articles/996001/A-Ready-To-Use-Software-Licensing-Solution-in-Csha

What You Need To Do

Step by Step is here for how to use this library. I will use those demo projects as a sample to describ how to create your own application that integrats this license solution.

1. Create your certificates for your application

It is recommended that you create a new set of certificates for each of your new application, this is for security consideration.

1) You can just use makecert command to do this like:

makecert -pe -ss My -sr CurrentUser -$ commercial -n "CN=<YourCertName>" -sky Signature

Replace "<YourCertName>" with your own certificate name.

2) After executing the above command, open your Certificate Management window by runninig "certmgr.msc".

3) Find the created certificate under "Personal" category with the name you specified in <YourAppName> above.

4) Right click on the certificate and select "All Tasks"->"Export"

5) On the Export dialogue, select "Yes, export the private key" and leave the other settings as default.

Image 8

6) On the password diaglogue, input a password to protect the private key. You need to copy this password in your code when using this certificate. E.g., we use password "demo" here.

7) For the file name, we may use "LicenseSign.pfx" for this demo. Now we have the certificate with private key on hand. 

8) Do step 4) to step 7) again to export the public key, just the difference is to choose "No, do not export the private key" and use file name as "LicenseVerify.cer" instead. Leave all the other options as default.

Well, we now have both the private key and public key generated for our solution.