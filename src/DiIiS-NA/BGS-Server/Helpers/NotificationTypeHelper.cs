using System;
using System.Collections.Generic;
using System.Text;

namespace DiIiS_NA.LoginServer.Helpers
{
	public static class NotificationTypeHelper
	{
		/// <summary>
		/// Returns the NotificationType for the given notification.
		/// </summary>
		/// <param name="notification">The notification</param>
		/// <returns><see cref="NotificationType"/></returns>
		public static NotificationType GetNotificationType(this bgs.protocol.notification.v1.Notification notification)
		{
			switch (notification.Type)
			{
				case "WHISPER":
					return NotificationType.Whisper;
			}
			return NotificationType.Unknown;
		}

		/// <summary>
		/// Notification types
		/// </summary>
		public enum NotificationType
		{
			Unknown,
			Whisper
		}
	}
}
