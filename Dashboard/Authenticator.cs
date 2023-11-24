﻿/******************************************************************************
<<<<<<< HEAD
* Filename    = Authenticator.cs
*
* Author      = Sushma kamuju
*

* Description = ViewModel for the Authentication Module which will authorize the app to use client information.
* 
*****************************************************************************/
=======
 * Filename    = Authenticator.cs
 *
 * Author      = Sushma Kamuju
 *
 * Product     = Analyzer
 * 
 * Project     = Dashboard
 *
 * Description = Implements authentication.
 *****************************************************************************/
>>>>>>> 1ac98df4c16c72acb4ad42cb25998ec60ae44909

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Dashboard
{
    /// <summary>
    /// Handles Google OAuth2 authentication.
    /// </summary>
    public static class Authenticator
    {
        private static readonly string s_clientId = "311678972089-oro9svmgd6utrgqs5ttugdgslfarp0i6.apps.googleusercontent.com";
        private static readonly string s_clientSecret = "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";
        private static readonly string s_authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";

        private static string s_userName = "";
        private static string s_userEmail = "";
        private static string s_imageName = "";

        /// <summary>
        /// Initiates the Google OAuth2 authentication process to obtain user credentials for accessing Google services.
        /// </summary>
        /// <param name="timeOut">The maximum time (in milliseconds) to wait for the authentication process to complete. The default timeout is 180,000 milliseconds (3 minutes).</param>
        /// <returns>
        /// A list of strings representing user-specific information or access tokens. The content of the list may vary based on the authentication and authorization flow.
        /// </returns>
        public static async Task<AuthenticationResult> Authenticate( int timeOut = 180000 )
        {

            Trace.WriteLine( "[Authenticator] Creating State and Redirecting URI on port 8080" );
            // Creating state and redirect URI using port 8080 on Loopback address
            string state = CryptRandomInt( 32 );
            string codeVerifier = CryptRandomInt( 32 );
            string codeChallenge = EncodeInputBuffer( Sha256( codeVerifier ) );
            const string codeChallengeMethod = "S256";
            string redirectURI = string.Format( "http://{0}:{1}/" , IPAddress.Loopback , "8080" );
            AuthenticationResult result = new();

            Trace.WriteLine( "[Authenticator] Creating HTTP Listener" );
            // Creating HTTP listener
            HttpListener httpListener = new();
            httpListener.Prefixes.Add( redirectURI );
            Trace.WriteLine( "[Authenticator] Listening on HTTP" );
            httpListener.Start();

            Trace.WriteLine( "[Authenticator] Sending Authorization Request" );
            // Creating an authorization request for OAuth 2.0
            string authorizationRequest = string.Format( "{0}?response_type=code&scope=openid%20email%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}" ,
                    s_authorizationEndpoint ,
                    Uri.EscapeDataString( redirectURI ) ,
                    s_clientId ,
                    state ,
                    codeChallenge ,
                    codeChallengeMethod );

            // Trying to open the request in a browser  
            try
            {
                Process.Start( new ProcessStartInfo( authorizationRequest ) { UseShellExecute = true } );
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                {
                    Trace.WriteLine( "[Authenticator] Error finding browser" );
                }
            }
            catch (Exception other)
            {
                Trace.WriteLine( other.Message );
            }

            // Sending HTTP request to browser and displaying response
            Task<HttpListenerContext> taskData = httpListener.GetContextAsync();

            // If no response is recorded, then we do a timeout after 3 minutes.
            // This may happen because of closing the browser
            if (await Task.WhenAny( taskData , Task.Delay( timeOut ) ) != taskData)
            {
                Trace.WriteLine( "[Authenticator] Timeout occurred before getting response" );
                httpListener.Stop();
                result.IsAuthenticated = false;
                return result;
            }

            HttpListenerContext context = taskData.Result;
            HttpListenerResponse response = context.Response;
            string responseString = string.Format( "<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body><center><h1>Authentication is completed!</h1></center></body></html>" );
            byte[] buffer = Encoding.UTF8.GetBytes( responseString );
            response.ContentLength64 = buffer.Length;
            Stream responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync( buffer , 0 , buffer.Length ).ContinueWith( ( task ) =>
            {
                responseOutput.Close();
                httpListener.Stop();
                Trace.WriteLine( "[Authenticator] HTTP server stopped." );
            } );

            // In case of errors, return to Sign In window
            if (context.Request.QueryString.Get( "error" ) != null)
            {
                Trace.WriteLine( string.Format( "OAuth authorization error: {0}." , context.Request.QueryString.Get( "error" ) ) );
                result.IsAuthenticated = false;
                return result;
            }

            if (context.Request.QueryString.Get( "code" ) == null || context.Request.QueryString.Get( "state" ) == null)
            {
                Trace.WriteLine( "[Authenticator] Malformed authorization response. " + context.Request.QueryString );
                result.IsAuthenticated = false;
                return result;
            }

            // Extracting code and state
            string? code = context.Request.QueryString.Get( "code" );
            string? incomingState = context.Request.QueryString.Get( "state" );

            // Comparing state to expected value
            if (incomingState != state)
            {
                Trace.WriteLine( string.Format( "Received request with invalid state ({0})" , incomingState ) );
                result.IsAuthenticated = false;
                return result;
            }

            Trace.WriteLine( "[Authenticator] No Errors Occurred" );
            // A new thread to wait for the GetUserData to get all required information
            Task task = Task.Factory.StartNew( () => GetUserData( code , codeVerifier , redirectURI ) );
            task.Wait();

            result.IsAuthenticated = true;
            while (s_userName == "" || s_userEmail == "" || s_imageName == "")
            {
                // Thread sleeps until information is received
                Thread.Sleep( 100 );
            }
            result.UserName = s_userName;
            result.UserEmail = s_userEmail;
            result.UserImage = s_imageName;
            return result;
        }

        /// <summary>
        /// Encodes the input buffer into a non-padded base64 URL format.
        /// </summary>
<<<<<<< HEAD
        /// <param name="buffer">The byte array to be encoded.</param>
        /// <returns>A string representing the encoded base64 URL.</returns>
        private static string EncodeInputBuffer(byte[] buffer)
=======
        /// <param name="buffer"></param>
        /// <returns>String: Encoded Base 64</returns>
        private static string EncodeInputBuffer( byte[] buffer )
>>>>>>> 1ac98df4c16c72acb4ad42cb25998ec60ae44909
        {
            string base64 = Convert.ToBase64String( buffer );
            // Converts base64 to base64url.
            base64 = base64.Replace( "+" , "-" );
            base64 = base64.Replace( "/" , "_" );
            // Strips padding.
            base64 = base64.Replace( "=" , "" );
            return base64;
        }

        /// <summary>
        /// Computes the SHA256 hash for the input string.
        /// </summary>
<<<<<<< HEAD
        /// <param name="inputString">The input string to be hashed.</param>
        /// <returns>A byte array representing the SHA256 hash.</returns>
        private static byte[] Sha256(string inputString)
=======
        /// <param name="inputString"></param>
        /// <returns>Byte Array: Sha256 Hashing</returns>
        private static byte[] Sha256( string inputString )
>>>>>>> 1ac98df4c16c72acb4ad42cb25998ec60ae44909
        {
            byte[] bytes = Encoding.ASCII.GetBytes( inputString );
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash( bytes );
        }

        /// <summary>
        /// Generates a random cryptographic number of the specified length.
        /// </summary>
<<<<<<< HEAD
        /// <param name="length">The length of the random number in bytes.</param>
        /// <returns>A base64-encoded representation of the random cryptographic number.</returns>

        private static string CryptRandomInt(uint length)
=======
        /// <param name="length"></param>
        /// <returns>Number in base64 representation.</returns>
        private static string CryptRandomInt( uint length )
>>>>>>> 1ac98df4c16c72acb4ad42cb25998ec60ae44909
        {
            byte[] bytes = new byte[length];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes( bytes );
            return EncodeInputBuffer( bytes );
        }

        /// <summary>
        /// Connects to the OAuth client, requests a token, and retrieves the profile information of the authenticated user.
        /// </summary>
<<<<<<< HEAD
        /// <param name="code">The authorization code received from the OAuth authorization response.</param>
        /// <param name="code_verifier">The code verifier used for the OAuth PKCE (Proof Key for Code Exchange) flow.</param>
        /// <param name="redirectURI">The redirect URI used during the OAuth authorization request.</param>

        private static async void GetUserData(string code, string code_verifier, string redirectURI)
=======
        /// <param name="code"></param>
        /// <param name="code_verifier"></param>
        /// <param name="redirectURI"></param>
        private static async void GetUserData( string code , string code_verifier , string redirectURI )
>>>>>>> 1ac98df4c16c72acb4ad42cb25998ec60ae44909
        {
            Trace.WriteLine( "[Authenticator] Getting Data From User" );
            // Building the  request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody = string.Format( "code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code" ,
                code ,
                Uri.EscapeDataString( redirectURI ) ,
                s_clientId ,
                code_verifier ,
                s_clientSecret
                );

            // Sending the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create( tokenRequestURI );
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes( tokenRequestBody );
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync( _byteVersion , 0 , _byteVersion.Length );
            stream.Close();

            try
            {
                // Getting the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();

                using StreamReader reader = new( tokenResponse.GetResponseStream() );
                // Reading response body
                string responseText = await reader.ReadToEndAsync();
                Trace.WriteLine( responseText );
                // Converting to dictionary
                Dictionary<string , string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string , string>>( responseText );
                string access_token = tokenEndpointDecoded["access_token"];
                UserInfoCall( access_token );
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        Trace.WriteLine( "[Authenticator] HTTP: " + response.StatusCode );
                        using StreamReader reader = new( response.GetResponseStream() );
                        // Reading response body
                        string responseText = await reader.ReadToEndAsync();
                        Trace.WriteLine( responseText );
                    }
                }
            }

        }

        /// <summary>
        /// Uses the access token to extract the required information.
        /// </summary>
