using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Luo.Shared.Data;
using Luo.Shared.Service;
using Luo.Shared.Helper;
using Luo.Shared.PlaybackEngine;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using System.Runtime.Serialization;
using System.Threading;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using System.Net;
using Windows.ApplicationModel.UserActivities;
using AdaptiveCards;
using Windows.UI.Shell;

namespace LuoMusic.ViewModel
{
    public class TimeLineHelper
    {
        // To support Timeline, we need to record user activity and create an adaptive card.
        public static UserActivitySession _currentActivity;
        public static AdaptiveCard apodTimelineCard;

        public async static void CreatVolPlayTimelineAsync(LuoVol volItem)
        {
            // First create the adaptive card.
            CreateAdaptiveCardForTimeline(volItem);

            // Second record the user activity.
            await GenerateActivityAsync(volItem);
        }

        private static void CreateAdaptiveCardForTimeline(LuoVol volItem)
        {
            // Create an adaptive card specifically to reference this app in Windows 10 Timeline.
            apodTimelineCard = new AdaptiveCard("1.0")
            {
                // Select a good background image.
                BackgroundImage = new Uri(volItem.Cover)
            };

            // Add a heading to the card, which allows the heading to wrap to the next line if necessary.
            var apodHeading = new AdaptiveTextBlock
            {
                Text = "Vol." + volItem.VolNum + " " + volItem.Title,
                Size = AdaptiveTextSize.Large,
                Weight = AdaptiveTextWeight.Bolder,
                Wrap = true,
                MaxLines = 2
            };
            apodTimelineCard.Body.Add(apodHeading);

            // Add a description to the card, and note that it can wrap for several lines.
            var apodDesc = new AdaptiveTextBlock
            {
                Text = volItem.Description,
                Size = AdaptiveTextSize.Default,
                Weight = AdaptiveTextWeight.Lighter,
                Wrap = true,
                MaxLines = 4,
            };
            apodTimelineCard.Body.Add(apodDesc);
        }

        private static async Task GenerateActivityAsync(LuoVol volItem)
        {
            // Get the default UserActivityChannel and query it for our UserActivity. If the activity doesn't exist, one is created.
            UserActivityChannel channel = UserActivityChannel.GetDefault();

            // The text here should be treated as a title for this activity and should be unique to this app.
            UserActivity userActivity = await channel.GetOrCreateUserActivityAsync("LuoVol." + volItem.VolNum);

            // Populate required properties: DisplayText and ActivationUri are required.
            userActivity.VisualElements.DisplayText = "Luoo-UWP Timeline activities";

            // The name in the ActivationUri must match the name in the protocol setting in the manifest file (except for the "://" part).
            userActivity.ActivationUri = new Uri("luoo://?volnum=" + volItem.VolNum);

            // Build the adaptive card from a JSON string.
            userActivity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(apodTimelineCard.ToJson());

            // Set the mime type of the user activity, in this case, an application.
            userActivity.ContentType = "application/octet-stream";

            // Save the new metadata.
            await userActivity.SaveAsync();

            // Dispose of any current UserActivitySession, and create a new one.
            _currentActivity?.Dispose();
            _currentActivity = userActivity.CreateSession();
        }
    }
}
