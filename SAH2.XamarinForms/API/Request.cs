/***********************************************************************************************************
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***                              This class is still under development                                  ***
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***                  MimeSharp Github Address: https://github.com/bafsar/MimeSharp                      ***
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***********************************************************************************************************
 ***********************************************************************************************************
 **********************************************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MimeSharp;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace SAH2.XamarinForms.API
{
    public enum RequestHttpMethod
    {
        Get,
        Post,
        Put,
        Delete
    }

    public class Request<T>
    {
        #region Request<T> Initializer

        public static Request<T> Init()
        {
            return new Request<T>();
        }

        #endregion


        #region Request Maker

        /// <summary>
        ///     Makes a request and deserializes the result JSON as T class objects.
        ///     Check https://forums.xamarin.com/discussion/22732/3-pcl-rest-functions-post-get-multipart
        /// </summary>
        /// <param name="method">The http method can be "GET", "POST", "PUT" or "DELETE"</param>
        /// <param name="endpoint">The endpoint name i.e. "/api/v1/feed"</param>
        /// <param name="body">If the method is GET, the query string URI to set in the URL. Otherwise the json body.</param>
        /// <param name="authToken">The AUTH_TOKEN cookie.</param>
        private async Task MakeRequest(string method, string endpoint, string body, string authToken = null)
        {
#if DEBUG
            Debug.WriteLine("Hitting: " + endpoint);
            Debug.WriteLine("Body payload: " + body);
#endif

            RequestStartedHandler?.Invoke();
            if (!CrossConnectivity.Current.IsConnected)
            {
                HandleNoConnectivity();
                return;
            }

            try
            {
                //If the method is GET we've to concat the query string uri, i.e. "/feeds" + "?id=something"
                if (method.Equals(HttpMethod.Get.Method) && !string.IsNullOrEmpty(body)) endpoint += body;
                var request = (HttpWebRequest)WebRequest.Create(new Uri(endpoint));
                SetHeaders(request);
                request.Method = method;
                if (!string.IsNullOrEmpty(authToken)) request.Headers["Cookie"] = authToken;

                if (method.Equals(HttpMethod.Post.Method) || method.Equals(HttpMethod.Put.Method))
                {
                    if (!string.IsNullOrEmpty(body))
                    {
                        var requestStream = await request.GetRequestStreamAsync();
                        using (var writer = new StreamWriter(requestStream))
                        {
                            writer.Write(body);
                            writer.Flush();
                            writer.Dispose();
                        }
                    }

                    if (string.IsNullOrWhiteSpace(_filePath) == false && File.Exists(_filePath))
                    {
                        //  Post Multi-part data
                        var fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Read);
                        //  Expected
                        //  Header
                        //  Content-Length: 18101
                        //  Content-Type: multipart/form-data; boundary = ---------------------------13455211745882
                        //  Cookie: AUTH-TOKEN=eyJhbGciOiJIUz
                        //  Body
                        //  -----------------------------13455211745882
                        //  Content-Disposition: form-data; name="file"; filename="Feed List View.png"
                        //  Content-Type: image/png
                        //  Byte body
                        //  -----------------------------13455211745882--

                        var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

                        request.ContentType = "multipart/form-data; boundary=" + boundary;

                        request.Credentials = CredentialCache.DefaultCredentials;

                        var requestStream = await request.GetRequestStreamAsync();

                        var headerTemplate =
                            "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n";

                        var header = string.Format(headerTemplate, boundary, _fileParameterName, _fileName,
                            _fileContentType);

                        Debug.WriteLine(header);

                        var headerbytes = Encoding.UTF8.GetBytes(header);

                        using (var requestStreamWriter = new BinaryWriter(requestStream))
                        {
                            requestStreamWriter.Write(headerbytes, 0, headerbytes.Length);

                            var fileByteStream = ReadFully(fileStream);
                            Debug.WriteLine("Bytes read:" + fileByteStream.Length);
                            requestStreamWriter.Write(fileByteStream, 0, fileByteStream.Length);

                            var trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--");
                            requestStreamWriter.Write(trailer, 0, trailer.Length);

                            Debug.WriteLine("(Using) Request ContentType: " + request.ContentType);
                        }
                    }
                }

                Debug.WriteLine("Request ContentType: " + request.ContentType);

                var response = (HttpWebResponse)await request.GetResponseAsync();

                Debug.WriteLine("Response Content-Lenght: " + response.ContentLength);

                HeaderResultHandler?.Invoke(response.Headers);
                var respStream = response.GetResponseStream();

                using (var sr = new StreamReader(respStream))
                {
                    //Need to return this response 
                    var stringResponse = sr.ReadToEnd();
                    Debug.WriteLine("Json response: " + stringResponse);
                    var result = JsonConvert.DeserializeObject<T>(stringResponse);
                    SuccessHandler?.Invoke(result);
                    RequestCompletedHandler?.Invoke();
                }
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    HandleUnknownError(new Exception("Null response"));
                    return;
                }

#if DEBUG
                using (var stream = e.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Debug.WriteLine("Server-side error:" + reader.ReadToEnd());
                }
#endif
                HandleWebExceptionError(e);
            }
            catch (JsonSerializationException e)
            {
                HandleJsonError(e);
            }
            catch (Exception e)
            {
                HandleUnknownError(e);
            }
        }

        #endregion


        #region Basic http methods

        /// <summary>
        ///     Configures the headers for every request that we make
        /// </summary>
        private static void SetHeaders(HttpWebRequest request)
        {
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=UTF-8";
            request.Headers["Pragma"] = "no-cache";
            request.Headers["Cache-Control"] = "no-cache";
            request.Headers["Accept-Encoding"] = "gzip, deflate, sdch";
            request.Headers["Upgrade-Insecure-Requests"] = "1";
            //request.Headers["Accept-Language"] = "es-419,es;q=0.8";
        }

        #endregion


        #region Request configuration variables

        private string _filePath;

        private string _httpMethod,
            _requestEndpoint,
            _jsonPayload,
            _authenticationToken,
            _fileName,
            _fileContentType,
            _fileParameterName,
            _jsonPayloadName;

        #endregion


        #region Request<T> Setter Methods

        public Request<T> SetFile(string filePath, string fileParameterName)
        {
            _filePath = filePath;
            _fileParameterName = fileParameterName;
            _fileName = filePath.Split('/').Last();
            _fileContentType = Mime.Lookup(_fileName);
            return this;
        }

        /// <summary>
        ///     Sets the http method that we are goint to use
        /// </summary>
        /// <param name="httpMethod">Can be any of HttpMethod.*.Method</param>
        public Request<T> SetHttpMethod( /*string*/ RequestHttpMethod httpMethod)
        {
            _httpMethod = httpMethod.ToString();
            return this;
        }

        /// <summary>
        ///     Sets the endpoint that we're gonna hit
        /// </summary>
        /// <param name="requestEndpoint">Any endpoint from the Settings class</param>
        public Request<T> SetEndpoint(string requestEndpoint)
        {
            _requestEndpoint = requestEndpoint;
            return this;
        }

        public Request<T> SetAuthenticationToken(string authenticationToken)
        {
            _authenticationToken = authenticationToken;
            return this;
        }

        /// <summary>
        ///     Sets the raw body content. If a jsonPayloadName variable is passed, the jsonPayload will be set into a
        ///     form with jsonPayloadName as the name of the form field.
        /// </summary>
        public Request<T> SetJsonPayloadBody(string jsonPayload, string jsonPayloadName = null)
        {
            _jsonPayload = jsonPayload;
            _jsonPayloadName = jsonPayloadName;
            return this;
        }

        #endregion


        #region Helper Methods

        private async void WriteRequestStream(HttpWebRequest request, string body)
        {
            var stream = await request.GetRequestStreamAsync();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(body);
                writer.Flush();
                writer.Dispose();
            }
        }

        //See http://stackoverflow.com/a/221941/1403997
        private byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) != 0) ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        #endregion


        #region Internal Helper Handlers

        private void HandleJsonError(Exception e)
        {
            JsonErrorHandler?.Invoke(e);
            ErrorHandler?.Invoke(e);
            RequestCompletedHandler?.Invoke();
        }

        private void HandleUnknownError(Exception e)
        {
            UnknownErrorHandler?.Invoke(e);
            ErrorHandler?.Invoke(e);
            RequestCompletedHandler?.Invoke();
        }

        private void HandleWebExceptionError(WebException e)
        {
            var response = (HttpWebResponse)e.Response;
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                ErrorHandler?.Invoke(e);
                HttpErrorHandler?.Invoke(response.StatusCode);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        BadRequestErrorHandler?.Invoke();
                        break;
                    case HttpStatusCode.InternalServerError:
                        InternalServerErrorHandler?.Invoke();
                        break;
                    case HttpStatusCode.RequestTimeout:
                        TimeOutHandler?.Invoke();
                        break;
                    case HttpStatusCode.NotFound:
                        NotFoundHandler?.Invoke();
                        break;
                    case HttpStatusCode.Unauthorized:
                        UnauthorizeHandler?.Invoke();
                        break;
                }
            }

            RequestCompletedHandler?.Invoke();
        }


        private void HandleNoConnectivity()
        {
            //If there's no internet connection
            ErrorHandler?.Invoke(new WebException("No internet connection"));
            NoInternetConnectionHandler?.Invoke();
            RequestCompletedHandler?.Invoke();
        }

        #endregion


        #region Request's lifecycle methods

        protected Action<T> SuccessHandler;

        protected Action<Exception> ErrorHandler, UnknownErrorHandler, JsonErrorHandler;

        protected Action<WebHeaderCollection> HeaderResultHandler;

        protected Action InternalServerErrorHandler,
            UnauthorizeHandler,
            TimeOutHandler,
            AuthTokenErrorHandler,
            NotFoundHandler,
            BadRequestErrorHandler,
            RequestCompletedHandler,
            RequestStartedHandler,
            NoInternetConnectionHandler;

        protected Action<HttpStatusCode> HttpErrorHandler;

        #endregion


        #region Builder object methods

        /// <summary>
        ///     Starts the request. Returns a task that can be awaited. The task has the response model within.
        /// </summary>
        public virtual Task Start()
        {
            return MakeRequest(_httpMethod, _requestEndpoint, _jsonPayload, _authenticationToken);
        }


        #region Handlers

        /// <summary>
        ///     Triggered if there's no internet connection in the device when we make the request.
        /// </summary>
        public Request<T> OnNoInternetConnection(Action handler)
        {
            NoInternetConnectionHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered always no matter what, when the request is completed.
        /// </summary>
        public Request<T> OnRequestCompleted(Action handler)
        {
            RequestCompletedHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered always no matter what, at the very begining of the request.
        /// </summary>
        public Request<T> OnRequestStarted(Action handler)
        {
            RequestStartedHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered if the request response returned 400.
        /// </summary>
        public Request<T> OnBadRequestError(Action handler)
        {
            BadRequestErrorHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered if an unhandled exception was fired when doing the request.
        /// </summary>
        /// <param name="handler">The exception thrown</param>
        public Request<T> OnJsonError(Action<Exception> handler)
        {
            JsonErrorHandler = handler;
            return this;
        }

        /// <summary>
        ///     Triggered if an unhandled exception was fired when doing the request.
        /// </summary>
        /// <param name="handler">The exception thrown</param>
        public Request<T> OnUnknownError(Action<Exception> handler)
        {
            UnknownErrorHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered when we retrieved the headers of the request response. Hook from this method if you want to
        ///     get something from the response headers i.e. the "Set-Cookie" header.
        /// </summary>
        /// <param name="handler">The response headers</param>
        public Request<T> OnHeaderResult(Action<WebHeaderCollection> handler)
        {
            HeaderResultHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered when the request response is properly parsed and returned status code 200.
        /// </summary>
        /// <param name="handler">The model response</param>
        public Request<T> OnSuccess(Action<T> handler)
        {
            SuccessHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered when the request response returned 401
        /// </summary>
        public Request<T> OnNotFound(Action handler)
        {
            NotFoundHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered every time that an http error ocurred (400, 401, 501, etc.)
        /// </summary>
        /// <param name="handler">The http status code</param>
        public Request<T> OnHttpError(Action<HttpStatusCode> handler)
        {
            HttpErrorHandler = handler;
            return this;
        }


        /// <summary>
        ///     Triggered if the token is invalid at the time that we're trying to make the request.
        /// </summary>
        public Request<T> OnAuthTokenError(Action handler)
        {
            AuthTokenErrorHandler = handler;
            return this;
        }

        /// <summary>
        ///     Triggered if the request response caused an unauthorized error
        /// </summary>
        public Request<T> OnUnauthorize(Action handler)
        {
            UnauthorizeHandler = handler;
            return this;
        }

        /// <summary>
        ///     Triggered if the request response caused a timeout error.
        /// </summary>
        public Request<T> OnTimeOut(Action handler)
        {
            TimeOutHandler = handler;
            return this;
        }

        /// <summary>
        ///     Executed every time that an error ocurred
        /// </summary>
        public Request<T> OnError(Action<Exception> handler)
        {
            ErrorHandler = handler;
            return this;
        }

        /// <summary>
        ///     Triggered if the request response is a 501 status code
        /// </summary>
        public Request<T> OnInternalServerError(Action handler)
        {
            InternalServerErrorHandler = handler;
            return this;
        }

        #endregion

        #endregion
    }
}
