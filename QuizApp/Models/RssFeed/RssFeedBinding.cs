using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using QuizApp.Models;

namespace QuizApp.Models
{
    public class RssFeedBinding
    {
        public IEnumerable<RSSFeed> RssData(string RSSURL, int PageNo)
        {

            var rssFeed = XDocument.Load(RSSURL);

            int iPageSize = 5;
            var skip = (PageNo - 1) * iPageSize;
            var rssFeedOut = (from item in rssFeed.Descendants("item")
                              select new RSSFeed
                              {
                                  Title = ((string)item.Element("title")),
                                  Link = ((string)item.Element("link")),
                                  Description = ((string)item.Element("description")), 
                                  PubDate = ((string)item.Element("pubDate")) 
                              }).Skip(skip).Take(iPageSize);
            if (rssFeedOut.Any())
            {
                return rssFeedOut;
            }
            else
            {
                return null;
            }
            #region Test
            //WebClient wclient = new WebClient();
            //string RSSData = wclient.DownloadString(RSSURL);

            //XDocument xml = XDocument.Parse(RSSData);
            //int iPageSize = 5;
            //var skip = (PageNo - 1) * iPageSize;
            //var RSSFeedData = (from x in xml.Descendants("item")
            //                   select new RSSFeed
            //                   {
            //                       Title = ((string)x.Element("title")),
            //                       Link = ((string)x.Element("link")),
            //                       Description = ((string)x.Element("description")),
            //                       PubDate = ((string)x.Element("pubDate"))
            //                   }).Skip(skip).Take(iPageSize);
            //if (RSSFeedData.Any())
            //{
            //    return RSSFeedData;
            //}
            //else
            //{
            //    return null;
            //}
            #endregion
        }
    }
}