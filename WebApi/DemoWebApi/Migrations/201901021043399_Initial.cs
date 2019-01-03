namespace DemoWebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Idclient = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Idclient);
            
            CreateTable(
                "dbo.Hotels",
                c => new
                    {
                        IdHotel = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Location = c.String(),
                        Category = c.Int(nullable: false),
                        HasWifi = c.Boolean(nullable: false),
                        HasParking = c.Boolean(nullable: false),
                        Phone = c.String(),
                        Email = c.String(),
                        Website = c.String(),
                    })
                .PrimaryKey(t => t.IdHotel);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        IdPicture = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Room_IdRoom = c.Int(),
                    })
                .PrimaryKey(t => t.IdPicture)
                .ForeignKey("dbo.Rooms", t => t.Room_IdRoom)
                .Index(t => t.Room_IdRoom);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        IdRoom = c.Int(nullable: false, identity: true),
                        IdHotel = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HasTV = c.Boolean(nullable: false),
                        HasHairDryer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdRoom);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        IdReservation = c.Int(nullable: false, identity: true),
                        IdClient = c.Int(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateStart = c.DateTime(nullable: false),
                        DateEnd = c.DateTime(nullable: false),
                        hotelName = c.String(),
                    })
                .PrimaryKey(t => t.IdReservation);
            
            CreateTable(
                "dbo.RoomReservations",
                c => new
                    {
                        IdRoomReservation = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Reservation_IdReservation = c.Int(),
                        Room_IdRoom = c.Int(),
                    })
                .PrimaryKey(t => t.IdRoomReservation)
                .ForeignKey("dbo.Reservations", t => t.Reservation_IdReservation)
                .ForeignKey("dbo.Rooms", t => t.Room_IdRoom)
                .Index(t => t.Reservation_IdReservation)
                .Index(t => t.Room_IdRoom);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoomReservations", "Room_IdRoom", "dbo.Rooms");
            DropForeignKey("dbo.RoomReservations", "Reservation_IdReservation", "dbo.Reservations");
            DropForeignKey("dbo.Pictures", "Room_IdRoom", "dbo.Rooms");
            DropIndex("dbo.RoomReservations", new[] { "Room_IdRoom" });
            DropIndex("dbo.RoomReservations", new[] { "Reservation_IdReservation" });
            DropIndex("dbo.Pictures", new[] { "Room_IdRoom" });
            DropTable("dbo.RoomReservations");
            DropTable("dbo.Reservations");
            DropTable("dbo.Rooms");
            DropTable("dbo.Pictures");
            DropTable("dbo.Hotels");
            DropTable("dbo.Clients");
        }
    }
}
