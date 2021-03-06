﻿namespace YahooFinanceAPI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    ///     Class for fetching token (cookie and crumb) from Yahoo Finance
    ///     Copyright Dennis Lee
    ///     3 Nov 2017
    /// </summary>
    public class Token
    {
        private static Regex _regexCrumb;

        public static string Cookie { get; set; }

        public static string Crumb { get; set; }

        /// <summary>
        /// Refresh cookie and crumb value
        /// </summary>
        /// <param name="symbol">Stock ticker symbol.</param>
        /// <returns>Boolean indicating whether the refresh was successful or not</returns>
        public static async Task<bool> RefreshAsync(string symbol = "SPY")
        {
            try
            {
                Cookie = string.Empty;
                Crumb = string.Empty;

                const string UrlScrape = "https://finance.yahoo.com/quote/{0}?p={0}";

                var url = string.Format(UrlScrape, symbol);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.CookieContainer = new CookieContainer();
                request.Method = "GET";

                using (var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false))
                {
                    var cookie = response.GetResponseHeader("Set-Cookie").Split(';')[0];

                    var html = string.Empty;

                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            html = await new StreamReader(stream).ReadToEndAsync().ConfigureAwait(false);
                        }
                    }

                    if (html.Length < 5000)
                    {
                        return false;
                    }

                    var crumb = await GetCrumbAsync(html).ConfigureAwait(false);

                    if (crumb != null)
                    {
                        Cookie = cookie;
                        Crumb = crumb;
                        Debug.Print("Crumb: '{0}', Cookie: '{1}'", crumb, cookie);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return false;
        }

        /// <summary>
        ///     Get crumb value from HTML
        /// </summary>
        /// <param name="html">HTML code</param>
        /// <returns>The crumb.</returns>
        private static Task<string> GetCrumbAsync(string html)
        {
            return Task.Run(
                () =>
                    {
                        string crumb = null;

                        try
                        {
                            // initialize on first time use
                            if (_regexCrumb == null)
                            {
                                _regexCrumb = new Regex("CrumbStore\":{\"crumb\":\"(?<crumb>.+?)\"}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                            }

                            var matches = _regexCrumb.Matches(html);

                            if (matches.Count > 0)
                            {
                                crumb = matches[0].Groups["crumb"].Value;

                                // fixed unicode character 'SOLIDUS'
                                if (crumb.Length != 11)
                                {
                                    crumb = crumb.Replace("\\u002F", "/");
                                }
                            }
                            else
                            {
                                Debug.Print("Regex no match");
                            }

                            // prevent regex memory leak
                            // ReSharper disable once RedundantAssignment
                            matches = null;
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.Message);
                        }

                        GC.Collect();
                        return crumb;
                    });
        }
    }
}