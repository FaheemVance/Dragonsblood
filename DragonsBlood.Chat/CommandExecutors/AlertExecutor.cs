using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DragonsBlood.Chat.Data;
using DragonsBlood.Data;
using DragonsBlood.Models.AlertModels;
using DragonsBlood.Models.ChatModels;
using DragonsBlood.Models.CustomModels;

namespace DragonsBlood.Chat.CommandExecutors
{
    public sealed class AlertExecutor : ExecutorBase
    {
        public override bool Execute(List<string> parameters, string requestorConnectionId="", string room="")
        {
            if (!parameters.Any())
            {
                DisplayHelp();
                return false;
            }
            var newAlert = GenerateNewAlertFromParameters(parameters);
            
            if(newAlert == null)
            {
                DisplayHelp();
                return false;
            }

            
            var appUser = AppContext.Users.Include(u => u.Alerts).FirstOrDefault(u => u.UserName == HttpContext.Current.User.Identity.Name);

            if (appUser == null)
                return false;

            var alerts = appUser.Alerts;

            var existingAlert =
                alerts.FirstOrDefault(
                    a =>
                        a.Kingdom == newAlert.Kingdom && a.Attacker == newAlert.Attacker &&
                        a.Coordinates.X == newAlert.Coordinates.X && a.Coordinates.Y == newAlert.Coordinates.Y);

            if (existingAlert != null)
            {
                // Look for any existing alerts that are the same.
                NotifyUser(
                    $"You have already raised an alert on: {existingAlert.TimeStamp.ToString("g")} : {existingAlert.Kingdom} - {existingAlert.Attacker} - {existingAlert.Coordinates.X}:{existingAlert.Coordinates.Y}");
                return false;
            }

            using (var context = AppContext)
            {
                var user = context.Users.Include(u => u.Alerts).FirstOrDefault(u => u.UserName == appUser.UserName);

                if (user != null)
                {
                    user.Alerts.Add(newAlert);
                    context.SaveChanges();
                    NotifyUser(
                        $"Alert has been raised. {newAlert.Kingdom} - {newAlert.Attacker} - {newAlert.Coordinates.X}:{newAlert.Coordinates.Y}");

                    UpdateClientAlerts();
                    return true;
                }
            }
            return false;
        }

        private Kingdoms GetKingdomFromShortKingdom(ShortKingdom kingdom)
        {
            switch (kingdom)
            {
                case ShortKingdom.High:
                    return Kingdoms.HighKingdom;
                case ShortKingdom.Ice:
                    return Kingdoms.IceStormMountains;
                case ShortKingdom.Dark:
                    return Kingdoms.DarkMarshes;
                default:
                    return Kingdoms.HighKingdom;
            }
        }

        private Alert GenerateNewAlertFromParameters(List<string> parameters)
        {
            var expectedParams = Commands.GetUserCommands().First(c => c.Type == CommandType.Alert).Parameters.Count;
            var userNameHasSpaces = parameters.Count != expectedParams;
            var userName = string.Empty;

            userName = userNameHasSpaces ? GetAttackerFromParameters(parameters) : parameters[1];

            var sKingdom = parameters.First();
            var coordString = parameters.Last();
            var newAlert = new Alert();

            ShortKingdom Short;
            var success = Enum.TryParse(sKingdom, true, out Short);

            if (!success)
                return null;

            newAlert.Coordinates = new Coordinates().Parse(coordString);

            if (newAlert.Coordinates == null)
                return null;

            newAlert.ShortKingdom = Short;
            newAlert.Kingdom = GetKingdomFromShortKingdom(Short);
            newAlert.Attacker = userName;
            newAlert.TimeStamp = DateTime.UtcNow;

            return newAlert;
        }

        private string GetAttackerFromParameters(List<string> parameters)
        {
            var paramList = parameters.ToList();

            paramList.Remove(paramList.First());
            paramList.Remove(paramList.Last());

            return paramList.Aggregate((name, part) => name + " " + part);
        }

        private void DisplayHelp()
        {
            NotifyUser("You have specified incorrect parameters.");
            NotifyUser("Please use: /Alert {Kingdom [High|Ice|Dark]} {Attacker} {Coordinates[0:0]} e.g.");
            NotifyUser("/Alert Ice HonorlessLegend -32:24");
        }

        private void UpdateClientAlerts()
        {
            using (var context = new ApplicationDbContext())
            {
                var alertCount = context.Alerts.Count(a => !a.Retaliated);
                Hub.Clients.All.updateAlerts(alertCount);
            }
        }
    }
}
