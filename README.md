# DotNetCore-DigitalSignaturePdf
Free dotNetCore library for digitaly sign pdf files with X509 certificates. This library is a dot net core wrapper on [JSignPdf](http://jsignpdf.sourceforge.net/) java application.
This library already include JSignPdf inside.
You can use this library by using nuget package, or by clone this repo and compile by yourself.

> be aware! java need to be installed on your server to use this library. I added links for windows & linux later.

## How to use

```
            Signer signer = new Signer("D:\\Temp\\", "C:\\Program Files\\Java\\jdk1.8.0_152\\bin\\java.exe");

            string fileAsBase64String = Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\temp\\basic.pdf"));


            string signedFileBase64String = signer.Sign(fileAsBase64String
                                            , "C:/TEMP/cert/eyal-cert.pfx",
                                            "password",
                                            "");

```

## Detailed Explain

* Create instance of Signer class. You need to supply : 
   * (1) Temporary folder (full permissions needed).
   * (2) full path to the java bin file.
* Now prepare base 64 string from your pdf.
* To sign your pdf , use Sign method. You need to supply : 
  * (1) base 64 string of your pdf.
  * (2) full path to your X509 certificate (pfx file). 
  * (3) password to your pfx file - if needed (if not, supply empty string).
  * (4) Password for the signed pdf that will be return. (if you don't want to lock the pdf with a password - supply empty string).
  
 
# Need to generate self sign certificate ? 
Here are the commands ( you need openssl - it is installed by default on most linux distributions, and for windows you can use [openssl for Windows](http://gnuwin32.sourceforge.net/packages/openssl.htm) ).

> If it is your first time you create certificate, just know, that you will be asked for some identification details (Country, city, FQDN = your domain, etc.). Answer these questions as you want. Just remmber that if someone will look on the pdf signature details - these are the details that he/she will see.

* First - create new certificate. 
```
openssl req -x509 -days 365 -newkey rsa:2048 -keyout my-key.pem -out my-cert.pem
```
* Next -  convert the certificate to single pfx file 
```
openssl pkcs12 -export -in my-cert.pem -inkey my-key.pem -out eyal-cert.pfx
```


# Links to install java
* As said before you need java to be installed (JDK).
- For linux you can use this instructions : [Install java on linux](https://www.digitalocean.com/community/tutorials/how-to-install-java-on-centos-and-fedora)
- For Windows you can use [this](https://www.java.com/en/download/)


# Licenses
JSignPdf is released under Mozilla Public License (version 1.1 or later) and GNU LGPL (version 2.1 or later). So this library also use the same licences.
