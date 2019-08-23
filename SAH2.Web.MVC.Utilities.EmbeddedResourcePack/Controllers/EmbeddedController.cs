using System.Web.Mvc;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Basics;

namespace SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Controllers
{
    public class EmbeddedController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public FileStreamResult Get(string assembly, string resource, string folders)
        {
            SetFromQueryStringIfEmpty(ref assembly, nameof(assembly));
            SetFromQueryStringIfEmpty(ref resource, nameof(resource));
            SetFromQueryStringIfEmpty(ref folders, nameof(folders));

            return EmbeddedResource.Get(assembly, resource, folders);
        }

        #region Helpers

        private void SetFromQueryStringIfEmpty(ref string strParam, string queryStringKey)
        {
            if (string.IsNullOrWhiteSpace(strParam))
                strParam = Request?.QueryString[queryStringKey];
        }

        #endregion
    }
}