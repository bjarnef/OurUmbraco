﻿using System;
using OurUmbraco.Forum.Extensions;
using OurUmbraco.Forum.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace OurUmbraco.Our.Models
{
    public class OurUmbracoTemplatePage : UmbracoTemplatePage
    {
        public MemberData MemberData;

        public OurUmbracoTemplatePage()
        {
            MemberData = GetMemberData();
        }

        public override void Execute()
        {
        }
        
        private static MemberData GetMemberData()
        {
            var membershipHelper = new Umbraco.Web.Security.MembershipHelper(UmbracoContext.Current);

            if (membershipHelper.IsLoggedIn() == false) return null;

            var userName = membershipHelper.CurrentUserName;
            var memberData = ApplicationContext.Current.ApplicationCache.RuntimeCache
                .GetCacheItem<MemberData>("MemberData" + userName, () =>
                {
                    var member = membershipHelper.GetCurrentMember();
                    var avatarImage = Utils.GetMemberAvatarImage(member);

                    var roles = member.GetRoles();

                    var topicService = new TopicService(ApplicationContext.Current.DatabaseContext);
                    var latestTopics = topicService.GetLatestTopicsForMember(member.Id, maxCount: 100);

                    var newTosAccepted = true;
                    var tosAccepted = member.GetPropertyValue<DateTime>("tos");
                    var newTosDate = new DateTime(2016, 09, 01);
                    if ((newTosDate - tosAccepted).TotalDays > 1)
                        newTosAccepted = false;

                    var data = new MemberData
                    {
                        Member = member,
                        AvatarImage = avatarImage,
                        AvatarImageTooSmall = avatarImage != null && (avatarImage.Width < 400 || avatarImage.Height < 400),
                        Roles = roles,
                        LatestTopics = latestTopics,
                        AvatarHtml = Utils.GetMemberAvatar(member, 100),
                        AvatarPath = member.Avatar(),
                        NumberOfForumPosts = member.ForumPosts(),
                        Karma = member.Karma(),
                        TwitterHandle = member.GetPropertyValue<string>("twitter").Replace("@", string.Empty),
                        IsAdmin = member.IsAdmin(),
                        Email = member.GetPropertyValue<string>("Email"),
                        IsBlocked = member.GetPropertyValue<bool>("blocked"),
                        NewTosAccepted = newTosAccepted
                    };

                    return data;
                }, TimeSpan.FromMinutes(5));

            return memberData;
        }
    }
}
