using API.Configs;
using API.Models.Notification;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PushSharp.Common;
using PushSharp.Google;
using System.Text;

namespace API.Services
{
    public class NotificationService
    {
        private const int _maxPayloadLength = 2048;
        private const int _maxAndroidPayloadLength = 4096;
        private readonly List<string> _messages;
        private readonly NotificationConfig.GoogleConfig _config;

        public NotificationService(IOptions<NotificationConfig> config)
        {
            _messages = new List<string>();
            _config = config.Value.Google ?? throw new ArgumentException("Google configuration not found");
        }

        public List<string> SendNotification(string notificationToken, NotificationModel notificationModel)
        {
            _messages.Clear();

            GcmConfiguration config = new GcmConfiguration(_config.ServerKey);
            config.GcmUrl = _config.GcmUrl;

            JObject jData = CreateDataMessage(notificationModel.CustomData);
            GcmNotification notify = new GcmNotification()
            {
                RegistrationIds = new List<string> { notificationToken },
                Data = jData,
                Notification = CreateMessage(notificationModel.Alert),
                ContentAvailable = jData["data"] != null,
            };

            GcmServiceBroker gcmBroker = new GcmServiceBroker(config);
            gcmBroker.OnNotificationFailed += GcmBroker_OnNotificationFailed;
            gcmBroker.OnNotificationSucceeded += GcmBroker_OnNotificationSucceeded;

            gcmBroker.Start();
            gcmBroker.QueueNotification(notify);
            gcmBroker.Stop();

            return _messages;
        }

        private JObject CreateMessage(AlertModel alert)
        {
            JObject jNotify = new JObject();

            if (!string.IsNullOrWhiteSpace(alert.Title))
                jNotify["title"] = alert.Title;

            if (!string.IsNullOrWhiteSpace(alert.Body))
            {
                jNotify["body"] = alert.Body;

                int payloadLength = Encoding.UTF8.GetBytes(jNotify.ToString(Newtonsoft.Json.Formatting.None)).Length;
                if (payloadLength > _maxAndroidPayloadLength)
                {
                    int difference = payloadLength - _maxPayloadLength + 3; // + 3 for add "..." in the end
                    jNotify["body"] = alert.Body.Length - difference <= 0 ? null : alert.Body[..^difference] + "...";
                }
            }

            return jNotify;
        }

        private JObject CreateDataMessage(object? customData)
        {
            JObject jData = new JObject();
            JObject jCustomData = new JObject();

            if (customData != null)
                jCustomData = JObject.FromObject(customData);

            jData["data"] = jCustomData;
            return jData;
        }

        private void GcmBroker_OnNotificationSucceeded(GcmNotification notification)
        {
            _messages.Add("An alert push has been successfully sent!");
        }

        private void GcmBroker_OnNotificationFailed(GcmNotification notification, AggregateException exception)
        {
            exception.Handle(exceptionHandle =>
            {
                switch (exceptionHandle)
                {
                    case GcmNotificationException gcmNotificationException:
                        GcmNotification gcmNotification = gcmNotificationException.Notification;
                        _messages.Add($"Unable to send notification to {gcmNotification.Tag} : ID={gcmNotification.MessageId}. Exception:{exceptionHandle}");
                        break;

                    case GcmMulticastResultException multicastException:
                        foreach (var succeededNotification in multicastException.Succeeded)
                            _messages.Add($"Notification for '{succeededNotification.Tag}' has been sent");

                        foreach (var failedNotification in multicastException.Failed)
                            _messages.Add($"Unable to send notification to {failedNotification.Key.Tag} : ID={failedNotification.Key.MessageId}. Exception:{exceptionHandle}");
                        break;

                    case DeviceSubscriptionExpiredException expiredException:
                        _messages.Add($"Unable to send notification. Token is expired. Exception:{exceptionHandle}");
                        break;

                    case RetryAfterException retryException:
                        _messages.Add($"Rate Limited, try to send after {retryException.RetryAfterUtc} UTC. Exception: {exceptionHandle}");
                        break;

                    default:
                        _messages.Add($"Unexpected error:{exceptionHandle}");
                        break;
                }

                return true;
            });

        }
    }
}