<<<<<<< HEAD
        /// <param name="access_token">The OAuth access token obtained after successful authentication.</param>

        private static async void UserInfoCall(string access_token)
=======
        /// <param name="access_token"></param>
        private static async void UserInfoCall( string access_token )
>>>>>>> 1ac98df4c16c72acb4ad42cb25998ec60ae44909
        {
            // Building the  request
            string userInfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

            // Sending the request
            HttpWebRequest userInfoRequest = (HttpWebRequest)WebRequest.Create( userInfoRequestURI );
            userInfoRequest.Method = "GET";
            userInfoRequest.Headers.Add( string.Format( "Authorization: Bearer {0}" , access_token ) );
            userInfoRequest.ContentType = "application/x-www-form-urlencoded";
            userInfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // Getting the response
            WebResponse userInfoResponse = await userInfoRequest.GetResponseAsync();
            using StreamReader userInfoResponseReader = new( userInfoResponse.GetResponseStream() );
            // Reading response body
            string userInfoResponseText = await userInfoResponseReader.ReadToEndAsync();
            Trace.WriteLine( "[Authenticator] USER INFO:\n" + userInfoResponseText );
            JObject json = JObject.Parse( userInfoResponseText );
            s_userName = json["name"].ToString();
            s_userEmail = json["email"].ToString();
            s_imageName = json["picture"].ToString();
        }
    }
}
