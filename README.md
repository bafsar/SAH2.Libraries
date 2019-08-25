# SAH2.Libraries

## SAH2.Core

This project includes some extension classes. These classes may have been used in other projects in this solution. Also, it can be used anywhere.

 - AssemblyExtensions
 - EnumerableListExtensions
 - ExpandoObjectExtensions
 - ExpressionExtensions
 - ObservableCollectionExtensions

## SAH2.TierArchitecture.Infrastructure
This project presents some classes and interfaces to use as infrastructure on an n-tiered project.

## SAH2.Utilities
This project presents some example utilities classes.

 - IconHelper
 - ImageUtilities

## SAH2.Web

This project presents some extension and functions to use for basic web projects' needs. Also includes a class to get data from Gravatar.

## SAH2.Web.Mail

Simple mail library that you can use easily.

Usage:
From Google:

    var googleMail = new GoogleMail("senderusername@gmail.com", "userPassword", "senderDisplayName", "Message Subject", "Message Body Content");
    
    new MailSender(googleMail, new MailAddress("receiver@provider.ext")).SendMail();
    
From Custom Mail Provider:

    var customMailProvider = new CustomMailProvider("smtp.provider.ext", true, 587);
    var customMail = new CustomMail(customMailProvider, "senderusername@provider.ext", "userPassword", "senderDisplayName", "Message Subject", "Message Body Content");
    
    new MailSender(customMail, new MailAddress("receiver@provider.ext")).SendMail();

***
## SAH2.Web.MVC

This project presents some extension and functions to use in an MVC projects. Also includes an HtmlHelper to get data from Gravatar.


## SAH2.Web.MVC.Utilities.EmbeddedResourcePack

With this project, you can create embedded resources and use them in html with friendly names

To prepare an example, you can follow these steps:

- First, create a new class library project named **"Test.EmbeddedResourcePack.jQuery"** with **use Framework** and add this class:
  >In this example, we used "Test.EmbeddedResourcePack.jQuery" as project name and put "jquery-3.4.1.min.js" file under /libs/js/ directory. Although the project name is Test.EmbeddedResource.jQuery, the assembly's default namespace is Test.EmbeddedResource. Because of this, **we use "Test.EmbeddedResource" string as a part of the file address definer**. If we don't want to use default namespace in the address definer, it still works but there will be a possibility to mix up with another embedded resource. Please be careful with it.  If you want to use another name or directory, remember to change address in the code below!

      namespace Test.EmbeddedResourcePack
      {
          public class jQuery : CustomEmbeddedBundle<jQuery>
          {
               public static MvcHtmlString JsOnLocal => AutoRender("Test.EmbeddedResourcePack/libs/js/jquery-3.4.1.min.js");
               public static MvcHtmlString JsOnCDN => AutoRender("//code.jquery.com/jquery-3.4.1.min.js");
          }
      }

- Second, download jQuery file from [//code.jquery.com/jquery-3.4.1.min.js](//code.jquery.com/jquery-3.4.1.min.js), add into folder that mentioned above and set  the file as Embedded Resource from file properties (Build Action: Embedded resource).

- Now, create a new MVC project using the Framework, reference the above project, and add one of these codes wherever you want to use the jQuery file:

      <!-- The result is: Generated local source address tag -->
      @jQuery.JsOnLocal

      <!-- The result is: Generated CDN address tag -->
      @jQuery.JsOnCDN

That's it. Also, if you want, you can create more complicated embedded resource projects.



## SAH2.XamarinForms
This project presents an experimental class named Request to do request from any api endpoints and also a markup extension to get any image from inside of a referenced assembly to use in Xamarin.Forms projects.

Request class' codes copied from https://github.com/4gus71n/Xamarin.Droid/blob/master/Request.cs and has been changed some. Now, it's experimental and still under development.

Example usage of (edited) Request<T> class (Original codes' usage is different little bit):

    await Request<TModel>.Init()
                         .SetHttpMethod(RequestHttpMethod.Get)
                         .SetEndpoint("endPointUrl")
                         .OnSuccess(serverResponse => { })
	                     .OnError(exception => { })
	                     .OnRequestCompleted(() => { })
	                     .Start();


Example usage of ImageResource markup extension:

    {ext:ImageResource Namespace.Folder.ImageName.ImgExt, ContainerAssemblyRelatedType={x:Type ContainerAssemblyXmlns:AnyClass}}


## SAH2.WPF

 - TextBoxSelectAllMode (AttachedProperties)
		With this AP, you can define which condition on you select text in a textbox.
		
 - GrayscaleEffect (Utility)
		This utility displays images in grayscale.
