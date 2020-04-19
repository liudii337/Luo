using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Luo.Shared.Helper
{
    public class Tile
    {
        public static void ShowTileNotification(string SongTitle, string SongArtist, string SongCover, string VolNum, string VolTitle, string VolCover)
        {
            string songxml = $@"
                    <tile>
                     <visual branding='nameAndLogo'>
                        <binding template='TileMedium' displayName='落网' branding='nameAndLogo' hint-textStacking='center'>
                            <image placement='peek' src='{SongCover}'/>
                            <text hint-align='center' hint-style='caption' hint-wrap='true' hint-maxLines='2'>{SongTitle}</text>
                            <text hint-align='center' hint-style='captionSubtle' hint-wrap='true' hint-maxLines='2'>{SongArtist}</text>
                        </binding>  
 					    <binding template='TileWide' displayName='落网' branding='nameAndLogo' hint-textStacking='center'>
 					        <group>
                                <subgroup hint-weight='33'>
                                    <image src='{SongCover}'/>
                                </subgroup>
                                <subgroup hint-textStacking='center'>
                                    <text hint-style='base' hint-wrap='true' hint-maxLines='2'>{SongTitle}</text>
                                    <text hint-style='captionSubtle' hint-wrap='true' hint-maxLines='2'>{SongArtist}</text>
                                </subgroup>
                            </group>
                            <image placement='peek' src='{SongCover}'/>
                        </binding>
 					    <binding template='TileLarge' displayName='落网' branding='nameAndLogo' hint-textStacking='center'>
                            <text hint-align='center' hint-style='base' hint-wrap='true' hint-maxLines='2'>{SongTitle}</text>
                            <text hint-align='center' hint-style='captionSubtle' hint-wrap='true' hint-maxLines='2'>{SongArtist}</text>
                            <image src='{SongCover}'/>
                            <image placement='background' hint-overlay='75' src='{SongCover}' />
                        </binding>
                     </visual>
                    </tile>
                    ";
            //<image placement="peek" src="Assets/Apps/Hipstame/hipster.jpg"/>
            string volxml = $@"
                    <tile>
                     <visual branding='nameAndLogo'>
                        <binding template='TileMedium' displayName='落网' branding='nameAndLogo' hint-textStacking='center'>
                            <image placement='peek' src='{VolCover}'/>
                            <text hint-align='center' hint-style='caption' hint-wrap='true' hint-maxLines='2'>Vol.{VolNum}</text>
                            <text hint-align='center' hint-style='captionSubtle' hint-wrap='true' hint-maxLines='2'>{VolTitle}</text>
                        </binding>  
 					    <binding template='TileWide' displayName='落网' branding='nameAndLogo' hint-textStacking='center'>
 					        <group>
                                <subgroup hint-weight='45'>
                                    <image src='{VolCover}'/>
                                </subgroup>
                                <subgroup hint-textStacking='center'>
                                    <text hint-style='base' hint-wrap='true' hint-maxLines='2'>Vol.{VolNum}</text>
                                    <text hint-style='captionSubtle' hint-wrap='true' hint-maxLines='2'>{VolTitle}</text>
                                </subgroup>
                            </group>
                            <image placement='background' hint-overlay='75' src='{VolCover}' />
                        </binding>
 					    <binding template='TileLarge' displayName='落网' branding='nameAndLogo' hint-textStacking='center'>
                            <text hint-align='center' hint-style='base' hint-wrap='true' hint-maxLines='2'>Vol.{VolNum}</text>
                            <text hint-align='center' hint-style='captionSubtle' hint-wrap='true' hint-maxLines='2'>{VolTitle}</text>
                            <image src='{VolCover}'/>
                            <image placement='background' hint-overlay='75' src='{VolCover}' />
                        </binding>
                     </visual>
                    </tile>
                    ";


            XmlDocument doc = new XmlDocument();

            //string nowTimeString = DateTime.Now.ToString();

            //// Assign date/time values through XmlDocument to avoid any xml escaping issues
            //foreach (XmlElement textEl in doc.SelectNodes("//text").OfType<XmlElement>())
            //    if (textEl.InnerText.Length == 0)
            //        textEl.InnerText = nowTimeString;

            //TileNotification notification = new TileNotification(doc);
            //TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Clear();

            updater.EnableNotificationQueue(true);
            // using above to create new tile

            // And send the notification
            doc.LoadXml(songxml);
            var songnoti = new ScheduledTileNotification(doc, DateTime.Now.AddSeconds(1));
            songnoti.ExpirationTime = DateTime.Now.AddMinutes(10);
            updater.AddToSchedule(songnoti);

            doc.LoadXml(volxml);
            var volnoti = new ScheduledTileNotification(doc, DateTime.Now.AddSeconds(5));
            updater.AddToSchedule(volnoti);
        }

        public static void ClearTileNotification()
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Clear();
        }
    }
}
