﻿using System;
using System.Net.Http;
using System.Collections.Generic;

namespace VolleyballApp {
	class DB_SelectEvent : DB_Select{
		public List<MySqlEvent> listEvent;
		
		public DB_SelectEvent(DB_Communicator dbCommunicator) : base(dbCommunicator) {}

		public async void SelectEventsForUser(string host, string requestEventsForUser, int idUser, string state) {
			HttpResponseMessage response = new HttpResponseMessage();
			Uri uri = new Uri(host + requestEventsForUser + "?idUser=" + idUser + "&state=" + state);

			string responseText;
			try {
				response = await base.client.GetAsync(uri);
				response.EnsureSuccessStatusCode();
				responseText = await response.Content.ReadAsStringAsync();

				listEvent = createEventFromResponse(responseText);
			} catch(Exception e) {
				Console.WriteLine("Error while selecting data from MySQL: " + e.Message);
			}
		}

		private List<MySqlEvent> createEventFromResponse(string response) {
			if(base.dbCommunicator.wasSuccesful(response)) {
				string[] eventInfo = response.Split('|');
				List<MySqlEvent> listEvent = new List<MySqlEvent>();

				int i = 0;
				do {
					if(debug) {
						Console.WriteLine("Creating Event: " + eventInfo[i] + " " + eventInfo[i + 1] + " " + eventInfo[i + 2] + " " + eventInfo[i + 3] + " " + eventInfo[i + 4]);
					}
					listEvent.Add(new MySqlEvent(Convert.ToInt32(eventInfo[i]), eventInfo[i + 1], Convert.ToDateTime(eventInfo[i + 2]), Convert.ToDateTime(eventInfo[i + 3]), eventInfo[i + 4]));
					i += 5;
				} while(!eventInfo[i].Equals("<endoffile>")) ;

				return listEvent;
			} else {
				Console.WriteLine("Invalid response for creating event!");
				return null;
			}
		}
	}
}

