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

## SAH2.Web.MVC
This project presents some extension and functions to use in an MVC projects. Also includes an HtmlHelper to get data from Gravatar.


## SAH2.Web.MVC.Utilities.EmbeddedResourcePack
###### Soon...




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


##
## SAH2.WPF

 - TextBoxSelectAllMode (AttachedProperties)
		With this AP, you can define which condition on you select text in a textbox.
		
 - GrayscaleEffect (Utility)
		This utility displays images in grayscale.
