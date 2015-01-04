// Copyright© 2014 Jeroen Stemerdink. All Rights Reserved.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

namespace EPiServer.Libraries.Localization.Models
{
    /// <summary>
    ///     Class BingAccessToken.
    /// </summary>
    public class BingAccessToken
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the access_token.
        /// </summary>
        /// <value>The access_token.</value>
        public string access_token { get; set; }

        /// <summary>
        ///     Gets or sets the expires_in.
        /// </summary>
        /// <value>The expires_in.</value>
        public string expires_in { get; set; }

        /// <summary>
        ///     Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public string scope { get; set; }

        /// <summary>
        ///     Gets or sets the token_type.
        /// </summary>
        /// <value>The token_type.</value>
        public string token_type { get; set; }

        #endregion
    }
}