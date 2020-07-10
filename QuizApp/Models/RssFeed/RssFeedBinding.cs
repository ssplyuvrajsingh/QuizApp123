using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using QuizApp.Models;
using QuizApp.Models.Entities;

namespace QuizApp.Models
{
    public class RssFeedBinding
    {
        #region Read Rss Feed Url
        public IEnumerable<RSSFeed> RssData(string RSSURL, int PageNo)
        {

            var rssFeed = XDocument.Load(RSSURL);

            int iPageSize = 10;
            var skip = (PageNo - 1) * iPageSize;
            var rssFeedOut = (from item in rssFeed.Descendants("item")
                              select new RSSFeed
                              {
                                  Title = ((string)item.Element("title")),
                                  //Link = ((string)item.Element("link")),

                                  Img = Regex.Match((item.Element("description").Value), "<img[^>]+>").ToString(),
                                  //PubDate = ((string)item.Element("pubDate"))
                              }).Skip(skip).Take(iPageSize);
            if (rssFeedOut.Any())
            {
                return rssFeedOut;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Rss Filter Data
        public RSSFeedDesc RssFilterData(string RSSURL, string Title)
        {
            
            var rssFeed = XDocument.Load(RSSURL);

            var data = rssFeed;
            var rssFeedOut = (from item in rssFeed.Descendants("item")
                              where
                              ((string)item.Element("title")).Trim() == Title.Trim()
                              select new RSSFeedDesc
                              {
                                  Title = ((string)item.Element("title")),
                                  Description = ((string)item.Element("description")),
                              }).FirstOrDefault();
            
            
            if (rssFeedOut != null)
            {
                QuizAppEntities entities = new QuizAppEntities();
                FilterWord filterWord = new FilterWord();
                var filterDataList = entities.FilterWords.ToList();
                for (int i = 0; i < filterDataList.Count; i++)
                {
                    string temp = rssFeedOut.Description;
                    string findString = filterDataList[i].FilterData;             //"Download Dainik Bhaskar App to read Latest Hindi News Today";
                    string filteredData = "";
                    if (temp.Contains(findString))
                    {
                        filteredData = temp.Replace(findString, "");
                    }
                    rssFeedOut.Description = filteredData;
                }
                return rssFeedOut;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}