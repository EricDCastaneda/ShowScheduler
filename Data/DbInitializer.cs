using ShowScheduler.Models;
using System;
using System.Linq;

namespace ShowScheduler.Data
{
    public class DbInitializer
    {
        public static void Initialize(ShowSchedulerContext context)
        {
            context.Database.EnsureCreated();

            // Look for any shows.
            if (context.Show.Any())
            {
                return;   // DB has been seeded
            }

            var shows = new Show[]
            {
            new Show{Date=new DateTime(2001,10,24),ShowName="Lateralus",Venue="Reunion Arena"},
            new Show{Date=new DateTime(1995,12,15),ShowName="Frogstomp",Venue="Bomb Factory"},
            new Show{Date=new DateTime(1991,10,19),ShowName="Nevermind",Venue="Trees"},
            new Show{Date=new DateTime(2018,6,27),ShowName="North American Summer Tour 2018",Venue="Dos Equis Pavilion"},
            new Show{Date=new DateTime(2005,10,29),ShowName="Vertigo",Venue="American Airlines Center"},
            new Show{Date=new DateTime(2000,11,1),ShowName="Back to School Tour",Venue="Fair Park Coliseum"},
            new Show{Date=new DateTime(1994,7,8),ShowName="Purple",Venue="Coca-Cola Starplex Amphitheatre"},
            new Show{Date=new DateTime(1993,10,6),ShowName="August & Everything After",Venue="Deep Ellum Live"},
            new Show{Date=new DateTime(1993,5,18),ShowName="1993 Tour",Venue="The Lizard Lounge"},
            new Show{Date=new DateTime(1994,10,20),ShowName="Lollapalooza 1994",Venue="Coca-Cola Starplex Amphitheatre"},
            new Show{Date=new DateTime(2009,12,13),ShowName="Lost in the Sound of Separation",Venue="House of Blues Dallas"},
            new Show{Date=new DateTime(2018,2,10),ShowName="25th Anniversary of New Miserable Experience Tour",Venue="House of Blues Dallas"},
            new Show{Date=new DateTime(2014,9,11),ShowName="Rubberneck 20th Anniversary",Venue="Pecan Lodge"},
            new Show{Date=new DateTime(1999,1,24),ShowName="World Domination Tour",Venue="Reunion Arena"},
            new Show{Date=new DateTime(2012,8,27),ShowName="Honda Civic Tour 2012",Venue="Gexa Energy Pavilion"}
            };

            foreach (Show s in shows)
            {
                context.Show.Add(s);
            }
            context.SaveChanges();

            var bands = new Band[]
            {
            new Band{ShowID=1,BandName="Tool",StartTime=new DateTime(2001,10,24,22,0,0),EndTime=new DateTime(2001,10,24,23,30,0)},
            new Band{ShowID=2,BandName="Silverchair",StartTime=new DateTime(1995,12,15,22,0,0),EndTime=new DateTime(1995,12,15,23,30,0)},
            new Band{ShowID=3,BandName="Nirvana",StartTime=new DateTime(1991,10,19,22,0,0),EndTime=new DateTime(1991,10,19,23,30,0)},
            new Band{ShowID=4,BandName="Weezer",StartTime=new DateTime(2018,6,27,22,0,0),EndTime=new DateTime(2018,6,27,23,30,0)},
            new Band{ShowID=5,BandName="U2",StartTime=new DateTime(2005,10,29,22,0,0),EndTime=new DateTime(2005,10,29,23,30,0)},
            new Band{ShowID=6,BandName="Deftones",StartTime=new DateTime(2000,11,1,22,0,0),EndTime=new DateTime(2000,11,1,23,30,0)},
            new Band{ShowID=7,BandName="Stone Temple Pilots",StartTime=new DateTime(1994,7,8,22,0,0),EndTime=new DateTime(1994,7,8,23,30,0)},
            new Band{ShowID=8,BandName="Counting Crows",StartTime=new DateTime(1993,10,6,22,0,0),EndTime=new DateTime(1993,10,6,23,30,0)},
            new Band{ShowID=9,BandName="311",StartTime=new DateTime(1993,5,18,22,0,0),EndTime=new DateTime(1993,5,18,23,30,0)},
            new Band{ShowID=10,BandName="Green Day",StartTime=new DateTime(1994,10,20,22,0,0),EndTime=new DateTime(1994,10,20,23,30,0)},
            new Band{ShowID=11,BandName="Underoath",StartTime=new DateTime(2009,12,13,22,0,0),EndTime=new DateTime(2009,12,13,23,30,0)},
            new Band{ShowID=12,BandName="Gin Blossoms",StartTime=new DateTime(2018,2,10,22,0,0),EndTime=new DateTime(2018,2,10,23,30,0)},
            new Band{ShowID=13,BandName="Toadies",StartTime=new DateTime(2014,9,11,22,0,0),EndTime=new DateTime(2014,9,11,23,30,0)},
            new Band{ShowID=14,BandName="Pantera",StartTime=new DateTime(1999,1,24,22,0,0),EndTime=new DateTime(1999,1,24,23,30,0)},
            new Band{ShowID=15,BandName="Incubus",StartTime=new DateTime(2012,8,27,22,0,0),EndTime=new DateTime(2012,8,27,23,30,0)}
            };

            foreach (Band b in bands)
            {
                context.Band.Add(b);
            }
            context.SaveChanges();

        }
    }
}


