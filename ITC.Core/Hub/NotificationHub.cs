// File: Hubs/NotificationHub.cs
using Microsoft.AspNetCore.SignalR;

namespace ITC.Core.Hubs
{
	public class NotificationHub : Hub
	{
		// Gửi theo userId. Ở client cần map token → userId
		public override Task OnConnectedAsync()
		{
			Console.WriteLine($"Client connected: {Context.ConnectionId}");
			return base.OnConnectedAsync();
		}
	}
}
