namespace FriendOrganizer.DataAccess.Migrations
{
  using FriendOrganizer.Model;
  using System;
  using System.Collections.Generic;
  using System.Data.Entity;
  using System.Data.Entity.Migrations;
  using System.Linq;

  internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
    {
      context.Friends.AddOrUpdate(
        f => f.FirstName,
        new Friend { FirstName = "Thomas", LastName = "Huber" },
        new Friend { FirstName = "Urs", LastName = "Meier" },
        new Friend { FirstName = "Erkan", LastName = "Egin" },
        new Friend { FirstName = "Sara", LastName = "Huber" }
        );
      context.ProgrammingLanguages.AddOrUpdate(
        pl => pl.Name,
        new ProgrammingLanguage { Name = "C#" },
        new ProgrammingLanguage { Name = "TypeScript" },
        new ProgrammingLanguage { Name = "F#" },
        new ProgrammingLanguage { Name = "Swift" },
        new ProgrammingLanguage { Name = "Java" });

      context.SaveChanges();

      context.FriendPhoneNumbers.AddOrUpdate(pn => pn.Number,
        new FriendPhoneNumber { Number = "+49 12345678", FriendId = context.Friends.First().Id });

      context.Meetings.AddOrUpdate(m => m.Title,
        new Meeting
        {
          Title = "Watching Soccer",
          DateFrom = new DateTime(2018, 5, 26),
          DateTo = new DateTime(2018, 5, 26),
          Friends = new List<Friend>
          {
            context.Friends.Single(f => f.FirstName == "Thomas" && f.LastName == "Huber"),
            context.Friends.Single(f => f.FirstName == "Urs" && f.LastName == "Meier")
          }
        });
    }
  }
}
